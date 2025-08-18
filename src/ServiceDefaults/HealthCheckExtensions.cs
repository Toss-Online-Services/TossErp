using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;

namespace eShop.ServiceDefaults;

/// <summary>
/// Enhanced health check configuration
/// </summary>
public static class HealthCheckExtensions
{
    /// <summary>
    /// Add comprehensive health checks
    /// </summary>
    public static IServiceCollection AddTossHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            // Self health check
            .AddCheck("self", () => HealthCheckResult.Healthy("Service is running"), tags: new[] { "live", "ready" })
            
            // Memory health check
            .AddCheck<MemoryHealthCheck>("memory", tags: new[] { "ready" })
            
            // Disk space health check
            .AddCheck<DiskSpaceHealthCheck>("disk", tags: new[] { "ready" });

        return services;
    }

    /// <summary>
    /// Add database health checks
    /// </summary>
    public static IServiceCollection AddDatabaseHealthChecks(this IServiceCollection services, string connectionString)
    {
        services.AddHealthChecks()
            .AddNpgSql(connectionString, name: "postgres", tags: new[] { "ready", "database" });

        return services;
    }

    /// <summary>
    /// Add Redis health checks
    /// </summary>
    public static IServiceCollection AddRedisHealthChecks(this IServiceCollection services, string connectionString)
    {
        services.AddHealthChecks()
            .AddRedis(connectionString, name: "redis", tags: new[] { "ready", "cache" });

        return services;
    }

    /// <summary>
    /// Add RabbitMQ health checks
    /// </summary>
    public static IServiceCollection AddRabbitMQHealthChecks(this IServiceCollection services, string connectionString)
    {
        services.AddHealthChecks()
            .AddRabbitMQ(connectionString, name: "rabbitmq", tags: new[] { "ready", "messaging" });

        return services;
    }

    /// <summary>
    /// Map health check endpoints with proper responses
    /// </summary>
    public static IEndpointRouteBuilder MapTossHealthChecks(this IEndpointRouteBuilder app)
    {
        // Liveness endpoint - simple check if the app is alive
        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("live"),
            ResponseWriter = WriteHealthCheckResponse
        });

        // Readiness endpoint - checks if the app is ready to serve traffic
        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ready"),
            ResponseWriter = WriteHealthCheckResponse
        });

        // Detailed health endpoint - all health checks with details
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = WriteDetailedHealthCheckResponse
        });

        // Startup endpoint - checks that can be used during startup
        app.MapHealthChecks("/health/startup", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("startup"),
            ResponseWriter = WriteHealthCheckResponse
        });

        return app;
    }

    private static async Task WriteHealthCheckResponse(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json";
        
        var response = new
        {
            status = report.Status.ToString(),
            timestamp = DateTime.UtcNow,
            duration = report.TotalDuration.TotalMilliseconds
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }

    private static async Task WriteDetailedHealthCheckResponse(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json";

        var response = new
        {
            status = report.Status.ToString(),
            timestamp = DateTime.UtcNow,
            duration = report.TotalDuration.TotalMilliseconds,
            checks = report.Entries.Select(kvp => new
            {
                name = kvp.Key,
                status = kvp.Value.Status.ToString(),
                duration = kvp.Value.Duration.TotalMilliseconds,
                description = kvp.Value.Description,
                data = kvp.Value.Data,
                exception = kvp.Value.Exception?.Message,
                tags = kvp.Value.Tags
            })
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }
}

/// <summary>
/// Memory health check implementation
/// </summary>
public class MemoryHealthCheck : IHealthCheck
{
    private const long MaxMemoryBytes = 1024L * 1024L * 1024L; // 1 GB

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var allocated = GC.GetTotalMemory(forceFullCollection: false);
        var data = new Dictionary<string, object>
        {
            { "AllocatedMemory", allocated },
            { "MaxMemory", MaxMemoryBytes },
            { "MemoryUsagePercent", (allocated * 100) / MaxMemoryBytes }
        };

        var status = allocated < MaxMemoryBytes ? HealthStatus.Healthy : HealthStatus.Degraded;
        var description = $"Memory usage: {allocated / 1024 / 1024} MB";

        return Task.FromResult(new HealthCheckResult(status, description, data: data));
    }
}

/// <summary>
/// Disk space health check implementation
/// </summary>
public class DiskSpaceHealthCheck : IHealthCheck
{
    private const long MinFreeSpaceBytes = 1024L * 1024L * 1024L; // 1 GB

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var drives = DriveInfo.GetDrives().Where(d => d.IsReady && d.DriveType == DriveType.Fixed);
            var data = new Dictionary<string, object>();
            var overallStatus = HealthStatus.Healthy;

            foreach (var drive in drives)
            {
                var freeSpace = drive.AvailableFreeSpace;
                var totalSpace = drive.TotalSize;
                var usedSpace = totalSpace - freeSpace;
                var usagePercent = (usedSpace * 100) / totalSpace;

                data[$"Drive_{drive.Name.Replace("\\", "").Replace(":", "")}"] = new
                {
                    Name = drive.Name,
                    FreeSpace = freeSpace,
                    TotalSpace = totalSpace,
                    UsagePercent = usagePercent
                };

                if (freeSpace < MinFreeSpaceBytes)
                {
                    overallStatus = HealthStatus.Degraded;
                }
            }

            var description = $"Checked {drives.Count()} drives";
            return Task.FromResult(new HealthCheckResult(overallStatus, description, data: data));
        }
        catch (Exception ex)
        {
            return Task.FromResult(new HealthCheckResult(HealthStatus.Unhealthy, "Error checking disk space", ex));
        }
    }
}
