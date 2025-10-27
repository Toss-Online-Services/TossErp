namespace Toss.Application.Common.Interfaces;

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
/// Service for managing users - abstracts Identity framework from Application layer
/// </summary>
public interface IUserManagementService
{
    Task<List<UserListItemDto>> GetUsersAsync(int skip, int take, string? searchTerm, CancellationToken cancellationToken = default);
    Task<UserDetailInfoDto?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<bool> UpdateUserRolesAsync(string userId, List<string> roles, CancellationToken cancellationToken = default);
}

