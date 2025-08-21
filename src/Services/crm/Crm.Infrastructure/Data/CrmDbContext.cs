using Crm.Domain.Entities;
using TossErp.CRM.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace Crm.Infrastructure.Data;

public class CrmDbContext : DbContext
{
    public CrmDbContext(DbContextOptions<CrmDbContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<CustomerInteraction> CustomerInteractions { get; set; } = null!;
    public DbSet<LoyaltyTransaction> LoyaltyTransactions { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Use PostgreSQL schema
        modelBuilder.HasDefaultSchema("crm");

        // Apply all entity configurations from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Global query filters for multi-tenancy (if needed in future)
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            // Add soft delete filter for all entities
            var isDeletedProperty = entityType.FindProperty("IsDeleted");
            if (isDeletedProperty != null && isDeletedProperty.ClrType == typeof(bool))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var body = Expression.Equal(
                    Expression.Property(parameter, isDeletedProperty.PropertyInfo!),
                    Expression.Constant(false));
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(Expression.Lambda(body, parameter));
            }

            // Add tenant filter if TenantId exists
            var tenantIdProperty = entityType.FindProperty("TenantId");
            if (tenantIdProperty != null && tenantIdProperty.ClrType == typeof(Guid))
            {
                // For now, we'll add the property but not filter - can be enabled when multi-tenancy is implemented
            }
        }

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Set audit fields before saving
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is Entity entity)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        if (entity.IsTransient())
                        {
                            // Let the entity constructor handle ID generation
                        }
                        break;
                    case EntityState.Modified:
                        // Handle any modification audit logic if needed
                        break;
                }
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
