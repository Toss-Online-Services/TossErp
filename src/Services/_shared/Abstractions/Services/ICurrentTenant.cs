namespace TossErp.Abstractions.Services;

/// <summary>
/// Service to get the current tenant context
/// </summary>
public interface ICurrentTenant
{
    /// <summary>
    /// Get the current tenant ID
    /// </summary>
    string? TenantId { get; }

    /// <summary>
    /// Get the current user ID
    /// </summary>
    string? UserId { get; }

    /// <summary>
    /// Check if the current context has a tenant
    /// </summary>
    bool HasTenant => !string.IsNullOrEmpty(TenantId);

    /// <summary>
    /// Check if the current context has a user
    /// </summary>
    bool HasUser => !string.IsNullOrEmpty(UserId);
}
