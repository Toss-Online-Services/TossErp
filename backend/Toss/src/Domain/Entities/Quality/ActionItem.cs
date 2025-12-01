using Toss.Domain.Common;
using Toss.Domain.Entities.Businesses;
using Toss.Domain.Enums;

namespace Toss.Domain.Entities.Quality;

/// <summary>
/// Represents an action item assigned to resolve an incident or follow up on a quality issue
/// </summary>
public class ActionItem : BaseAuditableEntity, IBusinessScopedEntity
{
    public ActionItem()
    {
        Title = string.Empty;
        Status = ActionItemStatus.Open;
    }

    public int BusinessId { get; set; }
    public Business Business { get; set; } = null!;

    /// <summary>
    /// Gets or sets the incident ID this action item is related to
    /// </summary>
    public int? IncidentId { get; set; }
    public Incident? Incident { get; set; }

    /// <summary>
    /// Gets or sets the action item title
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the action item description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the user ID of the assignee
    /// </summary>
    public string? AssignedToId { get; set; }

    /// <summary>
    /// Gets or sets the due date
    /// </summary>
    public DateTimeOffset? DueDate { get; set; }

    /// <summary>
    /// Gets or sets the action item status
    /// </summary>
    public ActionItemStatus Status { get; set; }

    /// <summary>
    /// Gets or sets when the action item was completed
    /// </summary>
    public DateTimeOffset? CompletedAt { get; set; }

    /// <summary>
    /// Gets or sets optional notes
    /// </summary>
    public string? Notes { get; set; }
}

