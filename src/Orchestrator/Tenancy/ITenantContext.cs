using Microsoft.Extensions.DependencyInjection;

namespace Orchestrator.Tenancy;

public interface ITenantContext
{
    string? TenantId { get; }
    IDictionary<string, object?> Metadata { get; }
    void Set(string? tenantId);
}

public class TenantContext : ITenantContext
{
    public string? TenantId { get; private set; }
    public IDictionary<string, object?> Metadata { get; } = new Dictionary<string, object?>();
    public void Set(string? tenantId) => TenantId = tenantId;
}

public static class TenantContextServiceExtensions
{
    public static IServiceCollection AddTenantContext(this IServiceCollection services)
    {
        return services.AddScoped<ITenantContext, TenantContext>();
    }
}
