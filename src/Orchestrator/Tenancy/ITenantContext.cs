namespace Orchestrator.Tenancy;

public interface ITenantContext
{
    string? TenantId { get; }
    void Set(string? tenantId);
}

public class TenantContext : ITenantContext
{
    public string? TenantId { get; private set; }
    public void Set(string? tenantId) => TenantId = tenantId;
}
