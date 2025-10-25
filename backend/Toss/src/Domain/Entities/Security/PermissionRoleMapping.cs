namespace Toss.Domain.Entities.Security;

/// <summary>
/// Represents a permission-role mapping
/// </summary>
public class PermissionRoleMapping : BaseEntity
{
    public PermissionRoleMapping()
    {
        RoleName = string.Empty;
    }

    /// <summary>
    /// Gets or sets the permission ID
    /// </summary>
    public int PermissionRecordId { get; set; }
    public PermissionRecord? PermissionRecord { get; set; }

    /// <summary>
    /// Gets or sets the role name (from Identity)
    /// </summary>
    public string RoleName { get; set; }
}

