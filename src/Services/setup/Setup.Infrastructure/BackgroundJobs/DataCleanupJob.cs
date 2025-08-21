using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Setup.Application.Common.Interfaces;

namespace Setup.Infrastructure.BackgroundJobs;

public class DataCleanupJob : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DataCleanupJob> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromHours(24); // Run daily

    public DataCleanupJob(IServiceProvider serviceProvider, ILogger<DataCleanupJob> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Data Cleanup Job started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await PerformDataCleanupAsync(stoppingToken);
                await Task.Delay(_interval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Data Cleanup Job is stopping");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Data Cleanup Job");
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken); // Wait 1 hour before retrying
            }
        }

        _logger.LogInformation("Data Cleanup Job stopped");
    }

    private async Task PerformDataCleanupAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<ISetupUnitOfWork>();

        try
        {
            _logger.LogInformation("Starting data cleanup operations");

            // Clean up expired notifications
            await notificationService.CleanupExpiredNotificationsAsync(cancellationToken);

            // Clean up old session data
            await CleanupOldSessionsAsync(unitOfWork, cancellationToken);

            // Clean up expired temporary files
            await CleanupTemporaryFilesAsync(unitOfWork, cancellationToken);

            // Clean up old email logs
            await CleanupOldEmailLogsAsync(unitOfWork, cancellationToken);

            // Clean up resolved quota warnings
            await CleanupResolvedQuotaWarningsAsync(unitOfWork, cancellationToken);

            // Clean up old user tokens
            await CleanupExpiredUserTokensAsync(unitOfWork, cancellationToken);

            // Clean up old backup files
            await CleanupOldBackupFilesAsync(unitOfWork, cancellationToken);

            // Update cleanup metrics
            await UpdateCleanupMetricsAsync(unitOfWork, cancellationToken);

            _logger.LogInformation("Data cleanup operations completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing data cleanup");
        }
    }

    private async Task CleanupOldSessionsAsync(ISetupUnitOfWork unitOfWork, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogDebug("Cleaning up old user sessions");

            var cutoffDate = DateTime.UtcNow.AddDays(-30); // Remove sessions older than 30 days
            var deletedCount = await unitOfWork.SystemConfigRepository.CleanupOldSessionsAsync(cutoffDate, cancellationToken);
            
            if (deletedCount > 0)
            {
                await unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Cleaned up {Count} old user sessions", deletedCount);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up old sessions");
        }
    }

    private async Task CleanupTemporaryFilesAsync(ISetupUnitOfWork unitOfWork, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogDebug("Cleaning up temporary files");

            var cutoffDate = DateTime.UtcNow.AddDays(-7); // Remove temp files older than 7 days
            var deletedCount = await unitOfWork.SystemConfigRepository.CleanupTemporaryFilesAsync(cutoffDate, cancellationToken);
            
            if (deletedCount > 0)
            {
                await unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Cleaned up {Count} temporary files", deletedCount);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up temporary files");
        }
    }

    private async Task CleanupOldEmailLogsAsync(ISetupUnitOfWork unitOfWork, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogDebug("Cleaning up old email logs");

            var cutoffDate = DateTime.UtcNow.AddDays(-90); // Remove email logs older than 90 days
            var deletedCount = await unitOfWork.SystemConfigRepository.CleanupOldEmailLogsAsync(cutoffDate, cancellationToken);
            
            if (deletedCount > 0)
            {
                await unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Cleaned up {Count} old email logs", deletedCount);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up old email logs");
        }
    }

    private async Task CleanupResolvedQuotaWarningsAsync(ISetupUnitOfWork unitOfWork, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogDebug("Cleaning up resolved quota warnings");

            var cutoffDate = DateTime.UtcNow.AddDays(-30); // Remove resolved warnings older than 30 days
            var deletedCount = await unitOfWork.SystemConfigRepository.CleanupResolvedQuotaWarningsAsync(cutoffDate, cancellationToken);
            
            if (deletedCount > 0)
            {
                await unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Cleaned up {Count} resolved quota warnings", deletedCount);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up resolved quota warnings");
        }
    }

    private async Task CleanupExpiredUserTokensAsync(ISetupUnitOfWork unitOfWork, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogDebug("Cleaning up expired user tokens");

            var cutoffDate = DateTime.UtcNow; // Remove tokens that have already expired
            var deletedCount = await unitOfWork.SystemConfigRepository.CleanupExpiredUserTokensAsync(cutoffDate, cancellationToken);
            
            if (deletedCount > 0)
            {
                await unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Cleaned up {Count} expired user tokens", deletedCount);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up expired user tokens");
        }
    }

    private async Task CleanupOldBackupFilesAsync(ISetupUnitOfWork unitOfWork, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogDebug("Cleaning up old backup files");

            // This will respect the backup retention policies defined in backup configurations
            var deletedCount = await unitOfWork.SystemConfigRepository.CleanupOldBackupFilesAsync(cancellationToken);
            
            if (deletedCount > 0)
            {
                await unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Cleaned up {Count} old backup files", deletedCount);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up old backup files");
        }
    }

    private async Task UpdateCleanupMetricsAsync(ISetupUnitOfWork unitOfWork, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogDebug("Updating cleanup metrics");

            var metrics = new CleanupMetrics
            {
                Date = DateTime.UtcNow.Date,
                SessionsCleanedUp = await GetLastCleanupCount("sessions", unitOfWork, cancellationToken),
                TemporaryFilesCleanedUp = await GetLastCleanupCount("temp_files", unitOfWork, cancellationToken),
                EmailLogsCleanedUp = await GetLastCleanupCount("email_logs", unitOfWork, cancellationToken),
                QuotaWarningsCleanedUp = await GetLastCleanupCount("quota_warnings", unitOfWork, cancellationToken),
                UserTokensCleanedUp = await GetLastCleanupCount("user_tokens", unitOfWork, cancellationToken),
                BackupFilesCleanedUp = await GetLastCleanupCount("backup_files", unitOfWork, cancellationToken),
                TotalItemsCleanedUp = 0 // Will be calculated
            };

            metrics.TotalItemsCleanedUp = metrics.SessionsCleanedUp + metrics.TemporaryFilesCleanedUp + 
                                        metrics.EmailLogsCleanedUp + metrics.QuotaWarningsCleanedUp + 
                                        metrics.UserTokensCleanedUp + metrics.BackupFilesCleanedUp;

            await unitOfWork.SystemConfigRepository.UpsertCleanupMetricsAsync(metrics, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Updated cleanup metrics: {TotalItems} items cleaned up", metrics.TotalItemsCleanedUp);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating cleanup metrics");
        }
    }

    private async Task<int> GetLastCleanupCount(string category, ISetupUnitOfWork unitOfWork, CancellationToken cancellationToken)
    {
        try
        {
            return await unitOfWork.SystemConfigRepository.GetLastCleanupCountAsync(category, cancellationToken);
        }
        catch
        {
            return 0;
        }
    }
}

// Supporting class
public class CleanupMetrics
{
    public DateTime Date { get; set; }
    public int SessionsCleanedUp { get; set; }
    public int TemporaryFilesCleanedUp { get; set; }
    public int EmailLogsCleanedUp { get; set; }
    public int QuotaWarningsCleanedUp { get; set; }
    public int UserTokensCleanedUp { get; set; }
    public int BackupFilesCleanedUp { get; set; }
    public int TotalItemsCleanedUp { get; set; }
}
