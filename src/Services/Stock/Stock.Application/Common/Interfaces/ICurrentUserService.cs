namespace TossErp.Stock.Application.Common.Interfaces;

/// <summary>
/// Service for accessing current user information
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Gets the current user ID
    /// </summary>
    string? UserId { get; }

    /// <summary>
    /// Gets the current user's company ID
    /// </summary>
    string? CompanyId { get; }

    /// <summary>
    /// Gets the current user's username
    /// </summary>
    string? Username { get; }

    /// <summary>
    /// Checks if the current user is authenticated
    /// </summary>
    bool IsAuthenticated { get; }
} 
