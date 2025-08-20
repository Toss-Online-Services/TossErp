namespace Identity.Domain.Services;

public interface IRbacAuthorizationService
{
    Task<bool> HasPermissionAsync(string userId, string permission, CancellationToken cancellationToken = default);
    Task<bool> HasRoleAsync(string userId, string role, CancellationToken cancellationToken = default);
    Task<bool> HasAnyRoleAsync(string userId, IEnumerable<string> roles, CancellationToken cancellationToken = default);
    Task<bool> HasAnyPermissionAsync(string userId, IEnumerable<string> permissions, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetUserPermissionsAsync(string userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetUserRolesAsync(string userId, CancellationToken cancellationToken = default);
    Task<bool> IsInTenantAsync(string userId, string tenantId, CancellationToken cancellationToken = default);
}
