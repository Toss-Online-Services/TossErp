namespace Identity.Domain.Entities;

public class UserRole : Entity
{
    public Guid UserId { get; private set; }
    public Guid RoleId { get; private set; }
    public DateTime AssignedAt { get; private set; }
    public string AssignedBy { get; private set; }

    public UserRole(Guid userId, Guid roleId, string assignedBy = "system")
    {
        Id = Guid.NewGuid();
        UserId = userId;
        RoleId = roleId;
        AssignedAt = DateTime.UtcNow;
        AssignedBy = assignedBy ?? "system";
    }

    private UserRole() { }

    // Navigation properties (for EF Core)
    public User? User { get; set; }
    public Role? Role { get; set; }
}
