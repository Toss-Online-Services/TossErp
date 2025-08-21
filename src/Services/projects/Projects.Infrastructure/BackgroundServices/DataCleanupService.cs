using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Projects.Application.Common.Interfaces;

namespace Projects.Infrastructure.BackgroundServices;

/// <summary>
/// Background service for periodic data cleanup operations
/// </summary>
public class DataCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<DataCleanupService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromHours(24); // Run daily at midnight

    public DataCleanupService(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<DataCleanupService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Data Cleanup Service started");

        // Wait until midnight for the first run
        await WaitUntilMidnight(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await PerformDataCleanup(stoppingToken);
                
                _logger.LogDebug("Data cleanup cycle completed. Next run in {Interval}", _interval);
                await Task.Delay(_interval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Data Cleanup Service is stopping");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in Data Cleanup Service");
                
                // Wait before retrying on error
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }

    private async Task WaitUntilMidnight(CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var nextMidnight = now.Date.AddDays(1);
        var timeToWait = nextMidnight - now;

        if (timeToWait.TotalMilliseconds > 0)
        {
            _logger.LogInformation("Waiting {TimeToWait} until first cleanup at midnight", timeToWait);
            await Task.Delay(timeToWait, cancellationToken);
        }
    }

    private async Task PerformDataCleanup(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        
        try
        {
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            
            _logger.LogInformation("Starting daily data cleanup operations");

            // Cleanup old audit logs (older than 1 year)
            await CleanupOldAuditLogs(scope, cancellationToken);

            // Cleanup orphaned files (files not referenced by any project or task)
            await CleanupOrphanedFiles(scope, cancellationToken);

            // Cleanup old notifications (older than 3 months)
            await CleanupOldNotifications(scope, cancellationToken);

            // Cleanup draft time entries (older than 30 days)
            await CleanupOldDraftTimeEntries(scope, cancellationToken);

            // Cleanup expired project invitations (older than 7 days)
            await CleanupExpiredInvitations(scope, cancellationToken);

            // Archive completed projects (completed more than 1 year ago)
            await ArchiveOldCompletedProjects(scope, cancellationToken);

            // Save all cleanup changes
            await unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Daily data cleanup operations completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in PerformDataCleanup");
            throw;
        }
    }

    private async Task CleanupOldAuditLogs(IServiceScope scope, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogDebug("Cleaning up old audit logs");

            // This would integrate with your audit log repository
            // For now, we'll just log the operation
            var cutoffDate = DateTime.UtcNow.AddYears(-1);
            
            _logger.LogInformation("Would cleanup audit logs older than {CutoffDate}", cutoffDate);

            // Example implementation:
            // var auditRepository = scope.ServiceProvider.GetRequiredService<IAuditRepository>();
            // var deletedCount = await auditRepository.DeleteOldLogsAsync(cutoffDate, cancellationToken);
            // _logger.LogInformation("Deleted {Count} old audit log entries", deletedCount);

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up old audit logs");
            throw;
        }
    }

    private async Task CleanupOrphanedFiles(IServiceScope scope, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogDebug("Cleaning up orphaned files");

            // This would integrate with your file storage and document repositories
            var cutoffDate = DateTime.UtcNow.AddDays(-7); // Files not referenced for 7 days

            _logger.LogInformation("Would cleanup orphaned files older than {CutoffDate}", cutoffDate);

            // Example implementation:
            // var documentRepository = scope.ServiceProvider.GetRequiredService<IDocumentRepository>();
            // var fileService = scope.ServiceProvider.GetRequiredService<IFileService>();
            // 
            // var orphanedFiles = await documentRepository.GetOrphanedFilesAsync(cutoffDate);
            // foreach (var file in orphanedFiles)
            // {
            //     await fileService.DeleteFileAsync(file.StoragePath);
            //     await documentRepository.DeleteAsync(file);
            // }
            // _logger.LogInformation("Deleted {Count} orphaned files", orphanedFiles.Count);

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up orphaned files");
            throw;
        }
    }

    private async Task CleanupOldNotifications(IServiceScope scope, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogDebug("Cleaning up old notifications");

            var cutoffDate = DateTime.UtcNow.AddMonths(-3);
            
            _logger.LogInformation("Would cleanup notifications older than {CutoffDate}", cutoffDate);

            // Example implementation:
            // var notificationRepository = scope.ServiceProvider.GetRequiredService<INotificationRepository>();
            // var deletedCount = await notificationRepository.DeleteOldNotificationsAsync(cutoffDate, cancellationToken);
            // _logger.LogInformation("Deleted {Count} old notifications", deletedCount);

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up old notifications");
            throw;
        }
    }

    private async Task CleanupOldDraftTimeEntries(IServiceScope scope, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogDebug("Cleaning up old draft time entries");

            var timeEntryRepository = scope.ServiceProvider.GetRequiredService<ITimeEntryRepository>();
            var cutoffDate = DateTime.UtcNow.AddDays(-30);

            var deletedCount = await timeEntryRepository.DeleteOldDraftEntriesAsync(cutoffDate, cancellationToken);
            
            if (deletedCount > 0)
            {
                _logger.LogInformation("Deleted {Count} old draft time entries", deletedCount);
            }
            else
            {
                _logger.LogDebug("No old draft time entries found for cleanup");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up old draft time entries");
            throw;
        }
    }

    private async Task CleanupExpiredInvitations(IServiceScope scope, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogDebug("Cleaning up expired project invitations");

            var cutoffDate = DateTime.UtcNow.AddDays(-7);
            
            _logger.LogInformation("Would cleanup expired invitations older than {CutoffDate}", cutoffDate);

            // Example implementation:
            // var invitationRepository = scope.ServiceProvider.GetRequiredService<IProjectInvitationRepository>();
            // var deletedCount = await invitationRepository.DeleteExpiredInvitationsAsync(cutoffDate, cancellationToken);
            // _logger.LogInformation("Deleted {Count} expired project invitations", deletedCount);

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up expired invitations");
            throw;
        }
    }

    private async Task ArchiveOldCompletedProjects(IServiceScope scope, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogDebug("Archiving old completed projects");

            var projectRepository = scope.ServiceProvider.GetRequiredService<IProjectRepository>();
            var cutoffDate = DateTime.UtcNow.AddYears(-1);

            var projectsToArchive = await projectRepository.GetCompletedProjectsOlderThanAsync(cutoffDate);

            foreach (var project in projectsToArchive)
            {
                if (!project.IsArchived)
                {
                    project.Archive();
                    await projectRepository.UpdateAsync(project);
                    
                    _logger.LogInformation("Archived completed project {ProjectId} ({ProjectName}) completed on {CompletedDate}", 
                        project.Id, project.Name, project.LastModifiedAt);
                }
            }

            if (projectsToArchive.Any())
            {
                _logger.LogInformation("Archived {Count} old completed projects", projectsToArchive.Count());
            }
            else
            {
                _logger.LogDebug("No old completed projects found for archiving");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error archiving old completed projects");
            throw;
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Data Cleanup Service is stopping");
        await base.StopAsync(cancellationToken);
    }
}
