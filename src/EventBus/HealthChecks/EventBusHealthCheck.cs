using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using eShop.EventBus.Configuration;

namespace eShop.EventBus.HealthChecks;

public class EventBusHealthCheck : IHealthCheck
{
    private readonly EventBusConfiguration _configuration;
    private readonly ILogger<EventBusHealthCheck> _logger;

    public EventBusHealthCheck(
        IOptions<EventBusConfiguration> configuration,
        ILogger<EventBusHealthCheck> logger)
    {
        _configuration = configuration.Value;
        _logger = logger;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if configuration is valid
            if (string.IsNullOrEmpty(_configuration.EventBusConnection))
            {
                _logger.LogWarning("EventBus connection string is not configured");
                return Task.FromResult(HealthCheckResult.Unhealthy("EventBus connection string is not configured"));
            }

            if (string.IsNullOrEmpty(_configuration.SubscriptionClientName))
            {
                _logger.LogWarning("EventBus subscription client name is not configured");
                return Task.FromResult(HealthCheckResult.Unhealthy("EventBus subscription client name is not configured"));
            }

            // For a more comprehensive health check, you could:
            // 1. Try to connect to RabbitMQ
            // 2. Check if the exchange exists
            // 3. Verify credentials

            _logger.LogDebug("EventBus health check passed");
            return Task.FromResult(HealthCheckResult.Healthy("EventBus is healthy"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "EventBus health check failed");
            return Task.FromResult(HealthCheckResult.Unhealthy("EventBus health check failed", ex));
        }
    }
}
