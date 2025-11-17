namespace Toss.Domain.Entities.Security;

/// <summary>
/// Represents a permission record
/// </summary>
public class PermissionRecord : BaseEntity
{
    public PermissionRecord()
    {
        Name = string.Empty;
        SystemName = string.Empty;
        Category = string.Empty;
        PermissionRoleMappings = new List<PermissionRoleMapping>();
    }

    /// <summary>
    /// Gets or sets the name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the system name
    /// </summary>
    public string SystemName { get; set; }

    /// <summary>
    /// Gets or sets the category
    /// </summary>
    public string Category { get; set; }

    /// <summary>
    /// Gets or sets the permission-role mappings
    /// </summary>
    public ICollection<PermissionRoleMapping> PermissionRoleMappings { get; set; }
}

