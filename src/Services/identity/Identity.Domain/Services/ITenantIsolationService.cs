namespace Identity.Domain.Services;

public interface ITenantIsolationService
{
    string GetCurrentTenantId();
    Task<bool> ValidateTenantAccessAsync(string userId, string tenantId, CancellationToken cancellationToken = default);
    Task<bool> IsMultiTenantAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetUserTenantsAsync(string userId, CancellationToken cancellationToken = default);
    Task<bool> HasTenantAccessAsync(string userId, string tenantId, CancellationToken cancellationToken = default);
}
