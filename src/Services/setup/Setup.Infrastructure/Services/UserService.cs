using Microsoft.Extensions.Logging;
using Setup.Application.Common.Interfaces;
using Setup.Domain.Aggregates.TenantAggregate;
using Setup.Domain.ValueObjects;
using System.Security.Cryptography;
using System.Text;

namespace Setup.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly ISetupUnitOfWork _unitOfWork;
    private readonly ILogger<UserService> _logger;

    public UserService(ISetupUnitOfWork unitOfWork, ILogger<UserService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<UserProfile> CreateUserAsync(string email, string firstName, string lastName, 
        string? employeeId = null, string? department = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new user: {Email}", email);

        // Validate unique email
        var isEmailUnique = await _unitOfWork.UserRepository.IsEmailUniqueAsync(email, null, cancellationToken);
        if (!isEmailUnique)
        {
            throw new InvalidOperationException($"Email '{email}' is already in use");
        }

        // Validate unique employee ID if provided
        if (!string.IsNullOrEmpty(employeeId))
        {
            var isEmployeeIdUnique = await _unitOfWork.UserRepository.IsEmployeeIdUniqueAsync(employeeId, null, cancellationToken);
            if (!isEmployeeIdUnique)
            {
                throw new InvalidOperationException($"Employee ID '{employeeId}' is already in use");
            }
        }

        var user = new UserProfile
        {
            Id = Guid.NewGuid().ToString(),
            Email = email.ToLowerInvariant(),
            FirstName = firstName,
            LastName = lastName,
            EmployeeId = employeeId,
            Department = department,
            IsActive = true,
            IsLocked = false,
            TwoFactorEnabled = false,
            FailedLoginAttempts = 0,
            PreferredLanguage = "en-US",
            PreferredTimezone = "UTC",
            PreferredDateFormat = "yyyy-MM-dd",
            SecurityPolicy = CreateDefaultSecurityPolicy(),
            Permissions = new List<string> { "user:read", "profile:edit" },
            Roles = new List<string> { "User" },
            CustomFields = new Dictionary<string, object>()
        };

        await _unitOfWork.UserRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully created user: {UserId} with email: {Email}", user.Id, email);
        return user;
    }

    public async Task<bool> UpdateUserAsync(string userId, string? firstName = null, string? lastName = null,
        string? department = null, string? jobTitle = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating user: {UserId}", userId);

        var user = await _unitOfWork.UserRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("User not found: {UserId}", userId);
            return false;
        }

        // Update fields if provided
        if (!string.IsNullOrEmpty(firstName))
            user.FirstName = firstName;

        if (!string.IsNullOrEmpty(lastName))
            user.LastName = lastName;

        if (!string.IsNullOrEmpty(department))
            user.Department = department;

        if (!string.IsNullOrEmpty(jobTitle))
            user.JobTitle = jobTitle;

        _unitOfWork.UserRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully updated user: {UserId}", userId);
        return true;
    }

    public async Task<bool> ActivateUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Activating user: {UserId}", userId);

        var user = await _unitOfWork.UserRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("User not found: {UserId}", userId);
            return false;
        }

        user.IsActive = true;
        user.IsLocked = false;
        user.FailedLoginAttempts = 0;
        user.LockedUntil = null;

        _unitOfWork.UserRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully activated user: {UserId}", userId);
        return true;
    }

    public async Task<bool> DeactivateUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deactivating user: {UserId}", userId);

        var user = await _unitOfWork.UserRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("User not found: {UserId}", userId);
            return false;
        }

        user.IsActive = false;
        _unitOfWork.UserRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully deactivated user: {UserId}", userId);
        return true;
    }

    public async Task<bool> LockUserAsync(string userId, string reason, TimeSpan? lockDuration = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Locking user: {UserId} for reason: {Reason}", userId, reason);

        var user = await _unitOfWork.UserRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("User not found: {UserId}", userId);
            return false;
        }

        user.IsLocked = true;
        user.LockedUntil = lockDuration.HasValue ? DateTime.UtcNow.Add(lockDuration.Value) : null;

        _unitOfWork.UserRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully locked user: {UserId}", userId);
        return true;
    }

    public async Task<bool> UnlockUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.UserRepository.UnlockUserAsync(userId, cancellationToken);
    }

    public async Task<bool> AssignRoleAsync(string userId, string role, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Assigning role {Role} to user: {UserId}", role, userId);

        var user = await _unitOfWork.UserRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("User not found: {UserId}", userId);
            return false;
        }

        if (!user.Roles.Contains(role))
        {
            user.Roles.Add(role);
            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        _logger.LogInformation("Successfully assigned role {Role} to user: {UserId}", role, userId);
        return true;
    }

    public async Task<bool> RemoveRoleAsync(string userId, string role, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Removing role {Role} from user: {UserId}", role, userId);

        var user = await _unitOfWork.UserRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("User not found: {UserId}", userId);
            return false;
        }

        if (user.Roles.Contains(role))
        {
            user.Roles.Remove(role);
            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        _logger.LogInformation("Successfully removed role {Role} from user: {UserId}", role, userId);
        return true;
    }

    public async Task<bool> GrantPermissionAsync(string userId, string permission, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Granting permission {Permission} to user: {UserId}", permission, userId);

        var user = await _unitOfWork.UserRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("User not found: {UserId}", userId);
            return false;
        }

        if (!user.Permissions.Contains(permission))
        {
            user.Permissions.Add(permission);
            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        _logger.LogInformation("Successfully granted permission {Permission} to user: {UserId}", permission, userId);
        return true;
    }

    public async Task<bool> RevokePermissionAsync(string userId, string permission, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Revoking permission {Permission} from user: {UserId}", permission, userId);

        var user = await _unitOfWork.UserRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("User not found: {UserId}", userId);
            return false;
        }

        if (user.Permissions.Contains(permission))
        {
            user.Permissions.Remove(permission);
            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        _logger.LogInformation("Successfully revoked permission {Permission} from user: {UserId}", permission, userId);
        return true;
    }

    public async Task<bool> EnableTwoFactorAsync(string userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Enabling two-factor authentication for user: {UserId}", userId);

        var user = await _unitOfWork.UserRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("User not found: {UserId}", userId);
            return false;
        }

        user.TwoFactorEnabled = true;
        user.TwoFactorSecret = GenerateTwoFactorSecret();

        _unitOfWork.UserRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully enabled two-factor authentication for user: {UserId}", userId);
        return true;
    }

    public async Task<bool> DisableTwoFactorAsync(string userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Disabling two-factor authentication for user: {UserId}", userId);

        var user = await _unitOfWork.UserRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("User not found: {UserId}", userId);
            return false;
        }

        user.TwoFactorEnabled = false;
        user.TwoFactorSecret = null;

        _unitOfWork.UserRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully disabled two-factor authentication for user: {UserId}", userId);
        return true;
    }

    public async Task<bool> RecordLoginAttemptAsync(string userId, bool successful, string? ipAddress = null, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.UserRepository.RecordLoginAttemptAsync(userId, successful, cancellationToken);
    }

    public async Task<bool> ChangePasswordAsync(string userId, string newPasswordHash, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Changing password for user: {UserId}", userId);

        var user = await _unitOfWork.UserRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("User not found: {UserId}", userId);
            return false;
        }

        user.PasswordLastChangedAt = DateTime.UtcNow;
        
        // Update security policy if required
        var securityPolicy = user.SecurityPolicy;
        securityPolicy.RequirePasswordChange = false;
        user.SecurityPolicy = securityPolicy;

        _unitOfWork.UserRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully changed password for user: {UserId}", userId);
        return true;
    }

    public async Task<UserMetrics> GetUserMetricsAsync(CancellationToken cancellationToken = default)
    {
        var totalUsers = await _unitOfWork.UserRepository.GetTotalUsersCountAsync(cancellationToken);
        var activeUsers = await _unitOfWork.UserRepository.GetActiveUsersCountAsync(cancellationToken);
        var lockedUsers = await _unitOfWork.UserRepository.GetLockedUsersAsync(cancellationToken);
        var usersWithFailedLogins = await _unitOfWork.UserRepository.GetUsersWithFailedLoginsAsync(3, cancellationToken);
        var inactiveUsers = await _unitOfWork.UserRepository.GetInactiveUsersAsync(30, cancellationToken);
        var departmentCounts = await _unitOfWork.UserRepository.GetUserCountByDepartmentAsync(cancellationToken);
        var roleCounts = await _unitOfWork.UserRepository.GetUserCountByRoleAsync(cancellationToken);

        return new UserMetrics
        {
            TotalUsers = totalUsers,
            ActiveUsers = activeUsers,
            InactiveUsers = totalUsers - activeUsers,
            LockedUsers = lockedUsers.Count(),
            UsersWithFailedLogins = usersWithFailedLogins.Count(),
            InactiveUsersLast30Days = inactiveUsers.Count(),
            UsersByDepartment = departmentCounts,
            UsersByRole = roleCounts
        };
    }

    public async Task<IEnumerable<UserProfile>> SearchUsersAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.UserRepository.SearchAsync(searchTerm, cancellationToken);
    }

    public async Task ProcessSecurityMaintenanceAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing security maintenance tasks");

        // Unlock users whose lock period has expired
        var lockedUsers = await _unitOfWork.UserRepository.GetLockedUsersAsync(cancellationToken);
        var now = DateTime.UtcNow;
        
        foreach (var user in lockedUsers.Where(u => u.LockedUntil.HasValue && u.LockedUntil.Value <= now))
        {
            await UnlockUserAsync(user.Id, cancellationToken);
        }

        // Identify users with expired passwords
        var usersWithExpiredPasswords = await _unitOfWork.UserRepository.GetUsersWithExpiredPasswordsAsync(cancellationToken);
        foreach (var user in usersWithExpiredPasswords)
        {
            var securityPolicy = user.SecurityPolicy;
            securityPolicy.RequirePasswordChange = true;
            user.SecurityPolicy = securityPolicy;
            _unitOfWork.UserRepository.Update(user);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Completed security maintenance tasks");
    }

    private static SecurityPolicy CreateDefaultSecurityPolicy()
    {
        return new SecurityPolicy
        {
            RequirePasswordChange = false,
            PasswordExpiryDays = 90,
            RequireTwoFactor = false,
            AllowedIpAddresses = null,
            SessionTimeoutMinutes = 30,
            MaxConcurrentSessions = 3
        };
    }

    private static string GenerateTwoFactorSecret()
    {
        // Generate a random 20-byte secret for TOTP
        var bytes = new byte[20];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase32String(bytes);
    }
}

// Extension method for Base32 encoding
public static class Base32Extensions
{
    private const string Base32Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

    public static string ToBase32String(this byte[] bytes)
    {
        if (bytes == null || bytes.Length == 0)
            return string.Empty;

        var result = new StringBuilder();
        var buffer = 0;
        var bufferLength = 0;

        foreach (var b in bytes)
        {
            buffer = (buffer << 8) | b;
            bufferLength += 8;

            while (bufferLength >= 5)
            {
                result.Append(Base32Chars[(buffer >> (bufferLength - 5)) & 31]);
                bufferLength -= 5;
            }
        }

        if (bufferLength > 0)
        {
            result.Append(Base32Chars[(buffer << (5 - bufferLength)) & 31]);
        }

        return result.ToString();
    }
}

public class UserMetrics
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int InactiveUsers { get; set; }
    public int LockedUsers { get; set; }
    public int UsersWithFailedLogins { get; set; }
    public int InactiveUsersLast30Days { get; set; }
    public Dictionary<string, int> UsersByDepartment { get; set; } = new();
    public Dictionary<string, int> UsersByRole { get; set; } = new();
}
