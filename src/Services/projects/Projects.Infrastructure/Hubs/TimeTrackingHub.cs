using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Projects.Application.Common.Interfaces;
using System.Security.Claims;

namespace Projects.Infrastructure.Hubs;

/// <summary>
/// SignalR Hub for real-time time tracking notifications and collaboration
/// </summary>
[Authorize]
public class TimeTrackingHub : Hub
{
    private readonly ILogger<TimeTrackingHub> _logger;
    private readonly ICurrentUserService _currentUser;

    public TimeTrackingHub(ILogger<TimeTrackingHub> logger, ICurrentUserService currentUser)
    {
        _logger = logger;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Join time tracking room for a specific project
    /// </summary>
    public async Task JoinTimeTrackingRoom(string projectId)
    {
        try
        {
            var userId = _currentUser.UserId;
            var tenantId = _currentUser.TenantId;
            var userName = Context.User?.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown User";

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(tenantId))
            {
                await Clients.Caller.SendAsync("Error", "User not authenticated");
                return;
            }

            var roomName = $"timetracking_{tenantId}_{projectId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);

            _logger.LogInformation("User {UserId} joined time tracking room {RoomName} with connection {ConnectionId}",
                userId, roomName, Context.ConnectionId);

            await Clients.Caller.SendAsync("JoinedTimeTrackingRoom", new { projectId, roomName });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error joining time tracking room for project {ProjectId}", projectId);
            await Clients.Caller.SendAsync("Error", "Failed to join time tracking room");
        }
    }

    /// <summary>
    /// Leave time tracking room
    /// </summary>
    public async Task LeaveTimeTrackingRoom(string projectId)
    {
        try
        {
            var userId = _currentUser.UserId;
            var tenantId = _currentUser.TenantId;

            var roomName = $"timetracking_{tenantId}_{projectId}";
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);

            _logger.LogInformation("User {UserId} left time tracking room {RoomName} with connection {ConnectionId}",
                userId, roomName, Context.ConnectionId);

            await Clients.Caller.SendAsync("LeftTimeTrackingRoom", new { projectId, roomName });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error leaving time tracking room for project {ProjectId}", projectId);
            await Clients.Caller.SendAsync("Error", "Failed to leave time tracking room");
        }
    }

    /// <summary>
    /// Notify when a user starts time tracking
    /// </summary>
    public async Task NotifyTimeTrackingStarted(string projectId, string taskId, string taskTitle, string timeEntryId)
    {
        try
        {
            var userId = _currentUser.UserId;
            var userName = Context.User?.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown User";
            var tenantId = _currentUser.TenantId;

            var notification = new
            {
                Type = "TimeTrackingStarted",
                ProjectId = projectId,
                TaskId = taskId,
                TaskTitle = taskTitle,
                TimeEntryId = timeEntryId,
                UserId = userId,
                UserName = userName,
                StartTime = DateTime.UtcNow,
                Timestamp = DateTime.UtcNow
            };

            var roomName = $"timetracking_{tenantId}_{projectId}";
            await Clients.OthersInGroup(roomName).SendAsync("TimeTrackingStarted", notification);

            // Send to project managers
            var managerRoomName = $"managers_{tenantId}_{projectId}";
            await Clients.Group(managerRoomName).SendAsync("TeamMemberStartedTracking", notification);

            _logger.LogInformation("Time tracking started notification sent for task {TaskId} by user {UserId}",
                taskId, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error notifying time tracking started for task {TaskId}", taskId);
            await Clients.Caller.SendAsync("Error", "Failed to notify time tracking started");
        }
    }

    /// <summary>
    /// Notify when a user stops time tracking
    /// </summary>
    public async Task NotifyTimeTrackingStopped(string projectId, string taskId, string taskTitle, string timeEntryId, double hoursTracked)
    {
        try
        {
            var userId = _currentUser.UserId;
            var userName = Context.User?.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown User";
            var tenantId = _currentUser.TenantId;

            var notification = new
            {
                Type = "TimeTrackingStopped",
                ProjectId = projectId,
                TaskId = taskId,
                TaskTitle = taskTitle,
                TimeEntryId = timeEntryId,
                UserId = userId,
                UserName = userName,
                HoursTracked = hoursTracked,
                StopTime = DateTime.UtcNow,
                Timestamp = DateTime.UtcNow
            };

            var roomName = $"timetracking_{tenantId}_{projectId}";
            await Clients.OthersInGroup(roomName).SendAsync("TimeTrackingStopped", notification);

            // Send to project managers
            var managerRoomName = $"managers_{tenantId}_{projectId}";
            await Clients.Group(managerRoomName).SendAsync("TeamMemberStoppedTracking", notification);

            _logger.LogInformation("Time tracking stopped notification sent for task {TaskId} by user {UserId} - {Hours} hours",
                taskId, userId, hoursTracked);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error notifying time tracking stopped for task {TaskId}", taskId);
            await Clients.Caller.SendAsync("Error", "Failed to notify time tracking stopped");
        }
    }

    /// <summary>
    /// Send time entry submission notification
    /// </summary>
    public async Task NotifyTimeEntrySubmitted(string projectId, string timeEntryId, double hours, string description, bool isBillable)
    {
        try
        {
            var userId = _currentUser.UserId;
            var userName = Context.User?.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown User";
            var tenantId = _currentUser.TenantId;

            var notification = new
            {
                Type = "TimeEntrySubmitted",
                ProjectId = projectId,
                TimeEntryId = timeEntryId,
                Hours = hours,
                Description = description,
                IsBillable = isBillable,
                SubmittedBy = userName,
                SubmittedById = userId,
                Timestamp = DateTime.UtcNow
            };

            // Send to project managers for approval
            var managerRoomName = $"managers_{tenantId}_{projectId}";
            await Clients.Group(managerRoomName).SendAsync("TimeEntrySubmittedForApproval", notification);

            _logger.LogInformation("Time entry submission notification sent for entry {TimeEntryId} by user {UserId}",
                timeEntryId, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error notifying time entry submitted for entry {TimeEntryId}", timeEntryId);
            await Clients.Caller.SendAsync("Error", "Failed to notify time entry submitted");
        }
    }

    /// <summary>
    /// Send time entry approval/rejection notification
    /// </summary>
    public async Task NotifyTimeEntryStatusChanged(string projectId, string timeEntryId, string status, string? comments, string submitterId)
    {
        try
        {
            var userId = _currentUser.UserId;
            var userName = Context.User?.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown User";
            var tenantId = _currentUser.TenantId;

            var notification = new
            {
                Type = "TimeEntryStatusChanged",
                ProjectId = projectId,
                TimeEntryId = timeEntryId,
                Status = status,
                Comments = comments,
                ReviewedBy = userName,
                ReviewedById = userId,
                SubmitterId = submitterId,
                Timestamp = DateTime.UtcNow
            };

            // Send to the user who submitted the time entry
            var userGroupName = $"user_{tenantId}_{submitterId}";
            await Clients.Group(userGroupName).SendAsync("TimeEntryStatusChanged", notification);

            // Also send to the time tracking room
            var roomName = $"timetracking_{tenantId}_{projectId}";
            await Clients.Group(roomName).SendAsync("TimeEntryStatusUpdate", notification);

            _logger.LogInformation("Time entry status change notification sent for entry {TimeEntryId} - Status: {Status}",
                timeEntryId, status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error notifying time entry status changed for entry {TimeEntryId}", timeEntryId);
            await Clients.Caller.SendAsync("Error", "Failed to notify time entry status change");
        }
    }

    /// <summary>
    /// Send daily/weekly timesheet reminder
    /// </summary>
    public async Task SendTimesheetReminder(string projectId, string reminderType, string message)
    {
        try
        {
            var tenantId = _currentUser.TenantId;

            var reminder = new
            {
                Type = "TimesheetReminder",
                ProjectId = projectId,
                ReminderType = reminderType, // "daily" or "weekly"
                Message = message,
                Timestamp = DateTime.UtcNow
            };

            var roomName = $"timetracking_{tenantId}_{projectId}";
            await Clients.Group(roomName).SendAsync("TimesheetReminder", reminder);

            _logger.LogInformation("Timesheet reminder sent to room {RoomName} - Type: {ReminderType}",
                roomName, reminderType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending timesheet reminder for project {ProjectId}", projectId);
            await Clients.Caller.SendAsync("Error", "Failed to send timesheet reminder");
        }
    }

    /// <summary>
    /// Get current time tracking status for a project
    /// </summary>
    public async Task GetTimeTrackingStatus(string projectId)
    {
        try
        {
            var userId = _currentUser.UserId;
            var tenantId = _currentUser.TenantId;

            // This would typically query the database for current active time entries
            // For now, returning a basic status structure
            var status = new
            {
                ProjectId = projectId,
                UserId = userId,
                IsCurrentlyTracking = false, // Would be determined from database
                ActiveTimeEntryId = (string?)null,
                ActiveTaskId = (string?)null,
                TrackingStartTime = (DateTime?)null,
                TodayHours = 0.0, // Would be calculated from database
                WeekHours = 0.0, // Would be calculated from database
                Timestamp = DateTime.UtcNow
            };

            await Clients.Caller.SendAsync("TimeTrackingStatus", status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting time tracking status for project {ProjectId}", projectId);
            await Clients.Caller.SendAsync("Error", "Failed to get time tracking status");
        }
    }

    /// <summary>
    /// Join project manager room for supervisory notifications
    /// </summary>
    public async Task JoinManagerRoom(string projectId)
    {
        try
        {
            var userId = _currentUser.UserId;
            var tenantId = _currentUser.TenantId;

            // Check if user has manager role (this would typically check roles from the token)
            var userRoles = Context.User?.FindAll(ClaimTypes.Role)?.Select(c => c.Value) ?? [];
            
            if (!userRoles.Any(r => r.Equals("Admin", StringComparison.OrdinalIgnoreCase) || 
                                  r.Equals("ProjectManager", StringComparison.OrdinalIgnoreCase) ||
                                  r.Equals("TeamLead", StringComparison.OrdinalIgnoreCase)))
            {
                await Clients.Caller.SendAsync("Error", "Insufficient permissions to join manager room");
                return;
            }

            var managerRoomName = $"managers_{tenantId}_{projectId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, managerRoomName);

            _logger.LogInformation("Manager {UserId} joined manager room {RoomName} with connection {ConnectionId}",
                userId, managerRoomName, Context.ConnectionId);

            await Clients.Caller.SendAsync("JoinedManagerRoom", new { projectId, roomName = managerRoomName });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error joining manager room for project {ProjectId}", projectId);
            await Clients.Caller.SendAsync("Error", "Failed to join manager room");
        }
    }

    public override async Task OnConnectedAsync()
    {
        try
        {
            var userId = _currentUser.UserId;
            var userName = Context.User?.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown User";
            var tenantId = _currentUser.TenantId;

            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(tenantId))
            {
                // Join personal user group for time tracking notifications
                var userGroupName = $"user_{tenantId}_{userId}";
                await Groups.AddToGroupAsync(Context.ConnectionId, userGroupName);

                _logger.LogInformation("User {UserId} ({UserName}) connected to TimeTrackingHub with connection {ConnectionId}",
                    userId, userName, Context.ConnectionId);

                await Clients.Caller.SendAsync("Connected", new
                {
                    message = "Connected to time tracking",
                    connectionId = Context.ConnectionId,
                    timestamp = DateTime.UtcNow
                });
            }

            await base.OnConnectedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during TimeTrackingHub connection");
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        try
        {
            var userId = _currentUser.UserId;

            if (exception != null)
            {
                _logger.LogWarning(exception, "User {UserId} disconnected from TimeTrackingHub with error", userId);
            }
            else
            {
                _logger.LogInformation("User {UserId} disconnected from TimeTrackingHub", userId);
            }

            await base.OnDisconnectedAsync(exception);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during TimeTrackingHub disconnection");
        }
    }
}
