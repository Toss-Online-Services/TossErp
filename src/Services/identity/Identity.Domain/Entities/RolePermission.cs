namespace Identity.Domain.Entities;

public class RolePermission : Entity
{
    public Guid RoleId { get; private set; }
    public Guid PermissionId { get; private set; }
    public DateTime AssignedAt { get; private set; }
    public string AssignedBy { get; private set; }

    public RolePermission(Guid roleId, Guid permissionId, string assignedBy = "system")
    {
        Id = Guid.NewGuid();
        RoleId = roleId;
        PermissionId = permissionId;
        AssignedAt = DateTime.UtcNow;
        AssignedBy = assignedBy ?? "system";
    }

    private RolePermission() { }

    // Navigation properties (for EF Core)
    public Role? Role { get; set; }
    public Permission? Permission { get; set; }
}
