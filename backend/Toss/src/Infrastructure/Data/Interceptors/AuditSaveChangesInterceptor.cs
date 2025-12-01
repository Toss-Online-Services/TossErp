using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text.Json;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.Audit;
using Toss.Domain.Entities.Businesses;
using Toss.Infrastructure.Identity;

namespace Toss.Infrastructure.Data.Interceptors;

public class AuditSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUser _user;
    private readonly IBusinessContext _businessContext;

    public AuditSaveChangesInterceptor(
        IHttpContextAccessor httpContextAccessor,
        IUser user,
        IBusinessContext businessContext)
    {
        _httpContextAccessor = httpContextAccessor;
        _user = user;
        _businessContext = businessContext;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        AuditChanges(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        AuditChanges(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void AuditChanges(DbContext? context)
    {
        if (context == null) return;

        var userId = _user.Id;
        var httpContext = _httpContextAccessor.HttpContext;
        var userName = httpContext?.User?.Identity?.Name ?? httpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
        var ipAddress = httpContext?.Connection?.RemoteIpAddress?.ToString();

        // Get business ID from business context
        if (!_businessContext.HasBusiness) return; // Skip audit if no business context
        var businessId = _businessContext.CurrentBusinessId ?? 0;

        var entries = context.ChangeTracker.Entries()
            .Where(e => e.Entity is not AuditEntry && e.Entity is not Business) // Don't audit audit entries or business itself
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted)
            .ToList();

        var auditEntries = new List<AuditEntry>();

        foreach (var entry in entries)
        {
            var entity = entry.Entity;

            // Skip if entity doesn't have an ID property or BusinessId
            if (!HasIdProperty(entity) || !HasBusinessIdProperty(entity)) continue;

            var entityType = entity.GetType().Name;
            var entityId = GetEntityId(entity);
            var action = entry.State switch
            {
                EntityState.Added => "Created",
                EntityState.Modified => "Updated",
                EntityState.Deleted => "Deleted",
                _ => "Unknown"
            };

            string? changes = null;
            if (entry.State == EntityState.Modified)
            {
                changes = GetChangesJson(entry);
            }

            var auditEntry = new AuditEntry
            {
                BusinessId = businessId,
                EntityType = entityType,
                EntityId = entityId,
                Action = action,
                UserId = userId,
                UserName = userName,
                Changes = changes,
                IpAddress = ipAddress,
                Created = DateTimeOffset.UtcNow
            };

            auditEntries.Add(auditEntry);
        }

        if (auditEntries.Any())
        {
            context.Set<AuditEntry>().AddRange(auditEntries);
        }
    }

    private int? GetBusinessIdFromContext(DbContext context)
    {
        // Try to get BusinessId from any entity being tracked
        var entryWithBusiness = context.ChangeTracker.Entries()
            .FirstOrDefault(e => HasBusinessIdProperty(e.Entity));

        if (entryWithBusiness != null)
        {
            var businessIdProperty = entryWithBusiness.Entity.GetType()
                .GetProperty("BusinessId");
            
            if (businessIdProperty != null)
            {
                var value = businessIdProperty.GetValue(entryWithBusiness.Entity);
                if (value is int businessId)
                {
                    return businessId;
                }
            }
        }

        return null;
    }

    private bool HasIdProperty(object entity)
    {
        return entity.GetType().GetProperty("Id") != null;
    }

    private bool HasBusinessIdProperty(object entity)
    {
        return entity.GetType().GetProperty("BusinessId") != null;
    }

    private int GetEntityId(object entity)
    {
        var idProperty = entity.GetType().GetProperty("Id");
        if (idProperty?.GetValue(entity) is int id)
        {
            return id;
        }
        return 0;
    }

    private string? GetChangesJson(EntityEntry entry)
    {
        var changes = new Dictionary<string, object>();

        foreach (var property in entry.Properties)
        {
            if (property.IsModified)
            {
                var propertyName = property.Metadata.Name;
                
                // Skip certain properties
                if (propertyName == "LastModified" || propertyName == "Created" || propertyName == "BusinessId")
                    continue;

                var originalValue = property.OriginalValue;
                var currentValue = property.CurrentValue;

                changes[propertyName] = new
                {
                    from = originalValue,
                    to = currentValue
                };
            }
        }

        if (changes.Count == 0)
            return null;

        return JsonSerializer.Serialize(changes, new JsonSerializerOptions
        {
            WriteIndented = false,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });
    }
}

