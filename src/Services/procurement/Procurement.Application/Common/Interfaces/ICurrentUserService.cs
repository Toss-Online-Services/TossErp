namespace TossErp.Procurement.Application.Common.Interfaces;

/// <summary>
/// Service for getting current user information
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Get current user ID
    /// </summary>
    string? UserId { get; }

    /// <summary>
    /// Get current user name
    /// </summary>
    string? UserName { get; }

    /// <summary>
    /// Get current user email
    /// </summary>
    string? UserEmail { get; }

    /// <summary>
    /// Get current tenant ID
    /// </summary>
    string? TenantId { get; }

    /// <summary>
    /// Check if user is authenticated
    /// </summary>
    bool IsAuthenticated { get; }
}
