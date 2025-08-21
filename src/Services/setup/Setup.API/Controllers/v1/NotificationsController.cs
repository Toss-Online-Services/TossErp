using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Setup.Application.Common.Interfaces;
using Setup.Application.Features.Notifications.Commands.CreateNotification;
using Setup.Application.Features.Notifications.Commands.UpdateNotification;
using Setup.Application.Features.Notifications.Queries.GetNotification;
using Setup.Application.Features.Notifications.Queries.GetNotifications;
using MediatR;

namespace Setup.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly INotificationService _notificationService;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(IMediator mediator, INotificationService notificationService, ILogger<NotificationsController> logger)
    {
        _mediator = mediator;
        _notificationService = notificationService;
        _logger = logger;
    }

    /// <summary>
    /// Get notifications with optional filtering
    /// </summary>
    /// <param name="tenantId">Filter by tenant ID</param>
    /// <param name="userId">Filter by user ID</param>
    /// <param name="type">Filter by notification type</param>
    /// <param name="isRead">Filter by read status</param>
    /// <param name="priority">Filter by priority level</param>
    /// <param name="fromDate">Start date for notifications</param>
    /// <param name="toDate">End date for notifications</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 20, max: 100)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of notifications</returns>
    [HttpGet]
    [ProducesResponseType(typeof(GetNotificationsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<GetNotificationsResponse>> GetNotifications(
        [FromQuery] string? tenantId = null,
        [FromQuery] string? userId = null,
        [FromQuery] string? type = null,
        [FromQuery] bool? isRead = null,
        [FromQuery] string? priority = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        try
        {
            pageSize = Math.Min(pageSize, 100); // Limit page size

            var query = new GetNotificationsQuery
            {
                TenantId = tenantId,
                UserId = userId,
                Type = type,
                IsRead = isRead,
                Priority = priority,
                FromDate = fromDate,
                ToDate = toDate,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving notifications");
            return BadRequest(new { Message = "Error retrieving notifications", Details = ex.Message });
        }
    }

    /// <summary>
    /// Get a specific notification by ID
    /// </summary>
    /// <param name="id">Notification ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Notification details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetNotificationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<GetNotificationResponse>> GetNotification(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetNotificationQuery { Id = id };
            var result = await _mediator.Send(query, cancellationToken);

            if (result == null)
                return NotFound(new { Message = $"Notification with ID {id} not found" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving notification {NotificationId}", id);
            return BadRequest(new { Message = "Error retrieving notification", Details = ex.Message });
        }
    }

    /// <summary>
    /// Create a new notification
    /// </summary>
    /// <param name="command">Notification creation details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created notification details</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CreateNotificationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CreateNotificationResponse>> CreateNotification(
        CreateNotificationCommand command, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetNotification), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating notification");
            return BadRequest(new { Message = "Error creating notification", Details = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing notification
    /// </summary>
    /// <param name="id">Notification ID</param>
    /// <param name="command">Notification update details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated notification details</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(UpdateNotificationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UpdateNotificationResponse>> UpdateNotification(
        string id, 
        UpdateNotificationCommand command, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (id != command.Id)
                return BadRequest(new { Message = "Notification ID in URL does not match command" });

            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating notification {NotificationId}", id);
            return BadRequest(new { Message = "Error updating notification", Details = ex.Message });
        }
    }

    /// <summary>
    /// Mark a notification as read
    /// </summary>
    /// <param name="id">Notification ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpPost("{id}/read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> MarkAsRead(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var success = await _notificationService.MarkAsReadAsync(id, cancellationToken);
            
            if (!success)
                return NotFound(new { Message = $"Notification with ID {id} not found" });

            return Ok(new { Message = "Notification marked as read" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking notification as read {NotificationId}", id);
            return BadRequest(new { Message = "Error marking notification as read", Details = ex.Message });
        }
    }

    /// <summary>
    /// Mark a notification as unread
    /// </summary>
    /// <param name="id">Notification ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpPost("{id}/unread")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> MarkAsUnread(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var success = await _notificationService.MarkAsUnreadAsync(id, cancellationToken);
            
            if (!success)
                return NotFound(new { Message = $"Notification with ID {id} not found" });

            return Ok(new { Message = "Notification marked as unread" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking notification as unread {NotificationId}", id);
            return BadRequest(new { Message = "Error marking notification as unread", Details = ex.Message });
        }
    }

    /// <summary>
    /// Mark multiple notifications as read
    /// </summary>
    /// <param name="request">Notification IDs to mark as read</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpPost("mark-read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> MarkMultipleAsRead(
        [FromBody] MarkMultipleNotificationsRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _notificationService.MarkMultipleAsReadAsync(request.NotificationIds, cancellationToken);
            
            return Ok(new { 
                Message = "Notifications processed", 
                SuccessCount = result.SuccessCount,
                FailureCount = result.FailureCount
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking multiple notifications as read");
            return BadRequest(new { Message = "Error marking notifications as read", Details = ex.Message });
        }
    }

    /// <summary>
    /// Delete a notification
    /// </summary>
    /// <param name="id">Notification ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteNotification(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var success = await _notificationService.DeleteNotificationAsync(id, cancellationToken);
            
            if (!success)
                return NotFound(new { Message = $"Notification with ID {id} not found" });

            return Ok(new { Message = "Notification deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting notification {NotificationId}", id);
            return BadRequest(new { Message = "Error deleting notification", Details = ex.Message });
        }
    }

    /// <summary>
    /// Delete multiple notifications
    /// </summary>
    /// <param name="request">Notification IDs to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteMultipleNotifications(
        [FromBody] DeleteMultipleNotificationsRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _notificationService.DeleteMultipleNotificationsAsync(request.NotificationIds, cancellationToken);
            
            return Ok(new { 
                Message = "Notifications processed", 
                SuccessCount = result.SuccessCount,
                FailureCount = result.FailureCount
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting multiple notifications");
            return BadRequest(new { Message = "Error deleting notifications", Details = ex.Message });
        }
    }

    /// <summary>
    /// Send a notification
    /// </summary>
    /// <param name="request">Send notification request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpPost("send")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SendNotification(
        [FromBody] SendNotificationRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var success = await _notificationService.SendNotificationAsync(
                request.TenantId,
                request.UserId,
                request.Title,
                request.Message,
                request.Type,
                request.Priority,
                request.Data,
                cancellationToken);

            if (success)
                return Ok(new { Message = "Notification sent successfully" });

            return BadRequest(new { Message = "Failed to send notification" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification");
            return BadRequest(new { Message = "Error sending notification", Details = ex.Message });
        }
    }

    /// <summary>
    /// Get notification statistics
    /// </summary>
    /// <param name="tenantId">Filter by tenant ID</param>
    /// <param name="userId">Filter by user ID</param>
    /// <param name="fromDate">Start date for statistics</param>
    /// <param name="toDate">End date for statistics</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Notification statistics</returns>
    [HttpGet("statistics")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetNotificationStatistics(
        [FromQuery] string? tenantId = null,
        [FromQuery] string? userId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var statistics = await _notificationService.GetNotificationStatisticsAsync(
                tenantId, userId, fromDate, toDate, cancellationToken);
            
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving notification statistics");
            return BadRequest(new { Message = "Error retrieving notification statistics", Details = ex.Message });
        }
    }

    /// <summary>
    /// Get user notification preferences
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User notification preferences</returns>
    [HttpGet("preferences/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUserPreferences(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var preferences = await _notificationService.GetUserPreferencesAsync(userId, cancellationToken);
            
            if (preferences == null)
                return NotFound(new { Message = $"User preferences for user {userId} not found" });

            return Ok(preferences);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user notification preferences {UserId}", userId);
            return BadRequest(new { Message = "Error retrieving user preferences", Details = ex.Message });
        }
    }

    /// <summary>
    /// Update user notification preferences
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="preferences">Notification preferences</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpPut("preferences/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateUserPreferences(
        string userId,
        [FromBody] UpdateNotificationPreferencesRequest preferences,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var success = await _notificationService.UpdateUserPreferencesAsync(userId, preferences, cancellationToken);
            
            if (success)
                return Ok(new { Message = "User preferences updated successfully" });

            return BadRequest(new { Message = "Failed to update user preferences" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user notification preferences {UserId}", userId);
            return BadRequest(new { Message = "Error updating user preferences", Details = ex.Message });
        }
    }
}

// Request models
public class MarkMultipleNotificationsRequest
{
    public List<string> NotificationIds { get; set; } = new();
}

public class DeleteMultipleNotificationsRequest
{
    public List<string> NotificationIds { get; set; } = new();
}

public class SendNotificationRequest
{
    public string TenantId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Priority { get; set; } = "Medium";
    public Dictionary<string, object>? Data { get; set; }
}

public class UpdateNotificationPreferencesRequest
{
    public bool EmailNotifications { get; set; }
    public bool SmsNotifications { get; set; }
    public bool PushNotifications { get; set; }
    public bool InAppNotifications { get; set; }
    public List<string> NotificationTypes { get; set; } = new();
    public Dictionary<string, object>? Settings { get; set; }
}
