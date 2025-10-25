namespace Toss.Domain.Entities.Security;

/// <summary>
/// Represents an ACL record for entity-level access control
/// </summary>
public class AclRecord : BaseEntity
{
    public AclRecord()
    {
        EntityName = string.Empty;
        RoleName = string.Empty;
    }

    /// <summary>
    /// Gets or sets the entity ID
    /// </summary>
    public int EntityId { get; set; }

    /// <summary>
    /// Gets or sets the entity name (e.g., "Product", "Category")
    /// </summary>
    public string EntityName { get; set; }

    /// <summary>
    /// Gets or sets the role name
    /// </summary>
    public string RoleName { get; set; }
}

