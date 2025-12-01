using Toss.Domain.Common;
using Toss.Domain.Entities.Businesses;
using Toss.Domain.Enums;

namespace Toss.Domain.Entities.Quality;

/// <summary>
/// Represents a quality incident
/// </summary>
public class Incident : BaseAuditableEntity, IBusinessScopedEntity
{
    public Incident()
    {
        Title = string.Empty;
        ActionItems = new List<ActionItem>();
    }

    public int BusinessId { get; set; }
    public Business Business { get; set; } = null!;

    /// <summary>
    /// Gets or sets the incident title
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the incident description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the incident type
    /// </summary>
    public IncidentType Type { get; set; }

    /// <summary>
    /// Gets or sets the incident severity
    /// </summary>
    public IncidentSeverity Severity { get; set; }

    /// <summary>
    /// Gets or sets when the incident occurred
    /// </summary>
    public DateTimeOffset OccurredAt { get; set; }

    /// <summary>
    /// Gets or sets the quality checklist ID if this incident is related to a checklist run (optional)
    /// </summary>
    public int? QualityChecklistId { get; set; }
    public QualityChecklist? QualityChecklist { get; set; }

    /// <summary>
    /// Gets or sets the checklist item ID if this incident is related to a specific checklist item (optional)
    /// </summary>
    public int? ChecklistItemId { get; set; }
    public ChecklistItem? ChecklistItem { get; set; }

    /// <summary>
    /// Gets or sets optional notes
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets the collection of action items for this incident
    /// </summary>
    public ICollection<ActionItem> ActionItems { get; private set; }
}

