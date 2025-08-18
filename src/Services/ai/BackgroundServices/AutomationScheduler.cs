using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TossErp.AI.Services;

namespace TossErp.AI.BackgroundServices;

/// <summary>
/// Background service that schedules and executes recurring automations
/// </summary>
public class AutomationScheduler : BackgroundService
{
    private readonly ILogger<AutomationScheduler> _logger;
    private readonly IServiceAutomationEngine _automationEngine;
    private readonly Dictionary<string, Timer> _scheduledJobs = new();

    public AutomationScheduler(
        ILogger<AutomationScheduler> logger,
        IServiceAutomationEngine automationEngine)
    {
        _logger = logger;
        _automationEngine = automationEngine;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Automation Scheduler started");

        // Load existing scheduled automations
        await LoadScheduledAutomationsAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        _logger.LogInformation("Automation Scheduler stopped");
    }

    private async Task LoadScheduledAutomationsAsync(CancellationToken cancellationToken)
    {
        // This would load scheduled automations from the database
        // For now, we'll create some default scheduled automations
        
        var defaultSchedules = new[]
        {
            new { Service = "inventory", Action = "daily_stock_check", Schedule = "0 8 * * *" }, // Daily at 8 AM
            new { Service = "sales", Action = "customer_follow_up", Schedule = "0 9 * * 1" }, // Weekly on Monday at 9 AM
            new { Service = "finance", Action = "monthly_report", Schedule = "0 10 1 * *" }, // Monthly on 1st at 10 AM
            new { Service = "purchasing", Action = "supplier_evaluation", Schedule = "0 11 * * 1" } // Weekly on Monday at 11 AM
        };

        foreach (var schedule in defaultSchedules)
        {
            await ScheduleAutomationAsync(schedule.Service, schedule.Action, schedule.Schedule, cancellationToken);
        }
    }

    private async Task ScheduleAutomationAsync(string service, string action, string cronExpression, CancellationToken cancellationToken)
    {
        var jobId = $"{service}_{action}";
        
        if (_scheduledJobs.ContainsKey(jobId))
        {
            _scheduledJobs[jobId].Dispose();
            _scheduledJobs.Remove(jobId);
        }

        var timer = new Timer(async _ => await ExecuteScheduledAutomationAsync(service, action), null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
        _scheduledJobs[jobId] = timer;

        _logger.LogInformation("Scheduled automation: {Service}.{Action} with schedule {Schedule}", service, action, cronExpression);
    }

    private async Task ExecuteScheduledAutomationAsync(string service, string action)
    {
        try
        {
            _logger.LogDebug("Executing scheduled automation: {Service}.{Action}", service, action);

            var automationRequest = new AutomationRequest
            {
                Service = service,
                Action = action,
                IsRecurring = true,
                Parameters = new Dictionary<string, object>
                {
                    ["scheduled_execution"] = true,
                    ["execution_time"] = DateTime.UtcNow
                }
            };

            var result = await _automationEngine.ExecuteAutomationAsync(automationRequest);
            
            if (result.Success)
            {
                _logger.LogInformation("Successfully executed scheduled automation: {Service}.{Action}", service, action);
            }
            else
            {
                _logger.LogWarning("Failed to execute scheduled automation: {Service}.{Action}: {Message}", service, action, result.Message);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing scheduled automation: {Service}.{Action}", service, action);
        }
    }
}

