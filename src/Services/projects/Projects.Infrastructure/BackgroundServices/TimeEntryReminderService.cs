using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Projects.Application.Common.Interfaces;

namespace Projects.Infrastructure.BackgroundServices;

/// <summary>
/// Background service for sending automated time entry reminders to team members
/// </summary>
public class TimeEntryReminderService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<TimeEntryReminderService> _logger;
    private readonly TimeSpan _dailyReminderTime = TimeSpan.FromHours(17); // 5 PM daily reminder
    private readonly TimeSpan _weeklyReminderTime = TimeSpan.FromHours(16); // 4 PM Friday reminder

    public TimeEntryReminderService(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<TimeEntryReminderService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Time Entry Reminder Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessTimeEntryReminders(stoppingToken);
                
                // Check every hour for reminder times
                var nextCheck = TimeSpan.FromHours(1);
                _logger.LogDebug("Time entry reminder cycle completed. Next check in {Interval}", nextCheck);
                await Task.Delay(nextCheck, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Time Entry Reminder Service is stopping");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in Time Entry Reminder Service");
                
                // Wait before retrying on error
                await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
            }
        }
    }

    private async Task ProcessTimeEntryReminders(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        
        try
        {
            var now = DateTime.UtcNow;
            var currentTime = now.TimeOfDay;
            var dayOfWeek = now.DayOfWeek;

            _logger.LogDebug("Processing time entry reminders at {CurrentTime} on {DayOfWeek}", currentTime, dayOfWeek);

            // Check if it's time for daily reminder (within 1 hour window)
            var isDailyReminderTime = Math.Abs((currentTime - _dailyReminderTime).TotalMinutes) <= 60;
            
            // Check if it's time for weekly reminder (Friday, within 1 hour window)
            var isWeeklyReminderTime = dayOfWeek == DayOfWeek.Friday && 
                                      Math.Abs((currentTime - _weeklyReminderTime).TotalMinutes) <= 60;

            if (isDailyReminderTime)
            {
                await SendDailyTimeEntryReminders(cancellationToken);
            }

            if (isWeeklyReminderTime)
            {
                await SendWeeklyTimesheetReminders(cancellationToken);
            }

            // Always check for overdue time entries
            await CheckOverdueTimeEntries(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ProcessTimeEntryReminders");
            throw;
        }
    }

    private async Task SendDailyTimeEntryReminders(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        
        try
        {
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var projectRepository = scope.ServiceProvider.GetRequiredService<IProjectRepository>();
            var timeEntryRepository = scope.ServiceProvider.GetRequiredService<ITimeEntryRepository>();

            _logger.LogInformation("Sending daily time entry reminders");

            // Get all active projects
            var activeProjects = await projectRepository.GetActiveProjectsAsync();

            foreach (var project in activeProjects)
            {
                try
                {
                    // Get team members who haven't logged time today
                    var today = DateTime.UtcNow.Date;
                    var teamMembers = await projectRepository.GetProjectTeamMembersAsync(project.Id);

                    foreach (var member in teamMembers)
                    {
                        var hasLoggedTimeToday = await timeEntryRepository.HasTimeEntryForDateAsync(
                            member.UserId, project.Id, today);

                        if (!hasLoggedTimeToday)
                        {
                            await SendTimeEntryReminder(member.UserId, project.Id, "daily", 
                                $"Don't forget to log your time for today's work on {project.Name}!");

                            _logger.LogDebug("Sent daily time entry reminder to user {UserId} for project {ProjectId}", 
                                member.UserId, project.Id);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error sending daily reminders for project {ProjectId}", project.Id);
                    // Continue with other projects
                }
            }

            _logger.LogInformation("Daily time entry reminders completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in SendDailyTimeEntryReminders");
            throw;
        }
    }

    private async Task SendWeeklyTimesheetReminders(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        
        try
        {
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var projectRepository = scope.ServiceProvider.GetRequiredService<IProjectRepository>();
            var timeEntryRepository = scope.ServiceProvider.GetRequiredService<ITimeEntryRepository>();

            _logger.LogInformation("Sending weekly timesheet reminders");

            // Get start and end of current week
            var today = DateTime.UtcNow.Date;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek + 1); // Monday
            var endOfWeek = startOfWeek.AddDays(6); // Sunday

            // Get all active projects
            var activeProjects = await projectRepository.GetActiveProjectsAsync();

            foreach (var project in activeProjects)
            {
                try
                {
                    var teamMembers = await projectRepository.GetProjectTeamMembersAsync(project.Id);

                    foreach (var member in teamMembers)
                    {
                        var weeklyHours = await timeEntryRepository.GetTotalHoursForPeriodAsync(
                            member.UserId, project.Id, startOfWeek, endOfWeek);

                        // Send reminder if less than expected hours (assuming 40 hours/week)
                        var expectedWeeklyHours = 40; // This could be configurable per user/project
                        
                        if (weeklyHours < expectedWeeklyHours * 0.8) // Less than 80% of expected hours
                        {
                            await SendTimeEntryReminder(member.UserId, project.Id, "weekly", 
                                $"Weekly timesheet reminder: You've logged {weeklyHours:F1} hours this week for {project.Name}. Please review and submit your timesheet.");

                            _logger.LogDebug("Sent weekly timesheet reminder to user {UserId} for project {ProjectId} - {Hours} hours logged", 
                                member.UserId, project.Id, weeklyHours);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error sending weekly reminders for project {ProjectId}", project.Id);
                    // Continue with other projects
                }
            }

            _logger.LogInformation("Weekly timesheet reminders completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in SendWeeklyTimesheetReminders");
            throw;
        }
    }

    private async Task CheckOverdueTimeEntries(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        
        try
        {
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var timeEntryRepository = scope.ServiceProvider.GetRequiredService<ITimeEntryRepository>();
            var projectRepository = scope.ServiceProvider.GetRequiredService<IProjectRepository>();

            _logger.LogDebug("Checking for overdue time entries");

            // Get time entries that are overdue for submission (more than 3 days old and still draft)
            var overdueThreshold = DateTime.UtcNow.AddDays(-3);
            var overdueEntries = await timeEntryRepository.GetOverdueTimeEntriesAsync(overdueThreshold);

            foreach (var entry in overdueEntries)
            {
                try
                {
                    var project = await projectRepository.GetByIdAsync(entry.ProjectId);
                    if (project != null)
                    {
                        await SendTimeEntryReminder(entry.UserId, project.Id, "overdue", 
                            $"You have overdue time entries for {project.Name} from {entry.Date:MMM dd}. Please submit them for approval.");

                        _logger.LogDebug("Sent overdue time entry reminder to user {UserId} for entry {EntryId}", 
                            entry.UserId, entry.Id);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error sending overdue reminder for time entry {EntryId}", entry.Id);
                    // Continue with other entries
                }
            }

            if (overdueEntries.Any())
            {
                _logger.LogInformation("Sent {Count} overdue time entry reminders", overdueEntries.Count());
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in CheckOverdueTimeEntries");
            throw;
        }
    }

    private async Task SendTimeEntryReminder(string userId, Guid projectId, string reminderType, string message)
    {
        try
        {
            // This method would integrate with your notification system
            // For now, we'll just log the reminder
            // In a real implementation, this would:
            // 1. Send email notification
            // 2. Send in-app notification
            // 3. Send push notification (if mobile app exists)
            // 4. Potentially send Slack/Teams message

            _logger.LogInformation("Time entry reminder - Type: {ReminderType}, User: {UserId}, Project: {ProjectId}, Message: {Message}", 
                reminderType, userId, projectId, message);

            // Example: Send notification through a notification service
            // await _notificationService.SendReminderAsync(userId, reminderType, message);

            // Example: Send email through email service
            // await _emailService.SendTimeEntryReminderAsync(userId, projectId, reminderType, message);

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending time entry reminder to user {UserId}", userId);
            throw;
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Time Entry Reminder Service is stopping");
        await base.StopAsync(cancellationToken);
    }
}
