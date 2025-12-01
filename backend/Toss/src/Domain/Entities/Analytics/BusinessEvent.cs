using Toss.Domain.Common;
using Toss.Domain.Entities.Businesses;
using Toss.Domain.Enums;

namespace Toss.Domain.Entities.Analytics;

/// <summary>
/// Represents a business event for analytics and metrics tracking
/// </summary>
public class BusinessEvent : BaseAuditableEntity, IBusinessScopedEntity
{
    public BusinessEvent()
    {
        EventType = BusinessEventType.Other;
        EventData = string.Empty;
    }

    public int BusinessId { get; set; }
    public Business Business { get; set; } = null!;

    /// <summary>
    /// Gets or sets the type of event
    /// </summary>
    public BusinessEventType EventType { get; set; }

    /// <summary>
    /// Gets or sets when the event occurred
    /// </summary>
    public DateTimeOffset OccurredAt { get; set; }

    /// <summary>
    /// Gets or sets the user ID who triggered the event (if applicable)
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Gets or sets additional event data as JSON
    /// </summary>
    public string EventData { get; set; }

    /// <summary>
    /// Gets or sets the module/feature where the event occurred (e.g., "POS", "Stock", "Sales")
    /// </summary>
    public string? Module { get; set; }
}

