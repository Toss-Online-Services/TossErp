using Toss.Domain.Common;
using Toss.Domain.Entities.Businesses;

namespace Toss.Domain.Entities.Audit;

/// <summary>
/// Represents an audit entry for tracking entity changes
/// </summary>
public class AuditEntry : BaseAuditableEntity, IBusinessScopedEntity
{
    public int BusinessId { get; set; }
    public Business Business { get; set; } = null!;

    /// <summary>
    /// Gets or sets the entity type name (e.g., "Product", "Sale")
    /// </summary>
    public string EntityType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the entity ID
    /// </summary>
    public int EntityId { get; set; }

    /// <summary>
    /// Gets or sets the action performed (Created, Updated, Deleted)
    /// </summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user ID who performed the action
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Gets or sets the user name (denormalized for quick access)
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// Gets or sets the changes made (JSON diff for updates)
    /// </summary>
    public string? Changes { get; set; }

    /// <summary>
    /// Gets or sets additional context or notes
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets the IP address (if available)
    /// </summary>
    public string? IpAddress { get; set; }
}

