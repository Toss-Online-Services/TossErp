using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Projects.Application.Common.Interfaces;
using System.Security.Claims;

namespace Projects.Infrastructure.Hubs;

/// <summary>
/// SignalR Hub for real-time project updates and notifications
/// </summary>
[Authorize]
public class ProjectUpdatesHub : Hub
{
    private readonly ILogger<ProjectUpdatesHub> _logger;
    private readonly ICurrentUserService _currentUser;

    public ProjectUpdatesHub(ILogger<ProjectUpdatesHub> logger, ICurrentUserService currentUser)
    {
        _logger = logger;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Join a project group to receive project-specific updates
    /// </summary>
    public async Task JoinProjectGroup(string projectId)
    {
        try
        {
            var userId = _currentUser.UserId;
            var tenantId = _currentUser.TenantId;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(tenantId))
            {
                await Clients.Caller.SendAsync("Error", "User not authenticated");
                return;
            }

            var groupName = $"project_{tenantId}_{projectId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            _logger.LogInformation("User {UserId} joined project group {GroupName} with connection {ConnectionId}",
                userId, groupName, Context.ConnectionId);

            await Clients.Caller.SendAsync("JoinedProjectGroup", new { projectId, groupName });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error joining project group {ProjectId}", projectId);
            await Clients.Caller.SendAsync("Error", "Failed to join project group");
        }
    }

    /// <summary>
    /// Leave a project group
    /// </summary>
    public async Task LeaveProjectGroup(string projectId)
    {
        try
        {
            var tenantId = _currentUser.TenantId;
            var groupName = $"project_{tenantId}_{projectId}";
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            _logger.LogInformation("User {UserId} left project group {GroupName} with connection {ConnectionId}",
                _currentUser.UserId, groupName, Context.ConnectionId);

            await Clients.Caller.SendAsync("LeftProjectGroup", new { projectId, groupName });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error leaving project group {ProjectId}", projectId);
            await Clients.Caller.SendAsync("Error", "Failed to leave project group");
        }
    }

    /// <summary>
    /// Send a project status update to all members of the project
    /// </summary>
    public async Task SendProjectStatusUpdate(string projectId, string status, string message)
    {
        try
        {
            var userId = _currentUser.UserId;
            var userName = Context.User?.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown User";
            var tenantId = _currentUser.TenantId;

            var update = new
            {
                ProjectId = projectId,
                Status = status,
                Message = message,
                UpdatedBy = userName,
                UpdatedById = userId,
                Timestamp = DateTime.UtcNow
            };

            var groupName = $"project_{tenantId}_{projectId}";
            await Clients.Group(groupName).SendAsync("ProjectStatusUpdated", update);

            _logger.LogInformation("Project status update sent to group {GroupName} by user {UserId}",
                groupName, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending project status update for project {ProjectId}", projectId);
            await Clients.Caller.SendAsync("Error", "Failed to send project status update");
        }
    }

    /// <summary>
    /// Send a task assignment notification
    /// </summary>
    public async Task SendTaskAssignment(string projectId, string taskId, string assigneeId, string taskTitle)
    {
        try
        {
            var userId = _currentUser.UserId;
            var userName = Context.User?.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown User";
            var tenantId = _currentUser.TenantId;

            var notification = new
            {
                Type = "TaskAssigned",
                ProjectId = projectId,
                TaskId = taskId,
                TaskTitle = taskTitle,
                AssigneeId = assigneeId,
                AssignedBy = userName,
                AssignedById = userId,
                Timestamp = DateTime.UtcNow
            };

            // Send to project group
            var groupName = $"project_{tenantId}_{projectId}";
            await Clients.Group(groupName).SendAsync("TaskAssigned", notification);

            // Send to specific assignee if they have a personal connection
            var userGroupName = $"user_{tenantId}_{assigneeId}";
            await Clients.Group(userGroupName).SendAsync("PersonalNotification", notification);

            _logger.LogInformation("Task assignment notification sent for task {TaskId} to user {AssigneeId}",
                taskId, assigneeId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending task assignment notification for task {TaskId}", taskId);
            await Clients.Caller.SendAsync("Error", "Failed to send task assignment notification");
        }
    }

    /// <summary>
    /// Send milestone completion notification
    /// </summary>
    public async Task SendMilestoneCompletion(string projectId, string milestoneId, string milestoneTitle)
    {
        try
        {
            var userId = _currentUser.UserId;
            var userName = Context.User?.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown User";
            var tenantId = _currentUser.TenantId;

            var notification = new
            {
                Type = "MilestoneCompleted",
                ProjectId = projectId,
                MilestoneId = milestoneId,
                MilestoneTitle = milestoneTitle,
                CompletedBy = userName,
                CompletedById = userId,
                Timestamp = DateTime.UtcNow
            };

            var groupName = $"project_{tenantId}_{projectId}";
            await Clients.Group(groupName).SendAsync("MilestoneCompleted", notification);

            _logger.LogInformation("Milestone completion notification sent for milestone {MilestoneId}",
                milestoneId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending milestone completion notification for milestone {MilestoneId}", milestoneId);
            await Clients.Caller.SendAsync("Error", "Failed to send milestone completion notification");
        }
    }

    /// <summary>
    /// Get connection information
    /// </summary>
    public async Task GetConnectionInfo()
    {
        try
        {
            var connectionInfo = new
            {
                ConnectionId = Context.ConnectionId,
                UserId = _currentUser.UserId,
                TenantId = _currentUser.TenantId,
                UserName = Context.User?.FindFirst(ClaimTypes.Name)?.Value,
                ConnectedAt = DateTime.UtcNow,
                Groups = new List<string>() // SignalR doesn't expose group membership directly
            };

            await Clients.Caller.SendAsync("ConnectionInfo", connectionInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting connection information");
            await Clients.Caller.SendAsync("Error", "Failed to get connection information");
        }
    }

    public override async Task OnConnectedAsync()
    {
        try
        {
            var userId = _currentUser.UserId;
            var tenantId = _currentUser.TenantId;

            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(tenantId))
            {
                // Join personal user group for direct notifications
                var userGroupName = $"user_{tenantId}_{userId}";
                await Groups.AddToGroupAsync(Context.ConnectionId, userGroupName);

                _logger.LogInformation("User {UserId} connected to ProjectUpdatesHub with connection {ConnectionId}",
                    userId, Context.ConnectionId);

                await Clients.Caller.SendAsync("Connected", new
                {
                    message = "Connected to project updates",
                    connectionId = Context.ConnectionId,
                    timestamp = DateTime.UtcNow
                });
            }

            await base.OnConnectedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during ProjectUpdatesHub connection");
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        try
        {
            var userId = _currentUser.UserId;

            if (exception != null)
            {
                _logger.LogWarning(exception, "User {UserId} disconnected from ProjectUpdatesHub with error", userId);
            }
            else
            {
                _logger.LogInformation("User {UserId} disconnected from ProjectUpdatesHub", userId);
            }

            await base.OnDisconnectedAsync(exception);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during ProjectUpdatesHub disconnection");
        }
    }
}
