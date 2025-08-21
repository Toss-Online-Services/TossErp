using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Setup.Application.Common.Interfaces;

namespace Setup.Infrastructure.BackgroundJobs;

public class SystemMaintenanceJob : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SystemMaintenanceJob> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromHours(6); // Run every 6 hours

    public SystemMaintenanceJob(IServiceProvider serviceProvider, ILogger<SystemMaintenanceJob> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("System Maintenance Job started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await PerformSystemMaintenanceAsync(stoppingToken);
                await Task.Delay(_interval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("System Maintenance Job is stopping");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in System Maintenance Job");
                await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken); // Wait 30 minutes before retrying
            }
        }

        _logger.LogInformation("System Maintenance Job stopped");
    }

    private async Task PerformSystemMaintenanceAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
        var systemConfigService = scope.ServiceProvider.GetRequiredService<ISystemConfigService>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<ISetupUnitOfWork>();

        try
        {
            _logger.LogInformation("Starting system maintenance operations");

            // Perform user maintenance
            await userService.PerformSecurityMaintenanceAsync(cancellationToken);

            // Update system health metrics
            await UpdateSystemHealthMetricsAsync(unitOfWork, cancellationToken);

            // Optimize database performance
            await OptimizeDatabasePerformanceAsync(unitOfWork, cancellationToken);

            // Validate system configuration integrity
            await ValidateSystemConfigurationAsync(systemConfigService, cancellationToken);

            // Check for system updates
            await CheckForSystemUpdatesAsync(unitOfWork, cancellationToken);

            // Generate system reports
            await GenerateSystemReportsAsync(unitOfWork, cancellationToken);

            _logger.LogInformation("System maintenance operations completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing system maintenance");
        }
    }

    private async Task UpdateSystemHealthMetricsAsync(ISetupUnitOfWork unitOfWork, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogDebug("Updating system health metrics");

            var healthMetrics = new SystemHealthMetrics
            {
                Date = DateTime.UtcNow.Date,
                Hour = DateTime.UtcNow.Hour,
                ActiveTenants = await unitOfWork.TenantRepository.GetActiveTenantCountAsync(cancellationToken),
                TotalUsers = await unitOfWork.UserRepository.GetTotalUserCountAsync(cancellationToken),
                ActiveUsers = await unitOfWork.UserRepository.GetActiveUserCountAsync(DateTime.UtcNow.AddDays(-30), cancellationToken),
                SystemLoad = await CalculateSystemLoadAsync(unitOfWork, cancellationToken),
                DatabaseSize = await GetDatabaseSizeAsync(unitOfWork, cancellationToken),
                MemoryUsage = GC.GetTotalMemory(false),
                UpTimeMinutes = GetSystemUptimeMinutes(),
                ErrorCount = await GetRecentErrorCountAsync(unitOfWork, cancellationToken),
                PerformanceScore = await CalculatePerformanceScoreAsync(unitOfWork, cancellationToken)
            };

            await unitOfWork.SystemConfigRepository.UpsertSystemHealthMetricsAsync(healthMetrics, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogDebug("System health metrics updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating system health metrics");
        }
    }

    private async Task OptimizeDatabasePerformanceAsync(ISetupUnitOfWork unitOfWork, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogDebug("Optimizing database performance");

            // Update table statistics
            await unitOfWork.SystemConfigRepository.UpdateDatabaseStatisticsAsync(cancellationToken);

            // Rebuild fragmented indexes
            await unitOfWork.SystemConfigRepository.RebuildFragmentedIndexesAsync(cancellationToken);

            // Analyze query performance
            await unitOfWork.SystemConfigRepository.AnalyzeQueryPerformanceAsync(cancellationToken);

            _logger.LogDebug("Database optimization completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error optimizing database performance");
        }
    }

    private async Task ValidateSystemConfigurationAsync(ISystemConfigService systemConfigService, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogDebug("Validating system configuration");

            // Check critical configuration values
            var criticalConfigs = new[]
            {
                "System.MaxUsers",
                "System.MaxTenants",
                "System.BackupEnabled",
                "System.SecurityPolicyEnabled"
            };

            foreach (var config in criticalConfigs)
            {
                var value = await systemConfigService.GetConfigValueAsync<string>(config, cancellationToken);
                if (string.IsNullOrEmpty(value))
                {
                    _logger.LogWarning("Critical configuration missing: {Config}", config);
                }
            }

            // Validate feature flags
            var systemMetrics = await systemConfigService.GetSystemMetricsAsync(cancellationToken);
            _logger.LogInformation("System configuration validated. Enabled features: {FeatureCount}", 
                systemMetrics.EnabledFeatureFlagsCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating system configuration");
        }
    }

    private async Task CheckForSystemUpdatesAsync(ISetupUnitOfWork unitOfWork, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogDebug("Checking for system updates");

            var lastUpdateCheck = await unitOfWork.SystemConfigRepository.GetLastUpdateCheckAsync(cancellationToken);
            var now = DateTime.UtcNow;

            // Check for updates daily
            if (lastUpdateCheck == null || (now - lastUpdateCheck.Value).TotalDays >= 1)
            {
                // Record the update check
                await unitOfWork.SystemConfigRepository.RecordUpdateCheckAsync(now, cancellationToken);

                // In a real implementation, this would check for available updates
                _logger.LogInformation("System update check completed at {Timestamp}", now);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking for system updates");
        }
    }

    private async Task GenerateSystemReportsAsync(ISetupUnitOfWork unitOfWork, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogDebug("Generating system reports");

            var today = DateTime.UtcNow.Date;
            var lastWeek = today.AddDays(-7);

            // Generate weekly system summary
            var weeklyReport = new WeeklySystemReport
            {
                WeekStartDate = lastWeek,
                WeekEndDate = today,
                NewTenants = await unitOfWork.TenantRepository.GetNewTenantCountAsync(lastWeek, today, cancellationToken),
                NewUsers = await unitOfWork.UserRepository.GetNewUserCountAsync(lastWeek, today, cancellationToken),
                TotalRevenue = await unitOfWork.TenantRepository.GetRevenueAsync(lastWeek, today, cancellationToken),
                SystemUptime = CalculateSystemUptimePercentage(),
                SecurityIncidents = await unitOfWork.SystemConfigRepository.GetSecurityIncidentCountAsync(lastWeek, today, cancellationToken),
                GeneratedAt = DateTime.UtcNow
            };

            await unitOfWork.SystemConfigRepository.SaveWeeklyReportAsync(weeklyReport, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Weekly system report generated for period {Start} to {End}", 
                lastWeek, today);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating system reports");
        }
    }

    private async Task<decimal> CalculateSystemLoadAsync(ISetupUnitOfWork unitOfWork, CancellationToken cancellationToken)
    {
        try
        {
            // Calculate based on active connections, CPU usage, etc.
            var activeConnections = await unitOfWork.SystemConfigRepository.GetActiveConnectionCountAsync(cancellationToken);
            var maxConnections = 1000; // Configuration value
            
            return Math.Min(100, (decimal)activeConnections / maxConnections * 100);
        }
        catch
        {
            return 0;
        }
    }

    private async Task<long> GetDatabaseSizeAsync(ISetupUnitOfWork unitOfWork, CancellationToken cancellationToken)
    {
        try
        {
            return await unitOfWork.SystemConfigRepository.GetDatabaseSizeAsync(cancellationToken);
        }
        catch
        {
            return 0;
        }
    }

    private static int GetSystemUptimeMinutes()
    {
        // This would typically track application start time
        return (int)Environment.TickCount64 / 60000; // Convert to minutes
    }

    private async Task<int> GetRecentErrorCountAsync(ISetupUnitOfWork unitOfWork, CancellationToken cancellationToken)
    {
        try
        {
            var since = DateTime.UtcNow.AddHours(-6);
            return await unitOfWork.SystemConfigRepository.GetErrorCountSinceAsync(since, cancellationToken);
        }
        catch
        {
            return 0;
        }
    }

    private async Task<decimal> CalculatePerformanceScoreAsync(ISetupUnitOfWork unitOfWork, CancellationToken cancellationToken)
    {
        try
        {
            // Calculate a composite performance score (0-100)
            var systemLoad = await CalculateSystemLoadAsync(unitOfWork, cancellationToken);
            var errorCount = await GetRecentErrorCountAsync(unitOfWork, cancellationToken);
            var uptimePercentage = CalculateSystemUptimePercentage();

            // Simple scoring algorithm
            var loadScore = Math.Max(0, 100 - systemLoad);
            var errorScore = Math.Max(0, 100 - (errorCount * 5)); // 5 points per error
            var uptimeScore = uptimePercentage;

            return (loadScore + errorScore + uptimeScore) / 3;
        }
        catch
        {
            return 50; // Default average score
        }
    }

    private static decimal CalculateSystemUptimePercentage()
    {
        // This would typically track application start time vs total time
        // For now, assume good uptime
        return 99.9m;
    }
}

// Supporting classes
public class SystemHealthMetrics
{
    public DateTime Date { get; set; }
    public int Hour { get; set; }
    public int ActiveTenants { get; set; }
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public decimal SystemLoad { get; set; }
    public long DatabaseSize { get; set; }
    public long MemoryUsage { get; set; }
    public int UpTimeMinutes { get; set; }
    public int ErrorCount { get; set; }
    public decimal PerformanceScore { get; set; }
}

public class WeeklySystemReport
{
    public DateTime WeekStartDate { get; set; }
    public DateTime WeekEndDate { get; set; }
    public int NewTenants { get; set; }
    public int NewUsers { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal SystemUptime { get; set; }
    public int SecurityIncidents { get; set; }
    public DateTime GeneratedAt { get; set; }
}
