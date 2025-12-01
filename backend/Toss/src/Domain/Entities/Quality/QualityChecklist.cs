using Toss.Domain.Common;
using Toss.Domain.Entities.Businesses;

namespace Toss.Domain.Entities.Quality;

/// <summary>
/// Represents a quality checklist template
/// </summary>
public class QualityChecklist : BaseAuditableEntity, IBusinessScopedEntity
{
    public QualityChecklist()
    {
        Name = string.Empty;
        Items = new List<ChecklistItem>();
    }

    public int BusinessId { get; set; }
    public Business Business { get; set; } = null!;

    /// <summary>
    /// Gets or sets the checklist name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the checklist description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets whether this checklist is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the collection of checklist items
    /// </summary>
    public ICollection<ChecklistItem> Items { get; private set; }

    /// <summary>
    /// Gets or sets the collection of checklist runs
    /// </summary>
    public ICollection<ChecklistRun> Runs { get; private set; } = new List<ChecklistRun>();
}

