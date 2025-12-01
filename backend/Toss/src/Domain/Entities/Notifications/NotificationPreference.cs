using Toss.Domain.Common;
using Toss.Domain.Entities.Businesses;
using Toss.Domain.Enums;

namespace Toss.Domain.Entities.Notifications;

/// <summary>
/// Represents user notification preferences
/// </summary>
public class NotificationPreference : BaseAuditableEntity, IBusinessScopedEntity
{
    public int BusinessId { get; set; }
    public Business Business { get; set; } = null!;

    /// <summary>
    /// Gets or sets the user ID
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the notification type
    /// </summary>
    public NotificationType NotificationType { get; set; }

    /// <summary>
    /// Gets or sets whether this notification type is enabled
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to send email for this notification type
    /// </summary>
    public bool SendEmail { get; set; } = false;

    /// <summary>
    /// Gets or sets whether to send SMS for this notification type
    /// </summary>
    public bool SendSms { get; set; } = false;
}

