using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Toss.Application.Common.Interfaces;

namespace Toss.Infrastructure.Identity.Services;

public class UserManagementService : IUserManagementService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserManagementService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<List<UserListItemDto>> GetUsersAsync(int skip, int take, string? searchTerm, string? role = null, CancellationToken cancellationToken = default)                         
    {
        IQueryable<ApplicationUser> query = _userManager.Users;

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(u =>
                (u.UserName != null && u.UserName.Contains(searchTerm)) ||      
                (u.Email != null && u.Email.Contains(searchTerm)) ||
                (u.PhoneNumber != null && u.PhoneNumber.Contains(searchTerm))   
            );
        }

        // Filter by role if specified
        if (!string.IsNullOrEmpty(role))
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync(role);
            var userIdsInRole = usersInRole.Select(u => u.Id).ToList();
            query = query.Where(u => userIdsInRole.Contains(u.Id));
        }

        var users = await query
            .OrderBy(u => u.UserName)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        var userDtos = new List<UserListItemDto>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userDtos.Add(new UserListItemDto(
                user.Id,
                user.UserName,
                user.Email,
                user.EmailConfirmed,
                user.PhoneNumber,
                user.TwoFactorEnabled,
                user.LockoutEnd,
                user.LockoutEnabled,
                roles.ToList()
            ));
        }

        return userDtos;
    }

    public async Task<UserDetailInfoDto?> GetUserByIdAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return null;

        var roles = await _userManager.GetRolesAsync(user);
        var claims = await _userManager.GetClaimsAsync(user);

        return new UserDetailInfoDto(
            user.Id,
            user.UserName,
            user.Email,
            user.EmailConfirmed,
            user.PhoneNumber,
            user.PhoneNumberConfirmed,
            user.TwoFactorEnabled,
            user.LockoutEnd,
            user.LockoutEnabled,
            user.AccessFailedCount,
            roles.ToList(),
            claims.Select(c => $"{c.Type}: {c.Value}").ToList()
        );
    }

    public async Task<bool> UpdateUserRolesAsync(string userId, List<string> roles, CancellationToken cancellationToken)                                        
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return false;

        // Get current roles
        var currentRoles = await _userManager.GetRolesAsync(user);

        // Remove user from all current roles
        var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);                                                                         
        if (!removeResult.Succeeded)
            return false;

        // Add user to new roles
        var addResult = await _userManager.AddToRolesAsync(user, roles);        
        return addResult.Succeeded;
    }

    public async Task<bool> ActivateUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return false;

        // Remove lockout to activate user
        var result = await _userManager.SetLockoutEndDateAsync(user, null);
        if (result.Succeeded)
        {
            // Reset access failed count
            await _userManager.ResetAccessFailedCountAsync(user);
        }
        
        return result.Succeeded;
    }

    public async Task<bool> DeactivateUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return false;

        // Set lockout to deactivate user (lockout until year 9999 effectively disables)
        var lockoutEnd = DateTimeOffset.UtcNow.AddYears(100);
        var result = await _userManager.SetLockoutEndDateAsync(user, lockoutEnd);
        
        return result.Succeeded;
    }
}

