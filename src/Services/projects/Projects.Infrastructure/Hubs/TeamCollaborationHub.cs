using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Projects.Application.Common.Interfaces;
using System.Security.Claims;

namespace Projects.Infrastructure.Hubs;

/// <summary>
/// SignalR Hub for team collaboration features like chat, file sharing, and real-time document editing
/// </summary>
[Authorize]
public class TeamCollaborationHub : Hub
{
    private readonly ILogger<TeamCollaborationHub> _logger;
    private readonly ICurrentUserService _currentUser;

    public TeamCollaborationHub(ILogger<TeamCollaborationHub> logger, ICurrentUserService currentUser)
    {
        _logger = logger;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Join a team collaboration room for a specific project
    /// </summary>
    public async Task JoinTeamRoom(string projectId)
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

            var roomName = $"team_{tenantId}_{projectId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);

            _logger.LogInformation("User {UserId} joined team room {RoomName} with connection {ConnectionId}",
                userId, roomName, Context.ConnectionId);

            // Notify other team members
            await Clients.OthersInGroup(roomName).SendAsync("UserJoined", new
            {
                UserId = userId,
                UserName = userName,
                ProjectId = projectId,
                Timestamp = DateTime.UtcNow
            });

            await Clients.Caller.SendAsync("JoinedTeamRoom", new { projectId, roomName });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error joining team room for project {ProjectId}", projectId);
            await Clients.Caller.SendAsync("Error", "Failed to join team room");
        }
    }

    /// <summary>
    /// Leave a team collaboration room
    /// </summary>
    public async Task LeaveTeamRoom(string projectId)
    {
        try
        {
            var userId = _currentUser.UserId;
            var tenantId = _currentUser.TenantId;
            var userName = Context.User?.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown User";

            var roomName = $"team_{tenantId}_{projectId}";
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);

            _logger.LogInformation("User {UserId} left team room {RoomName} with connection {ConnectionId}",
                userId, roomName, Context.ConnectionId);

            // Notify other team members
            await Clients.Group(roomName).SendAsync("UserLeft", new
            {
                UserId = userId,
                UserName = userName,
                ProjectId = projectId,
                Timestamp = DateTime.UtcNow
            });

            await Clients.Caller.SendAsync("LeftTeamRoom", new { projectId, roomName });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error leaving team room for project {ProjectId}", projectId);
            await Clients.Caller.SendAsync("Error", "Failed to leave team room");
        }
    }

    /// <summary>
    /// Send a team chat message
    /// </summary>
    public async Task SendTeamMessage(string projectId, string message, string? replyToMessageId = null)
    {
        try
        {
            var userId = _currentUser.UserId;
            var userName = Context.User?.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown User";
            var tenantId = _currentUser.TenantId;

            if (string.IsNullOrWhiteSpace(message))
            {
                await Clients.Caller.SendAsync("Error", "Message cannot be empty");
                return;
            }

            var chatMessage = new
            {
                Id = Guid.NewGuid().ToString(),
                ProjectId = projectId,
                Message = message.Trim(),
                SenderId = userId,
                SenderName = userName,
                ReplyToMessageId = replyToMessageId,
                Timestamp = DateTime.UtcNow,
                Type = "text"
            };

            var roomName = $"team_{tenantId}_{projectId}";
            await Clients.Group(roomName).SendAsync("TeamMessageReceived", chatMessage);

            _logger.LogInformation("Team message sent to room {RoomName} by user {UserId}",
                roomName, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending team message for project {ProjectId}", projectId);
            await Clients.Caller.SendAsync("Error", "Failed to send message");
        }
    }

    /// <summary>
    /// Send typing indicator
    /// </summary>
    public async Task SendTypingIndicator(string projectId, bool isTyping)
    {
        try
        {
            var userId = _currentUser.UserId;
            var userName = Context.User?.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown User";
            var tenantId = _currentUser.TenantId;

            var roomName = $"team_{tenantId}_{projectId}";
            await Clients.OthersInGroup(roomName).SendAsync("TypingIndicator", new
            {
                UserId = userId,
                UserName = userName,
                ProjectId = projectId,
                IsTyping = isTyping,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending typing indicator for project {ProjectId}", projectId);
        }
    }

    /// <summary>
    /// Share file information with the team
    /// </summary>
    public async Task ShareFile(string projectId, string fileName, string fileUrl, long fileSize, string fileType)
    {
        try
        {
            var userId = _currentUser.UserId;
            var userName = Context.User?.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown User";
            var tenantId = _currentUser.TenantId;

            var fileShare = new
            {
                Id = Guid.NewGuid().ToString(),
                ProjectId = projectId,
                FileName = fileName,
                FileUrl = fileUrl,
                FileSize = fileSize,
                FileType = fileType,
                SharedBy = userName,
                SharedById = userId,
                Timestamp = DateTime.UtcNow,
                Type = "file"
            };

            var roomName = $"team_{tenantId}_{projectId}";
            await Clients.Group(roomName).SendAsync("FileShared", fileShare);

            _logger.LogInformation("File {FileName} shared in room {RoomName} by user {UserId}",
                fileName, roomName, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sharing file for project {ProjectId}", projectId);
            await Clients.Caller.SendAsync("Error", "Failed to share file");
        }
    }

    /// <summary>
    /// Start collaborative document editing session
    /// </summary>
    public async Task StartDocumentSession(string projectId, string documentId, string documentName)
    {
        try
        {
            var userId = _currentUser.UserId;
            var userName = Context.User?.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown User";
            var tenantId = _currentUser.TenantId;

            var sessionInfo = new
            {
                DocumentId = documentId,
                DocumentName = documentName,
                ProjectId = projectId,
                StartedBy = userName,
                StartedById = userId,
                Timestamp = DateTime.UtcNow
            };

            var roomName = $"team_{tenantId}_{projectId}";
            await Clients.OthersInGroup(roomName).SendAsync("DocumentSessionStarted", sessionInfo);

            // Add user to document-specific group
            var docGroupName = $"doc_{tenantId}_{documentId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, docGroupName);

            await Clients.Caller.SendAsync("JoinedDocumentSession", new { documentId, docGroupName });

            _logger.LogInformation("Document session started for {DocumentId} by user {UserId}",
                documentId, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting document session for document {DocumentId}", documentId);
            await Clients.Caller.SendAsync("Error", "Failed to start document session");
        }
    }

    /// <summary>
    /// Send document change for collaborative editing
    /// </summary>
    public async Task SendDocumentChange(string documentId, object changeData)
    {
        try
        {
            var userId = _currentUser.UserId;
            var tenantId = _currentUser.TenantId;

            var change = new
            {
                DocumentId = documentId,
                UserId = userId,
                ChangeData = changeData,
                Timestamp = DateTime.UtcNow
            };

            var docGroupName = $"doc_{tenantId}_{documentId}";
            await Clients.OthersInGroup(docGroupName).SendAsync("DocumentChanged", change);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending document change for document {DocumentId}", documentId);
            await Clients.Caller.SendAsync("Error", "Failed to send document change");
        }
    }

    /// <summary>
    /// End collaborative document editing session
    /// </summary>
    public async Task EndDocumentSession(string documentId)
    {
        try
        {
            var userId = _currentUser.UserId;
            var tenantId = _currentUser.TenantId;

            var docGroupName = $"doc_{tenantId}_{documentId}";
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, docGroupName);

            await Clients.Group(docGroupName).SendAsync("DocumentSessionEnded", new
            {
                DocumentId = documentId,
                EndedBy = userId,
                Timestamp = DateTime.UtcNow
            });

            await Clients.Caller.SendAsync("LeftDocumentSession", new { documentId });

            _logger.LogInformation("Document session ended for {DocumentId} by user {UserId}",
                documentId, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ending document session for document {DocumentId}", documentId);
            await Clients.Caller.SendAsync("Error", "Failed to end document session");
        }
    }

    /// <summary>
    /// Send team announcement
    /// </summary>
    public async Task SendTeamAnnouncement(string projectId, string title, string message, string priority = "normal")
    {
        try
        {
            var userId = _currentUser.UserId;
            var userName = Context.User?.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown User";
            var tenantId = _currentUser.TenantId;

            var announcement = new
            {
                Id = Guid.NewGuid().ToString(),
                ProjectId = projectId,
                Title = title,
                Message = message,
                Priority = priority,
                AnnouncedBy = userName,
                AnnouncedById = userId,
                Timestamp = DateTime.UtcNow,
                Type = "announcement"
            };

            var roomName = $"team_{tenantId}_{projectId}";
            await Clients.Group(roomName).SendAsync("TeamAnnouncement", announcement);

            _logger.LogInformation("Team announcement sent to room {RoomName} by user {UserId}",
                roomName, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending team announcement for project {ProjectId}", projectId);
            await Clients.Caller.SendAsync("Error", "Failed to send announcement");
        }
    }

    public override async Task OnConnectedAsync()
    {
        try
        {
            var userId = _currentUser.UserId;
            var userName = Context.User?.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown User";

            _logger.LogInformation("User {UserId} ({UserName}) connected to TeamCollaborationHub with connection {ConnectionId}",
                userId, userName, Context.ConnectionId);

            await Clients.Caller.SendAsync("Connected", new
            {
                message = "Connected to team collaboration",
                connectionId = Context.ConnectionId,
                timestamp = DateTime.UtcNow
            });

            await base.OnConnectedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during TeamCollaborationHub connection");
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        try
        {
            var userId = _currentUser.UserId;

            if (exception != null)
            {
                _logger.LogWarning(exception, "User {UserId} disconnected from TeamCollaborationHub with error", userId);
            }
            else
            {
                _logger.LogInformation("User {UserId} disconnected from TeamCollaborationHub", userId);
            }

            await base.OnDisconnectedAsync(exception);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during TeamCollaborationHub disconnection");
        }
    }
}
