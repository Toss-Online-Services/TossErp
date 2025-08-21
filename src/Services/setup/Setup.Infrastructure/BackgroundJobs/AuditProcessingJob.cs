using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Setup.Application.Common.Interfaces;

namespace Setup.Infrastructure.BackgroundJobs;

public class AuditProcessingJob : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AuditProcessingJob> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(5); // Run every 5 minutes

    public AuditProcessingJob(IServiceProvider serviceProvider, ILogger<AuditProcessingJob> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Audit Processing Job started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessAuditEntriesAsync(stoppingToken);
                await Task.Delay(_interval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Audit Processing Job is stopping");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Audit Processing Job");
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Wait 1 minute before retrying
            }
        }

        _logger.LogInformation("Audit Processing Job stopped");
    }

    private async Task ProcessAuditEntriesAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();

        try
        {
            _logger.LogDebug("Processing unprocessed audit entries");

            // Process unprocessed audit entries
            await auditService.ProcessUnprocessedAuditEntriesAsync(cancellationToken);

            // Archive old audit logs (older than 1 year)
            var archiveCutoff = DateTime.UtcNow.AddYears(-1);
            await auditService.ArchiveOldAuditLogsAsync(archiveCutoff, cancellationToken);

            // Purge very old archived logs (older than 7 years)
            var purgeCutoff = DateTime.UtcNow.AddYears(-7);
            await auditService.PurgeArchivedAuditLogsAsync(purgeCutoff, cancellationToken);

            _logger.LogDebug("Audit processing completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing audit entries");
        }
    }
}
