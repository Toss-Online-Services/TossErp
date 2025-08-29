using TossErp.Shared.SeedWork;

namespace Notifications.Domain.Entities;

public class NotificationAuditLog : Entity
{
    public override Guid Id { get; protected set; }
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; }
    public Guid NotificationId { get; private set; }
    public string RecipientId { get; private set; }
    public NotificationChannel Channel { get; private set; }
    public NotificationType Type { get; private set; }
    public AuditAction Action { get; private set; }
    public string Description { get; private set; }
    public string? UserId { get; private set; }
    public string? UserEmail { get; private set; }
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }
    public DateTime Timestamp { get; private set; }
    public Dictionary<string, object> Metadata { get; private set; }
    public bool IsConsentRelated { get; private set; }
    public ConsentAction? ConsentAction { get; private set; }
    public string? ConsentReason { get; private set; }

    public NotificationAuditLog(
        Guid notificationId,
        string recipientId,
        NotificationChannel channel,
        NotificationType type,
        AuditAction action,
        string description,
        string? userId = null,
        string? userEmail = null,
        string? ipAddress = null,
        string? userAgent = null,
        Dictionary<string, object>? metadata = null,
        bool isConsentRelated = false,
        ConsentAction? consentAction = null,
        string? consentReason = null)
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        CreatedBy = userId ?? "system";
        NotificationId = notificationId;
        RecipientId = recipientId ?? throw new ArgumentNullException(nameof(recipientId));
        Channel = channel;
        Type = type;
        Action = action;
        Description = description ?? throw new ArgumentNullException(nameof(description));
        UserId = userId;
        UserEmail = userEmail;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        Timestamp = DateTime.UtcNow;
        Metadata = metadata ?? new Dictionary<string, object>();
        IsConsentRelated = isConsentRelated;
        ConsentAction = consentAction;
        ConsentReason = consentReason;
    }

    private NotificationAuditLog()
    {
        CreatedBy = string.Empty;
        RecipientId = string.Empty;
        Description = string.Empty;
        Metadata = new Dictionary<string, object>();
    }

    public void AddMetadata(string key, object value)
    {
        Metadata[key] = value;
    }

    public void RemoveMetadata(string key)
    {
        if (Metadata.ContainsKey(key))
        {
            Metadata.Remove(key);
        }
    }
}

public enum AuditAction
{
    Created,
    Sent,
    Delivered,
    Failed,
    Retried,
    Cancelled,
    ConsentGiven,
    ConsentRevoked,
    ConsentUpdated,
    Viewed,
    Clicked,
    Unsubscribed,
    Bounced,
    SpamReported
}

public enum ConsentAction
{
    Given,
    Revoked,
    Updated,
    Withdrawn
}
