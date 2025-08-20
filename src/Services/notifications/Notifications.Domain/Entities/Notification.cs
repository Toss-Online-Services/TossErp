namespace Notifications.Domain.Entities;

public class Notification : Entity
{
    public string Title { get; private set; }
    public string Message { get; private set; }
    public NotificationType Type { get; private set; }
    public NotificationPriority Priority { get; private set; }
    public NotificationStatus Status { get; private set; }
    public string RecipientId { get; private set; }
    public string RecipientEmail { get; private set; }
    public string RecipientPhone { get; private set; }
    public NotificationChannel Channel { get; private set; }
    public string TemplateId { get; private set; }
    public Dictionary<string, object> TemplateVariables { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ScheduledAt { get; private set; }
    public DateTime? SentAt { get; private set; }
    public DateTime? DeliveredAt { get; private set; }
    public int RetryCount { get; private set; }
    public int MaxRetries { get; private set; }
    public string? ErrorMessage { get; private set; }
    public bool RequiresConsent { get; private set; }
    public bool ConsentGiven { get; private set; }
    public string? AuditLogId { get; private set; }

    public Notification(
        string title,
        string message,
        NotificationType type,
        NotificationPriority priority,
        string recipientId,
        string recipientEmail,
        string recipientPhone,
        NotificationChannel channel,
        string templateId,
        Dictionary<string, object> templateVariables,
        DateTime? scheduledAt = null,
        bool requiresConsent = false)
    {
        Id = Guid.NewGuid();
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Message = message ?? throw new ArgumentNullException(nameof(message));
        Type = type;
        Priority = priority;
        Status = NotificationStatus.Pending;
        RecipientId = recipientId ?? throw new ArgumentNullException(nameof(recipientId));
        RecipientEmail = recipientEmail ?? string.Empty;
        RecipientPhone = recipientPhone ?? string.Empty;
        Channel = channel;
        TemplateId = templateId ?? string.Empty;
        TemplateVariables = templateVariables ?? new Dictionary<string, object>();
        CreatedAt = DateTime.UtcNow;
        ScheduledAt = scheduledAt;
        RetryCount = 0;
        MaxRetries = 3;
        RequiresConsent = requiresConsent;
        ConsentGiven = !requiresConsent; // If consent not required, assume given
    }

    private Notification() 
    {
        TemplateVariables = new Dictionary<string, object>();
    }

    public void MarkAsSent()
    {
        Status = NotificationStatus.Sent;
        SentAt = DateTime.UtcNow;
    }

    public void MarkAsDelivered()
    {
        Status = NotificationStatus.Delivered;
        DeliveredAt = DateTime.UtcNow;
    }

    public void MarkAsFailed(string errorMessage)
    {
        Status = NotificationStatus.Failed;
        ErrorMessage = errorMessage;
    }

    public void IncrementRetryCount()
    {
        RetryCount++;
        if (RetryCount >= MaxRetries)
        {
            Status = NotificationStatus.Failed;
            ErrorMessage = "Max retries exceeded";
        }
        else
        {
            Status = NotificationStatus.Pending;
        }
    }

    public void GiveConsent()
    {
        ConsentGiven = true;
    }

    public void RevokeConsent()
    {
        ConsentGiven = false;
    }

    public bool CanBeSent => Status == NotificationStatus.Pending && 
                            ConsentGiven && 
                            (ScheduledAt == null || ScheduledAt <= DateTime.UtcNow);

    public bool IsExpired => Status == NotificationStatus.Pending && 
                            CreatedAt.AddDays(7) < DateTime.UtcNow;
}

public enum NotificationType
{
    Alert,
    Promotion,
    Report,
    Reminder,
    Welcome,
    PasswordReset,
    OrderConfirmation,
    StockAlert,
    SystemMaintenance
}

public enum NotificationPriority
{
    Low,
    Normal,
    High,
    Urgent
}

public enum NotificationStatus
{
    Pending,
    Sent,
    Delivered,
    Failed,
    Cancelled
}

public enum NotificationChannel
{
    Email,
    SMS,
    Push,
    InApp,
    WhatsApp
}
