using Toss.Domain.Common;
using Toss.Domain.Entities.Businesses;

namespace Toss.Domain.Entities.Quality;

/// <summary>
/// Represents the result of a specific checklist item in a checklist run
/// </summary>
public class ChecklistRunItem : BaseAuditableEntity, IBusinessScopedEntity
{
    public ChecklistRunItem()
    {
        Notes = string.Empty;
    }

    public int BusinessId { get; set; }
    public Business Business { get; set; } = null!;

    /// <summary>
    /// Gets or sets the checklist run ID
    /// </summary>
    public int ChecklistRunId { get; set; }
    public ChecklistRun ChecklistRun { get; set; } = null!;

    /// <summary>
    /// Gets or sets the checklist item ID
    /// </summary>
    public int ChecklistItemId { get; set; }
    public ChecklistItem ChecklistItem { get; set; } = null!;

    /// <summary>
    /// Gets or sets whether this item passed
    /// </summary>
    public bool Passed { get; set; }

    /// <summary>
    /// Gets or sets optional notes about this item result
    /// </summary>
    public string? Notes { get; set; }
}

