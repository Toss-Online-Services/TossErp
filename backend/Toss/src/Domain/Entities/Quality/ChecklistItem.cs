using Toss.Domain.Common;
using Toss.Domain.Entities.Businesses;

namespace Toss.Domain.Entities.Quality;

/// <summary>
/// Represents an item within a quality checklist
/// </summary>
public class ChecklistItem : BaseAuditableEntity, IBusinessScopedEntity
{
    public ChecklistItem()
    {
        Title = string.Empty;
    }

    public int BusinessId { get; set; }
    public Business Business { get; set; } = null!;

    /// <summary>
    /// Gets or sets the quality checklist ID
    /// </summary>
    public int QualityChecklistId { get; set; }
    public QualityChecklist QualityChecklist { get; set; } = null!;

    /// <summary>
    /// Gets or sets the item title/description
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets whether this item is required
    /// </summary>
    public bool IsRequired { get; set; }

    /// <summary>
    /// Gets or sets the display order
    /// </summary>
    public int Order { get; set; }
}

