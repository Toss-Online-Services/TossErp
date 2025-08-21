namespace TossErp.Setup.Application.Common.Interfaces;

/// <summary>
/// Repository interface for Tenant management
/// </summary>
public interface ITenantRepository
{
    Task<Tenant?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Tenant?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Tenant?> GetByDomainAsync(string domain, CancellationToken cancellationToken = default);
    Task<IEnumerable<Tenant>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Tenant>> GetActiveTenantsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Tenant>> GetTenantsByPlanAsync(SubscriptionPlan plan, CancellationToken cancellationToken = default);
    Task<Tenant> AddAsync(Tenant tenant, CancellationToken cancellationToken = default);
    Task<Tenant> UpdateAsync(Tenant tenant, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> ExistsByDomainAsync(string domain, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository interface for User management
/// </summary>
public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetByRoleAsync(UserRole role, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetActiveUsersAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetInactiveUsersAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<User> AddAsync(User user, CancellationToken cancellationToken = default);
    Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository interface for UserRole management
/// </summary>
public interface IUserRoleRepository
{
    Task<UserRole?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserRole?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserRole>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<UserRole>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserRole>> GetSystemRolesAsync(CancellationToken cancellationToken = default);
    Task<UserRole> AddAsync(UserRole role, CancellationToken cancellationToken = default);
    Task<UserRole> UpdateAsync(UserRole role, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository interface for Permission management
/// </summary>
public interface IPermissionRepository
{
    Task<Permission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Permission?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<Permission>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Permission>> GetByModuleAsync(string module, CancellationToken cancellationToken = default);
    Task<IEnumerable<Permission>> GetByRoleAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Permission>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Permission> AddAsync(Permission permission, CancellationToken cancellationToken = default);
    Task<Permission> UpdateAsync(Permission permission, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository interface for SystemConfig management
/// </summary>
public interface ISystemConfigRepository
{
    Task<SystemConfig?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<SystemConfig?> GetByKeyAsync(string key, CancellationToken cancellationToken = default);
    Task<SystemConfig?> GetByKeyAndTenantAsync(string key, Guid? tenantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<SystemConfig>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<SystemConfig>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<SystemConfig>> GetGlobalConfigsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<SystemConfig>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default);
    Task<SystemConfig> AddAsync(SystemConfig config, CancellationToken cancellationToken = default);
    Task<SystemConfig> UpdateAsync(SystemConfig config, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByKeyAsync(string key, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository interface for AuditLog management
/// </summary>
public interface IAuditLogRepository
{
    Task<AuditLog?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditLog>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditLog>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditLog>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditLog>> GetByEntityAsync(string entityType, Guid entityId, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditLog>> GetByActionAsync(string action, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<AuditLog> AddAsync(AuditLog auditLog, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}

/// <summary>
/// Service interface for tenant management operations
/// </summary>
public interface ITenantService
{
    Task<Tenant> CreateTenantAsync(string name, string domain, SubscriptionPlan plan, CancellationToken cancellationToken = default);
    Task<Tenant> UpgradeTenantPlanAsync(Guid tenantId, SubscriptionPlan newPlan, CancellationToken cancellationToken = default);
    Task<Tenant> SuspendTenantAsync(Guid tenantId, string reason, CancellationToken cancellationToken = default);
    Task<Tenant> ActivateTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task InitializeTenantDataAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<bool> ValidateTenantDomainAsync(string domain, CancellationToken cancellationToken = default);
    Task<TenantUsageStats> GetTenantUsageStatsAsync(Guid tenantId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Service interface for user management operations
/// </summary>
public interface IUserService
{
    Task<User> CreateUserAsync(string email, string username, string firstName, string lastName, Guid tenantId, CancellationToken cancellationToken = default);
    Task<User> AssignRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);
    Task<User> RemoveRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);
    Task<User> ActivateUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<User> DeactivateUserAsync(Guid userId, string reason, CancellationToken cancellationToken = default);
    Task<bool> ValidateUserPermissionAsync(Guid userId, string permission, CancellationToken cancellationToken = default);
    Task<IEnumerable<Permission>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task SendWelcomeEmailAsync(Guid userId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Service interface for system configuration management
/// </summary>
public interface ISystemConfigService
{
    Task<string?> GetConfigValueAsync(string key, Guid? tenantId = null, CancellationToken cancellationToken = default);
    Task<T?> GetConfigValueAsync<T>(string key, Guid? tenantId = null, CancellationToken cancellationToken = default);
    Task SetConfigValueAsync(string key, string value, Guid? tenantId = null, string? category = null, CancellationToken cancellationToken = default);
    Task SetConfigValueAsync<T>(string key, T value, Guid? tenantId = null, string? category = null, CancellationToken cancellationToken = default);
    Task DeleteConfigAsync(string key, Guid? tenantId = null, CancellationToken cancellationToken = default);
    Task<Dictionary<string, string>> GetConfigByCategoryAsync(string category, Guid? tenantId = null, CancellationToken cancellationToken = default);
}

/// <summary>
/// Service interface for audit logging
/// </summary>
public interface IAuditService
{
    Task LogAsync(string action, string entityType, Guid entityId, Guid userId, Guid tenantId, object? oldValues = null, object? newValues = null, CancellationToken cancellationToken = default);
    Task LogUserActionAsync(string action, Guid userId, Guid tenantId, string? details = null, CancellationToken cancellationToken = default);
    Task LogSystemActionAsync(string action, string? details = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditLog>> GetAuditTrailAsync(string entityType, Guid entityId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Service interface for notification handling
/// </summary>
public interface INotificationService
{
    Task SendTenantWelcomeEmailAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task SendUserWelcomeEmailAsync(Guid userId, CancellationToken cancellationToken = default);
    Task SendPasswordResetEmailAsync(Guid userId, string resetToken, CancellationToken cancellationToken = default);
    Task SendTenantSuspensionNoticeAsync(Guid tenantId, string reason, CancellationToken cancellationToken = default);
    Task SendSystemMaintenanceNoticeAsync(DateTime maintenanceDate, string details, CancellationToken cancellationToken = default);
}

/// <summary>
/// Service interface for email operations
/// </summary>
public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, bool isHtml = true, CancellationToken cancellationToken = default);
    Task SendBulkEmailAsync(IEnumerable<string> recipients, string subject, string body, bool isHtml = true, CancellationToken cancellationToken = default);
    Task SendTemplatedEmailAsync(string to, string templateName, object templateData, CancellationToken cancellationToken = default);
}

/// <summary>
/// Tenant usage statistics
/// </summary>
public class TenantUsageStats
{
    public Guid TenantId { get; set; }
    public string TenantName { get; set; } = string.Empty;
    public int UserCount { get; set; }
    public int ActiveUserCount { get; set; }
    public long StorageUsedBytes { get; set; }
    public int ApiCallsThisMonth { get; set; }
    public DateTime LastActivity { get; set; }
    public Dictionary<string, int> FeatureUsage { get; set; } = new();
}
