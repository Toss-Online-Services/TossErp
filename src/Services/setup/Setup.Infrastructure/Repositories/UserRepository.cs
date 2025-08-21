using Microsoft.EntityFrameworkCore;
using Setup.Application.Common.Interfaces;
using Setup.Domain.Aggregates.TenantAggregate;
using Setup.Infrastructure.Data;

namespace Setup.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly SetupDbContext _context;

    public UserRepository(SetupDbContext context)
    {
        _context = context;
    }

    public async Task<UserProfile?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _context.UserProfiles
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<UserProfile?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.UserProfiles
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<UserProfile?> GetByEmployeeIdAsync(string employeeId, CancellationToken cancellationToken = default)
    {
        return await _context.UserProfiles
            .FirstOrDefaultAsync(u => u.EmployeeId == employeeId, cancellationToken);
    }

    public async Task<IEnumerable<UserProfile>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.UserProfiles
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<UserProfile>> GetActiveUsersAsync(CancellationToken cancellationToken = default)
    {
        return await _context.UserProfiles
            .Where(u => u.IsActive && !u.IsLocked)
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<UserProfile>> GetByDepartmentAsync(string department, CancellationToken cancellationToken = default)
    {
        return await _context.UserProfiles
            .Where(u => u.Department == department)
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<UserProfile>> GetByManagerAsync(string managerId, CancellationToken cancellationToken = default)
    {
        return await _context.UserProfiles
            .Where(u => u.ManagerId == managerId)
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<UserProfile>> GetByRoleAsync(string role, CancellationToken cancellationToken = default)
    {
        return await _context.UserProfiles
            .Where(u => u.Roles.Contains(role))
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<UserProfile>> GetLockedUsersAsync(CancellationToken cancellationToken = default)
    {
        return await _context.UserProfiles
            .Where(u => u.IsLocked)
            .OrderByDescending(u => u.LockedUntil)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<UserProfile>> GetUsersWithFailedLoginsAsync(int minFailedAttempts, CancellationToken cancellationToken = default)
    {
        return await _context.UserProfiles
            .Where(u => u.FailedLoginAttempts >= minFailedAttempts)
            .OrderByDescending(u => u.FailedLoginAttempts)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<UserProfile>> GetInactiveUsersAsync(int daysSinceLastLogin, CancellationToken cancellationToken = default)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-daysSinceLastLogin);
        return await _context.UserProfiles
            .Where(u => u.IsActive && (u.LastLoginAt == null || u.LastLoginAt < cutoffDate))
            .OrderBy(u => u.LastLoginAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsEmailUniqueAsync(string email, string? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.UserProfiles.Where(u => u.Email == email);
        
        if (!string.IsNullOrEmpty(excludeId))
        {
            query = query.Where(u => u.Id != excludeId);
        }

        return !await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> IsEmployeeIdUniqueAsync(string employeeId, string? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.UserProfiles.Where(u => u.EmployeeId == employeeId);
        
        if (!string.IsNullOrEmpty(excludeId))
        {
            query = query.Where(u => u.Id != excludeId);
        }

        return !await query.AnyAsync(cancellationToken);
    }

    public async Task<int> GetTotalUsersCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.UserProfiles.CountAsync(cancellationToken);
    }

    public async Task<int> GetActiveUsersCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.UserProfiles
            .CountAsync(u => u.IsActive && !u.IsLocked, cancellationToken);
    }

    public async Task<int> GetUsersByDepartmentCountAsync(string department, CancellationToken cancellationToken = default)
    {
        return await _context.UserProfiles
            .CountAsync(u => u.Department == department, cancellationToken);
    }

    public async Task AddAsync(UserProfile user, CancellationToken cancellationToken = default)
    {
        await _context.UserProfiles.AddAsync(user, cancellationToken);
    }

    public void Update(UserProfile user)
    {
        _context.UserProfiles.Update(user);
    }

    public void Remove(UserProfile user)
    {
        _context.UserProfiles.Remove(user);
    }

    // Advanced user management methods
    public async Task<IEnumerable<UserProfile>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await _context.UserProfiles
            .Where(u => u.FirstName.Contains(searchTerm) || 
                       u.LastName.Contains(searchTerm) || 
                       u.Email.Contains(searchTerm) ||
                       u.EmployeeId!.Contains(searchTerm) ||
                       u.Department!.Contains(searchTerm))
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<UserProfile>> GetUsersRequiringPasswordChangeAsync(CancellationToken cancellationToken = default)
    {
        return await _context.UserProfiles
            .Where(u => u.IsActive && u.SecurityPolicy.RequirePasswordChange)
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<UserProfile>> GetUsersWithExpiredPasswordsAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _context.UserProfiles
            .Where(u => u.IsActive && 
                       u.PasswordLastChangedAt.HasValue &&
                       u.PasswordLastChangedAt.Value.AddDays(u.SecurityPolicy.PasswordExpiryDays) < now)
            .OrderBy(u => u.PasswordLastChangedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<UserProfile>> GetRecentlyCreatedUsersAsync(int days, CancellationToken cancellationToken = default)
    {
        var fromDate = DateTime.UtcNow.AddDays(-days);
        return await _context.UserProfiles
            .Where(u => EF.Property<DateTime>(u, "CreatedAt") >= fromDate)
            .OrderByDescending(u => EF.Property<DateTime>(u, "CreatedAt"))
            .ToListAsync(cancellationToken);
    }

    public async Task<Dictionary<string, int>> GetUserCountByDepartmentAsync(CancellationToken cancellationToken = default)
    {
        return await _context.UserProfiles
            .Where(u => !string.IsNullOrEmpty(u.Department))
            .GroupBy(u => u.Department!)
            .ToDictionaryAsync(g => g.Key, g => g.Count(), cancellationToken);
    }

    public async Task<Dictionary<string, int>> GetUserCountByRoleAsync(CancellationToken cancellationToken = default)
    {
        // This is a simplified version - in practice, you'd need to handle JSON arrays properly
        var users = await _context.UserProfiles.ToListAsync(cancellationToken);
        
        var roleCounts = new Dictionary<string, int>();
        foreach (var user in users)
        {
            foreach (var role in user.Roles)
            {
                if (roleCounts.ContainsKey(role))
                    roleCounts[role]++;
                else
                    roleCounts[role] = 1;
            }
        }
        
        return roleCounts;
    }

    // Security and audit methods
    public async Task RecordLoginAttemptAsync(string userId, bool successful, CancellationToken cancellationToken = default)
    {
        var user = await GetByIdAsync(userId, cancellationToken);
        if (user != null)
        {
            if (successful)
            {
                user.LastLoginAt = DateTime.UtcNow;
                user.FailedLoginAttempts = 0;
            }
            else
            {
                user.FailedLoginAttempts++;
            }
            
            Update(user);
        }
    }

    public async Task LockUserAsync(string userId, DateTime? lockUntil = null, CancellationToken cancellationToken = default)
    {
        var user = await GetByIdAsync(userId, cancellationToken);
        if (user != null)
        {
            user.IsLocked = true;
            user.LockedUntil = lockUntil;
            Update(user);
        }
    }

    public async Task UnlockUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await GetByIdAsync(userId, cancellationToken);
        if (user != null)
        {
            user.IsLocked = false;
            user.LockedUntil = null;
            user.FailedLoginAttempts = 0;
            Update(user);
        }
    }
}
