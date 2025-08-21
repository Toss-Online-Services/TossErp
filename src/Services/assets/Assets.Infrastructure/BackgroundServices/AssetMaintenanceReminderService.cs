namespace TossErp.Assets.Infrastructure.BackgroundServices;

/// <summary>
/// Background service for sending asset maintenance reminders
/// </summary>
public class AssetMaintenanceReminderService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AssetMaintenanceReminderService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(6); // Check every 6 hours

    public AssetMaintenanceReminderService(
        IServiceProvider serviceProvider,
        ILogger<AssetMaintenanceReminderService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Asset Maintenance Reminder Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessMaintenanceReminders(stoppingToken);
                await Task.Delay(_checkInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Expected when cancellation is requested
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing maintenance reminders");
                
                // Wait a bit before retrying to avoid tight error loops
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        _logger.LogInformation("Asset Maintenance Reminder Service stopped");
    }

    private async Task ProcessMaintenanceReminders(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AssetsDbContext>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        // Get all tenants with maintenance schedules due
        var upcomingMaintenance = await context.MaintenanceSchedules
            .Include(m => m.Asset)
            .Where(m => m.IsActive && 
                       m.NextMaintenanceDate <= DateTime.UtcNow.AddDays(7) && // 7 days advance notice
                       !m.ReminderSent)
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Found {Count} maintenance schedules requiring reminders", upcomingMaintenance.Count);

        foreach (var schedule in upcomingMaintenance)
        {
            try
            {
                // Create maintenance reminder event
                var reminderEvent = new MaintenanceReminderEvent
                {
                    TenantId = schedule.TenantId,
                    AssetId = schedule.AssetId,
                    AssetName = schedule.Asset.Name,
                    AssetTag = schedule.Asset.AssetTag,
                    MaintenanceType = schedule.MaintenanceType,
                    ScheduledDate = schedule.NextMaintenanceDate,
                    DaysUntilDue = (schedule.NextMaintenanceDate - DateTime.UtcNow).Days,
                    Description = schedule.Description,
                    Priority = schedule.Priority,
                    EstimatedCost = schedule.EstimatedCost
                };

                await mediator.Publish(reminderEvent, cancellationToken);

                // Mark reminder as sent
                schedule.ReminderSent = true;
                schedule.LastReminderDate = DateTime.UtcNow;

                _logger.LogInformation("Sent maintenance reminder for asset {AssetTag} ({AssetName})", 
                    schedule.Asset.AssetTag, schedule.Asset.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send maintenance reminder for asset {AssetId}", schedule.AssetId);
            }
        }

        if (upcomingMaintenance.Any())
        {
            await context.SaveChangesAsync(cancellationToken);
        }

        // Also check for overdue maintenance
        var overdueMaintenance = await context.MaintenanceSchedules
            .Include(m => m.Asset)
            .Where(m => m.IsActive && 
                       m.NextMaintenanceDate < DateTime.UtcNow &&
                       !m.OverdueNotificationSent)
            .ToListAsync(cancellationToken);

        foreach (var schedule in overdueMaintenance)
        {
            try
            {
                var overdueEvent = new MaintenanceOverdueEvent
                {
                    TenantId = schedule.TenantId,
                    AssetId = schedule.AssetId,
                    AssetName = schedule.Asset.Name,
                    AssetTag = schedule.Asset.AssetTag,
                    MaintenanceType = schedule.MaintenanceType,
                    ScheduledDate = schedule.NextMaintenanceDate,
                    DaysOverdue = (DateTime.UtcNow - schedule.NextMaintenanceDate).Days,
                    Description = schedule.Description,
                    Priority = schedule.Priority
                };

                await mediator.Publish(overdueEvent, cancellationToken);

                // Mark overdue notification as sent
                schedule.OverdueNotificationSent = true;
                schedule.LastOverdueNotificationDate = DateTime.UtcNow;

                _logger.LogWarning("Sent overdue maintenance notification for asset {AssetTag} ({AssetName})", 
                    schedule.Asset.AssetTag, schedule.Asset.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send overdue maintenance notification for asset {AssetId}", schedule.AssetId);
            }
        }

        if (overdueMaintenance.Any())
        {
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}

/// <summary>
/// Event published when maintenance reminder is sent
/// </summary>
public class MaintenanceReminderEvent : INotification
{
    public Guid TenantId { get; set; }
    public Guid AssetId { get; set; }
    public string AssetName { get; set; } = string.Empty;
    public string AssetTag { get; set; } = string.Empty;
    public string MaintenanceType { get; set; } = string.Empty;
    public DateTime ScheduledDate { get; set; }
    public int DaysUntilDue { get; set; }
    public string? Description { get; set; }
    public string Priority { get; set; } = string.Empty;
    public decimal? EstimatedCost { get; set; }
}

/// <summary>
/// Event published when maintenance is overdue
/// </summary>
public class MaintenanceOverdueEvent : INotification
{
    public Guid TenantId { get; set; }
    public Guid AssetId { get; set; }
    public string AssetName { get; set; } = string.Empty;
    public string AssetTag { get; set; } = string.Empty;
    public string MaintenanceType { get; set; } = string.Empty;
    public DateTime ScheduledDate { get; set; }
    public int DaysOverdue { get; set; }
    public string? Description { get; set; }
    public string Priority { get; set; } = string.Empty;
}
