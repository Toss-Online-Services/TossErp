using Toss.Domain.Common;
using Toss.Domain.Entities.Businesses;

namespace Toss.Domain.Entities.Quality;

/// <summary>
/// Represents a completed run/execution of a quality checklist
/// </summary>
public class ChecklistRun : BaseAuditableEntity, IBusinessScopedEntity
{
    public ChecklistRun()
    {
        Results = new List<ChecklistRunItem>();
    }

    public int BusinessId { get; set; }
    public Business Business { get; set; } = null!;

    /// <summary>
    /// Gets or sets the quality checklist ID
    /// </summary>
    public int QualityChecklistId { get; set; }
    public QualityChecklist QualityChecklist { get; set; } = null!;

    /// <summary>
    /// Gets or sets when the checklist was run
    /// </summary>
    public DateTimeOffset RunDate { get; set; }

    /// <summary>
    /// Gets or sets the user ID who ran the checklist
    /// </summary>
    public string RunByUserId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets optional notes about this run
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets the collection of results for each checklist item
    /// </summary>
    public ICollection<ChecklistRunItem> Results { get; private set; }
}

