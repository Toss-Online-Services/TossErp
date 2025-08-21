using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Projects.Application.Common.Interfaces;

namespace Projects.Infrastructure.HealthChecks;

/// <summary>
/// Health check for Projects API infrastructure components
/// </summary>
public class ProjectsHealthCheck : IHealthCheck
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<ProjectsHealthCheck> _logger;

    public ProjectsHealthCheck(IServiceScopeFactory serviceScopeFactory, ILogger<ProjectsHealthCheck> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var healthData = new Dictionary<string, object>();

            // Check database connectivity
            var dbHealthy = await CheckDatabaseHealth(scope, healthData, cancellationToken);
            
            // Check repository health
            var repoHealthy = await CheckRepositoryHealth(scope, healthData, cancellationToken);
            
            // Check business services health
            var servicesHealthy = await CheckServicesHealth(scope, healthData, cancellationToken);

            // Check background services status
            var backgroundServicesHealthy = await CheckBackgroundServicesHealth(scope, healthData, cancellationToken);

            // Determine overall health status
            var overallHealthy = dbHealthy && repoHealthy && servicesHealthy && backgroundServicesHealthy;

            var status = overallHealthy ? HealthStatus.Healthy : HealthStatus.Unhealthy;
            var description = overallHealthy ? "Projects API is healthy" : "Projects API has health issues";

            _logger.LogInformation("Projects health check completed with status: {Status}", status);

            return new HealthCheckResult(status, description, data: healthData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during Projects health check");
            
            var errorData = new Dictionary<string, object>
            {
                { "error", ex.Message },
                { "exception_type", ex.GetType().Name },
                { "timestamp", DateTime.UtcNow }
            };

            return new HealthCheckResult(HealthStatus.Unhealthy, "Health check failed due to exception", ex, errorData);
        }
    }

    private async Task<bool> CheckDatabaseHealth(IServiceScope scope, Dictionary<string, object> healthData, CancellationToken cancellationToken)
    {
        try
        {
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            
            // Test database connectivity by attempting a simple operation
            var startTime = DateTime.UtcNow;
            await unitOfWork.TestConnectionAsync(cancellationToken);
            var responseTime = (DateTime.UtcNow - startTime).TotalMilliseconds;

            healthData.Add("database_status", "healthy");
            healthData.Add("database_response_time_ms", responseTime);
            
            _logger.LogDebug("Database health check passed in {ResponseTime}ms", responseTime);
            return true;
        }
        catch (Exception ex)
        {
            healthData.Add("database_status", "unhealthy");
            healthData.Add("database_error", ex.Message);
            
            _logger.LogWarning(ex, "Database health check failed");
            return false;
        }
    }

    private async Task<bool> CheckRepositoryHealth(IServiceScope scope, Dictionary<string, object> healthData, CancellationToken cancellationToken)
    {
        try
        {
            var projectRepository = scope.ServiceProvider.GetRequiredService<IProjectRepository>();
            var taskRepository = scope.ServiceProvider.GetRequiredService<ITaskRepository>();
            var timeEntryRepository = scope.ServiceProvider.GetRequiredService<ITimeEntryRepository>();

            // Test repository operations
            var startTime = DateTime.UtcNow;
            
            // Test basic query operations (should not throw exceptions)
            var projectCount = await projectRepository.GetProjectCountAsync();
            var taskCount = await taskRepository.GetTaskCountAsync();
            var timeEntryCount = await timeEntryRepository.GetTimeEntryCountAsync();
            
            var responseTime = (DateTime.UtcNow - startTime).TotalMilliseconds;

            healthData.Add("repositories_status", "healthy");
            healthData.Add("repositories_response_time_ms", responseTime);
            healthData.Add("project_count", projectCount);
            healthData.Add("task_count", taskCount);
            healthData.Add("time_entry_count", timeEntryCount);
            
            _logger.LogDebug("Repository health check passed in {ResponseTime}ms", responseTime);
            return true;
        }
        catch (Exception ex)
        {
            healthData.Add("repositories_status", "unhealthy");
            healthData.Add("repositories_error", ex.Message);
            
            _logger.LogWarning(ex, "Repository health check failed");
            return false;
        }
    }

    private async Task<bool> CheckServicesHealth(IServiceScope scope, Dictionary<string, object> healthData, CancellationToken cancellationToken)
    {
        try
        {
            // Test that required services can be resolved
            var projectService = scope.ServiceProvider.GetService<IProjectService>();
            var taskService = scope.ServiceProvider.GetService<ITaskService>();
            var timeTrackingService = scope.ServiceProvider.GetService<ITimeTrackingService>();

            var servicesHealthy = projectService != null && taskService != null && timeTrackingService != null;

            healthData.Add("services_status", servicesHealthy ? "healthy" : "unhealthy");
            healthData.Add("project_service_available", projectService != null);
            healthData.Add("task_service_available", taskService != null);
            healthData.Add("time_tracking_service_available", timeTrackingService != null);

            if (servicesHealthy)
            {
                _logger.LogDebug("Services health check passed");
            }
            else
            {
                _logger.LogWarning("Services health check failed - some services unavailable");
            }

            await Task.CompletedTask;
            return servicesHealthy;
        }
        catch (Exception ex)
        {
            healthData.Add("services_status", "unhealthy");
            healthData.Add("services_error", ex.Message);
            
            _logger.LogWarning(ex, "Services health check failed");
            return false;
        }
    }

    private async Task<bool> CheckBackgroundServicesHealth(IServiceScope scope, Dictionary<string, object> healthData, CancellationToken cancellationToken)
    {
        try
        {
            // Check if background services are registered
            var hostedServices = scope.ServiceProvider.GetServices<IHostedService>();
            
            var projectStatusUpdateService = hostedServices.Any(s => s.GetType().Name == "ProjectStatusUpdateService");
            var timeEntryReminderService = hostedServices.Any(s => s.GetType().Name == "TimeEntryReminderService");
            var dataCleanupService = hostedServices.Any(s => s.GetType().Name == "DataCleanupService");

            var backgroundServicesHealthy = projectStatusUpdateService && timeEntryReminderService && dataCleanupService;

            healthData.Add("background_services_status", backgroundServicesHealthy ? "healthy" : "unhealthy");
            healthData.Add("project_status_update_service", projectStatusUpdateService);
            healthData.Add("time_entry_reminder_service", timeEntryReminderService);
            healthData.Add("data_cleanup_service", dataCleanupService);
            healthData.Add("total_hosted_services", hostedServices.Count());

            if (backgroundServicesHealthy)
            {
                _logger.LogDebug("Background services health check passed");
            }
            else
            {
                _logger.LogWarning("Background services health check failed - some services not registered");
            }

            await Task.CompletedTask;
            return backgroundServicesHealthy;
        }
        catch (Exception ex)
        {
            healthData.Add("background_services_status", "unhealthy");
            healthData.Add("background_services_error", ex.Message);
            
            _logger.LogWarning(ex, "Background services health check failed");
            return false;
        }
    }
}
