namespace TossErp.Sales.Application.Common.Interfaces;

/// <summary>
/// Service for getting current user information
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Current user ID
    /// </summary>
    string? UserId { get; }

    /// <summary>
    /// Current tenant ID
    /// </summary>
    string? TenantId { get; }

    /// <summary>
    /// Current user email
    /// </summary>
    string? Email { get; }

    /// <summary>
    /// Current user name
    /// </summary>
    string? Name { get; }

    /// <summary>
    /// Check if user is authenticated
    /// </summary>
    bool IsAuthenticated { get; }
}
