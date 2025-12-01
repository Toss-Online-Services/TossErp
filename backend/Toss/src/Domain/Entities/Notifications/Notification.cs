using Toss.Domain.Common;
using Toss.Domain.Entities.Businesses;
using Toss.Domain.Enums;

namespace Toss.Domain.Entities.Notifications;

/// <summary>
/// Represents an in-app notification
/// </summary>
public class Notification : BaseAuditableEntity, IBusinessScopedEntity
{
    public Notification()
    {
        Title = string.Empty;
        Message = string.Empty;
        Type = NotificationType.Info;
        Status = NotificationStatus.Unread;
    }

    public int BusinessId { get; set; }
    public Business Business { get; set; } = null!;

    /// <summary>
    /// Gets or sets the user ID who should receive this notification
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the notification title
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the notification message
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Gets or sets the notification type
    /// </summary>
    public NotificationType Type { get; set; }

    /// <summary>
    /// Gets or sets the notification status
    /// </summary>
    public NotificationStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the type of entity this notification is linked to (e.g., "Sale", "PurchaseOrder")
    /// </summary>
    public string? LinkedType { get; set; }

    /// <summary>
    /// Gets or sets the ID of the linked entity
    /// </summary>
    public int? LinkedId { get; set; }

    /// <summary>
    /// Gets or sets the URL to navigate to when notification is clicked
    /// </summary>
    public string? ActionUrl { get; set; }

    /// <summary>
    /// Gets or sets when the notification was read
    /// </summary>
    public DateTimeOffset? ReadAt { get; set; }
}

