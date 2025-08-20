namespace Identity.Domain.Entities;

public class Role : Entity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string TenantId { get; private set; }
    
    private readonly List<RolePermission> _rolePermissions = new();
    public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions.AsReadOnly();

    public Role(string name, string description, string tenantId)
    {
        Id = Guid.NewGuid();
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? string.Empty;
        TenantId = tenantId ?? throw new ArgumentNullException(nameof(tenantId));
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    private Role() { }

    public void Update(string description)
    {
        Description = description ?? string.Empty;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void AddPermission(Permission permission)
    {
        if (permission == null)
            throw new ArgumentNullException(nameof(permission));

        if (!_rolePermissions.Any(rp => rp.PermissionId == permission.Id))
        {
            _rolePermissions.Add(new RolePermission(Id, permission.Id));
        }
    }

    public void RemovePermission(Guid permissionId)
    {
        var rolePermission = _rolePermissions.FirstOrDefault(rp => rp.PermissionId == permissionId);
        if (rolePermission != null)
        {
            _rolePermissions.Remove(rolePermission);
        }
    }

    public bool HasPermission(string permissionName)
    {
        return _rolePermissions.Any(rp => rp.Permission?.Name == permissionName);
    }

    public IEnumerable<Permission> Permissions => _rolePermissions.Select(rp => rp.Permission).Where(p => p != null);
}
