namespace Toss.Application.Common.Interfaces;

/// <summary>
/// Data transfer object containing essential user information for list views.
/// </summary>
/// <param name="Id">The unique identifier of the user.</param>
/// <param name="UserName">The user's username.</param>
/// <param name="Email">The user's email address.</param>
/// <param name="EmailConfirmed">Indicates if the email has been verified.</param>
/// <param name="PhoneNumber">The user's phone number.</param>
/// <param name="TwoFactorEnabled">Indicates if two-factor authentication is enabled.</param>
/// <param name="LockoutEnd">The date/time when account lockout expires, if locked.</param>
/// <param name="LockoutEnabled">Indicates if account lockout is enabled for this user.</param>
/// <param name="Roles">List of role names assigned to the user.</param>
public record UserListItemDto(
    string Id,
    string? UserName,
    string? Email,
    bool EmailConfirmed,
    string? PhoneNumber,
    bool TwoFactorEnabled,
    DateTimeOffset? LockoutEnd,
    bool LockoutEnabled,
    List<string> Roles
);

/// <summary>
/// Data transfer object containing comprehensive user information for detail views.
/// </summary>
/// <param name="Id">The unique identifier of the user.</param>
/// <param name="UserName">The user's username.</param>
/// <param name="Email">The user's email address.</param>
/// <param name="EmailConfirmed">Indicates if the email has been verified.</param>
/// <param name="PhoneNumber">The user's phone number.</param>
/// <param name="PhoneNumberConfirmed">Indicates if the phone number has been verified.</param>
/// <param name="TwoFactorEnabled">Indicates if two-factor authentication is enabled.</param>
/// <param name="LockoutEnd">The date/time when account lockout expires, if locked.</param>
/// <param name="LockoutEnabled">Indicates if account lockout is enabled for this user.</param>
/// <param name="AccessFailedCount">Number of failed login attempts.</param>
/// <param name="Roles">List of role names assigned to the user.</param>
/// <param name="Claims">List of custom claims assigned to the user.</param>
public record UserDetailInfoDto(
    string Id,
    string? UserName,
    string? Email,
    bool EmailConfirmed,
    string? PhoneNumber,
    bool PhoneNumberConfirmed,
    bool TwoFactorEnabled,
    DateTimeOffset? LockoutEnd,
    bool LockoutEnabled,
    int AccessFailedCount,
    List<string> Roles,
    List<string> Claims
);

/// <summary>
/// Service for managing users - abstracts Identity framework from Application layer.
/// Provides high-level user management operations for administrative tasks.
/// </summary>
/// <remarks>
/// This service focuses on user administration and querying, complementing
/// <see cref="IIdentityService"/> which handles authentication and authorization.
/// </remarks>
public interface IUserManagementService
{
    /// <summary>
    /// Retrieves a paginated and searchable list of users.
    /// </summary>
    /// <param name="skip">Number of users to skip for pagination.</param>
    /// <param name="take">Number of users to take for pagination.</param>
    /// <param name="searchTerm">Optional search term to filter users by username or email.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>List of users matching the criteria.</returns>
    Task<List<UserListItemDto>> GetUsersAsync(int skip, int take, string? searchTerm, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Retrieves comprehensive information about a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>Detailed user information if found, null otherwise.</returns>
    Task<UserDetailInfoDto?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates the complete list of roles assigned to a user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="roles">The complete list of role names to assign (replaces existing roles).</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>True if roles were updated successfully, false otherwise.</returns>
    Task<bool> UpdateUserRolesAsync(string userId, List<string> roles, CancellationToken cancellationToken = default);
}


