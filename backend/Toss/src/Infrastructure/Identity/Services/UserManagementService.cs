using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Models;
using Toss.Application.Common.Models.Businesses;
using Toss.Domain.Constants;
using Toss.Domain.Entities.Businesses;
using Toss.Infrastructure.Data;

namespace Toss.Infrastructure.Identity.Services;

public class UserManagementService : IUserManagementService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;

    public UserManagementService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
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

    public async Task<IReadOnlyList<BusinessMemberDto>> GetBusinessMembersAsync(
        int businessId,
        CancellationToken cancellationToken = default)
    {
        var query =
            from membership in _context.UserBusinesses.AsNoTracking()
            join user in _context.Users.AsNoTracking()
                on membership.UserId equals user.Id
            where membership.BusinessId == businessId
            orderby user.Email
            select new BusinessMemberDto(
                membership.BusinessId,
                membership.UserId,
                user.Email ?? string.Empty,
                user.FirstName,
                user.LastName,
                membership.Role,
                membership.IsDefault);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<Result> UpsertBusinessMemberAsync(
        int businessId,
        string userId,
        string role,
        bool isDefault,
        CancellationToken cancellationToken = default)
    {
        if (!BusinessRoles.All.Contains(role, StringComparer.OrdinalIgnoreCase))
        {
            return Result.Failure(new[] { $"Role '{role}' is not allowed." });
        }

        var normalizedRole = BusinessRoles.All
            .First(r => string.Equals(r, role, StringComparison.OrdinalIgnoreCase));

        if (!await _context.Businesses.AnyAsync(b => b.Id == businessId, cancellationToken))
        {
            return Result.Failure(new[] { "Business not found." });
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return Result.Failure(new[] { "User not found." });
        }

        var membership = await _context.UserBusinesses
            .FirstOrDefaultAsync(
                ub => ub.BusinessId == businessId && ub.UserId == userId,
                cancellationToken);

        if (membership is null)
        {
            membership = new UserBusiness
            {
                BusinessId = businessId,
                UserId = userId,
                Role = normalizedRole,
                IsDefault = isDefault
            };

            _context.UserBusinesses.Add(membership);
        }
        else
        {
            membership.Role = normalizedRole;
            membership.IsDefault = isDefault;
        }

        if (isDefault)
        {
            var others = await _context.UserBusinesses
                .Where(ub => ub.BusinessId == businessId && ub.UserId != userId && ub.IsDefault)
                .ToListAsync(cancellationToken);

            foreach (var other in others)
            {
                other.IsDefault = false;
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> RemoveBusinessMemberAsync(
        int businessId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        var membership = await _context.UserBusinesses
            .FirstOrDefaultAsync(
                ub => ub.BusinessId == businessId && ub.UserId == userId,
                cancellationToken);

        if (membership is null)
        {
            return Result.Success();
        }

        _context.UserBusinesses.Remove(membership);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

