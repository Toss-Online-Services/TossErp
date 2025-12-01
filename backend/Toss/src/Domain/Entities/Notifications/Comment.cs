using Toss.Domain.Common;
using Toss.Domain.Entities.Businesses;

namespace Toss.Domain.Entities.Notifications;

/// <summary>
/// Represents a comment on a transaction or entity
/// </summary>
public class Comment : BaseAuditableEntity, IBusinessScopedEntity
{
    public Comment()
    {
        Body = string.Empty;
    }

    public int BusinessId { get; set; }
    public Business Business { get; set; } = null!;

    /// <summary>
    /// Gets or sets the type of entity this comment is linked to (e.g., "Sale", "PurchaseOrder", "Project")
    /// </summary>
    public string LinkedType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the ID of the linked entity
    /// </summary>
    public int LinkedId { get; set; }

    /// <summary>
    /// Gets or sets the comment body/text
    /// </summary>
    public string Body { get; set; }

    // Note: CreatedBy is inherited from BaseAuditableEntity

    /// <summary>
    /// Gets or sets the parent comment ID (for threaded comments)
    /// </summary>
    public int? ParentCommentId { get; set; }
    public Comment? ParentComment { get; set; }

    /// <summary>
    /// Gets or sets child comments (replies)
    /// </summary>
    public ICollection<Comment> Replies { get; private set; } = new List<Comment>();

    /// <summary>
    /// Gets or sets whether this comment is edited
    /// </summary>
    public bool IsEdited { get; set; }
}

