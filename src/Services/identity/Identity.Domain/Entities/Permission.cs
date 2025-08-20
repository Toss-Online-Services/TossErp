namespace Identity.Domain.Entities;

public class Permission : Entity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string Resource { get; private set; }
    public string Action { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string TenantId { get; private set; }

    public Permission(string name, string description, string resource, string action, string tenantId)
    {
        Id = Guid.NewGuid();
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? string.Empty;
        Resource = resource ?? throw new ArgumentNullException(nameof(resource));
        Action = action ?? throw new ArgumentNullException(nameof(action));
        TenantId = tenantId ?? throw new ArgumentNullException(nameof(tenantId));
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    private Permission() { }

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

    public string FullPermission => $"{Resource}:{Action}";
}
