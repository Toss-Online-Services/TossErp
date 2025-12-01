using Toss.Domain.Common;
using Toss.Domain.Entities.Businesses;
using Toss.Domain.Enums;

namespace Toss.Domain.Entities.Collaborations;

/// <summary>
/// Represents a tokenized link for public collaboration (feedback/offers)
/// </summary>
public class CollabLink : BaseAuditableEntity, IBusinessScopedEntity
{
    public CollabLink()
    {
        LinkCode = string.Empty;
        LinkedType = string.Empty;
    }

    public int BusinessId { get; set; }
    public Business Business { get; set; } = null!;

    /// <summary>
    /// Gets or sets the unique link code/token
    /// </summary>
    public string LinkCode { get; set; }

    /// <summary>
    /// Gets or sets the type of entity this link is for (e.g., "Sale", "PurchaseRequest")
    /// </summary>
    public string LinkedType { get; set; }

    /// <summary>
    /// Gets or sets the ID of the linked entity
    /// </summary>
    public int LinkedId { get; set; }

    /// <summary>
    /// Gets or sets the purpose of this link (Feedback or Offer)
    /// </summary>
    public CollabLinkPurpose Purpose { get; set; }

    /// <summary>
    /// Gets or sets whether this link is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets when this link expires (optional)
    /// </summary>
    public DateTimeOffset? ExpiresAt { get; set; }
}

