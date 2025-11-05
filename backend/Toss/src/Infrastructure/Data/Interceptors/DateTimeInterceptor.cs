using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Toss.Infrastructure.Data.Interceptors;

/// <summary>
/// Interceptor to ensure all DateTime values are converted to UTC before saving to PostgreSQL.
/// PostgreSQL only supports UTC DateTime values for timestamp with time zone columns.
/// </summary>
public class DateTimeInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        NormalizeDateTimes(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        NormalizeDateTimes(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void NormalizeDateTimes(DbContext? context)
    {
        if (context == null) return;

        var entries = context.ChangeTracker.Entries();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
            {
                // Use EF Core's metadata API for more reliable property access
                var properties = entry.Metadata.GetProperties();

                foreach (var property in properties)
                {
                    var propertyType = property.ClrType;

                    if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
                    {
                        try
                        {
                            var currentValue = entry.CurrentValues[property.Name];

                            if (currentValue is DateTime dateTime)
                            {
                                // Convert to UTC if not already UTC
                                if (dateTime.Kind != DateTimeKind.Utc)
                                {
                                    entry.CurrentValues[property.Name] = dateTime.Kind == DateTimeKind.Local
                                        ? dateTime.ToUniversalTime()
                                        : DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
                                }
                            }
                            else if (currentValue != null && propertyType == typeof(DateTime?))
                            {
                                var nullableDateTime = (DateTime?)currentValue;
                                if (nullableDateTime.HasValue)
                                {
                                    var dt = nullableDateTime.Value;
                                    if (dt.Kind != DateTimeKind.Utc)
                                    {
                                        entry.CurrentValues[property.Name] = dt.Kind == DateTimeKind.Local
                                            ? dt.ToUniversalTime()
                                            : DateTime.SpecifyKind(dt, DateTimeKind.Utc);
                                    }
                                }
                            }
                        }
                        catch
                        {
                            // Skip properties that can't be accessed (e.g., shadow properties without values)
                            continue;
                        }
                    }
                }
            }
        }
    }
}





