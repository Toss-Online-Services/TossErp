namespace AgentManager.Tenancy;

/// <summary>
/// Provides access to the current tenant information for the request.
/// </summary>
public interface ITenantContext
{
    /// <summary>
    /// Gets the current tenant ID from the request context.
    /// </summary>
    string? TenantId { get; }
    
    /// <summary>
    /// Gets additional tenant metadata if available.
    /// </summary>
    IDictionary<string, object> Metadata { get; }
    
    /// <summary>
    /// Sets the tenant ID for the current request context.
    /// </summary>
    void Set(string? tenantId);
}

/// <summary>
/// Default implementation that extracts tenant from HTTP headers or JWT claims.
/// </summary>
public class TenantContext : ITenantContext
{
    public string? TenantId { get; private set; }
    public IDictionary<string, object> Metadata { get; } = new Dictionary<string, object>();
    
    public void Set(string? tenantId) => TenantId = tenantId;
}

/// <summary>
/// Extension methods for registering tenant services.
/// </summary>
public static class TenantServiceExtensions
{
    public static IServiceCollection AddTenantContext(this IServiceCollection services)
    {
        services.AddScoped<ITenantContext, TenantContext>();
        return services;
    }
}
