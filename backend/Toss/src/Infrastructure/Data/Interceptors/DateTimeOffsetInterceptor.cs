using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Reflection;

namespace Toss.Infrastructure.Data.Interceptors;

/// <summary>
/// Interceptor to ensure all DateTimeOffset values are converted to UTC before saving to PostgreSQL.
/// PostgreSQL only supports UTC offsets for timestamp with time zone columns.
/// </summary>
public class DateTimeOffsetInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        NormalizeDateTimeOffsets(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        NormalizeDateTimeOffsets(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void NormalizeDateTimeOffsets(DbContext? context)
    {
        if (context == null) return;

        var entries = context.ChangeTracker.Entries();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
            {
                // Use EF Core's metadata API for more reliable property access
                var entityType = entry.Metadata.ClrType;
                var properties = entry.Metadata.GetProperties();

                foreach (var property in properties)
                {
                    var propertyType = property.ClrType;
                    
                    if (propertyType == typeof(DateTimeOffset) || propertyType == typeof(DateTimeOffset?))
                    {
                        var currentValue = entry.CurrentValues[property.Name];
                        
                        if (currentValue is DateTimeOffset dateTimeOffset)
                        {
                            // Convert to UTC if not already UTC
                            if (dateTimeOffset.Offset != TimeSpan.Zero)
                            {
                                entry.CurrentValues[property.Name] = dateTimeOffset.ToUniversalTime();
                            }
                        }
                        else if (currentValue != null && propertyType == typeof(DateTimeOffset?))
                        {
                            var nullableDto = (DateTimeOffset?)currentValue;
                            if (nullableDto.HasValue && nullableDto.Value.Offset != TimeSpan.Zero)
                            {
                                entry.CurrentValues[property.Name] = nullableDto.Value.ToUniversalTime();
                            }
                        }
                    }
                }
            }
        }
    }
}

