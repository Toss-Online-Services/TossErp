namespace Identity.Domain.Common;

public abstract class TenantEntity : Entity
{
    public string TenantId { get; protected set; } = string.Empty;
    
    protected TenantEntity() { }
    
    protected TenantEntity(string tenantId)
    {
        TenantId = tenantId ?? throw new ArgumentNullException(nameof(tenantId));
    }

    public bool BelongsToTenant(string tenantId)
    {
        return string.Equals(TenantId, tenantId, StringComparison.OrdinalIgnoreCase);
    }
}
