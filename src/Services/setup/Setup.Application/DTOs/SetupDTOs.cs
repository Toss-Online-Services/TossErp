namespace TossErp.Setup.Application.DTOs;

/// <summary>
/// Tenant data transfer object
/// </summary>
public class TenantDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public SubscriptionPlan SubscriptionPlan { get; set; }
    public string SubscriptionPlanName { get; set; } = string.Empty;
    public TenantStatus Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ActivatedAt { get; set; }
    public DateTime? SuspendedAt { get; set; }
    public string? SuspensionReason { get; set; }
    public Dictionary<string, string> Settings { get; set; } = new();
    public ContactInfo ContactInfo { get; set; } = new();
    public int UserCount { get; set; }
    public int ActiveUserCount { get; set; }
    public long StorageUsedBytes { get; set; }
    public bool IsActive { get; set; }
    public bool IsTrialExpired { get; set; }
    public DateTime? TrialExpiresAt { get; set; }
}

/// <summary>
/// Tenant summary data transfer object
/// </summary>
public class TenantSummaryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public string SubscriptionPlanName { get; set; } = string.Empty;
    public string StatusName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int UserCount { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// User data transfer object
/// </summary>
public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public Guid TenantId { get; set; }
    public string TenantName { get; set; } = string.Empty;
    public UserStatus Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime? ActivatedAt { get; set; }
    public DateTime? DeactivatedAt { get; set; }
    public string? DeactivationReason { get; set; }
    public ContactInfo ContactInfo { get; set; } = new();
    public UserPreferences Preferences { get; set; } = new();
    public ICollection<UserRoleDto> Roles { get; set; } = new List<UserRoleDto>();
    public ICollection<PermissionDto> Permissions { get; set; } = new List<PermissionDto>();
    public bool IsActive { get; set; }
    public bool EmailVerified { get; set; }
    public bool HasValidSession { get; set; }
}

/// <summary>
/// User summary data transfer object
/// </summary>
public class UserSummaryDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string StatusName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; }
    public int RoleCount { get; set; }
}

/// <summary>
/// User role data transfer object
/// </summary>
public class UserRoleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public RoleType Type { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public Guid? TenantId { get; set; }
    public string? TenantName { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<PermissionDto> Permissions { get; set; } = new List<PermissionDto>();
    public int UserCount { get; set; }
    public bool IsSystemRole { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// Permission data transfer object
/// </summary>
public class PermissionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Module { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Resource { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsSystemPermission { get; set; }
    public int RoleCount { get; set; }
    public int UserCount { get; set; }
}

/// <summary>
/// System configuration data transfer object
/// </summary>
public class SystemConfigDto
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Category { get; set; }
    public Guid? TenantId { get; set; }
    public string? TenantName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsGlobal { get; set; }
    public bool IsEncrypted { get; set; }
}

/// <summary>
/// Audit log data transfer object
/// </summary>
public class AuditLogDto
{
    public Guid Id { get; set; }
    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public Guid TenantId { get; set; }
    public string TenantName { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? Details { get; set; }
}

/// <summary>
/// Tenant usage statistics data transfer object
/// </summary>
public class TenantUsageStatsDto
{
    public Guid TenantId { get; set; }
    public string TenantName { get; set; } = string.Empty;
    public int UserCount { get; set; }
    public int ActiveUserCount { get; set; }
    public long StorageUsedBytes { get; set; }
    public string StorageUsedFormatted { get; set; } = string.Empty;
    public int ApiCallsThisMonth { get; set; }
    public int ApiCallsLastMonth { get; set; }
    public DateTime LastActivity { get; set; }
    public Dictionary<string, int> FeatureUsage { get; set; } = new();
    public Dictionary<string, int> ModuleUsage { get; set; } = new();
    public decimal UtilizationPercentage { get; set; }
    public SubscriptionPlan SubscriptionPlan { get; set; }
    public string SubscriptionPlanName { get; set; } = string.Empty;
    public bool IsOverLimit { get; set; }
    public List<string> LimitExceeded { get; set; } = new();
}

/// <summary>
/// System health data transfer object
/// </summary>
public class SystemHealthDto
{
    public string Status { get; set; } = string.Empty;
    public DateTime CheckTime { get; set; }
    public Dictionary<string, ComponentHealthDto> Components { get; set; } = new();
    public SystemMetricsDto Metrics { get; set; } = new();
    public List<string> Issues { get; set; } = new();
    public bool IsHealthy { get; set; }
    public TimeSpan Uptime { get; set; }
}

/// <summary>
/// Component health data transfer object
/// </summary>
public class ComponentHealthDto
{
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Message { get; set; }
    public TimeSpan ResponseTime { get; set; }
    public DateTime LastChecked { get; set; }
    public bool IsHealthy { get; set; }
}

/// <summary>
/// System metrics data transfer object
/// </summary>
public class SystemMetricsDto
{
    public int TotalTenants { get; set; }
    public int ActiveTenants { get; set; }
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public long TotalStorageUsed { get; set; }
    public int ApiCallsToday { get; set; }
    public int ApiCallsThisMonth { get; set; }
    public double CpuUsagePercentage { get; set; }
    public double MemoryUsagePercentage { get; set; }
    public double DiskUsagePercentage { get; set; }
    public int DatabaseConnections { get; set; }
    public TimeSpan AverageResponseTime { get; set; }
    public int ErrorsInLast24Hours { get; set; }
}

/// <summary>
/// Tenant dashboard data transfer object
/// </summary>
public class TenantDashboardDto
{
    public TenantDto Tenant { get; set; } = new();
    public TenantUsageStatsDto UsageStats { get; set; } = new();
    public List<UserSummaryDto> RecentUsers { get; set; } = new();
    public List<AuditLogDto> RecentActivity { get; set; } = new();
    public Dictionary<string, int> ModuleUsageStats { get; set; } = new();
    public List<SystemConfigDto> TenantConfigs { get; set; } = new();
    public bool HasPendingInvitations { get; set; }
    public int PendingInvitationCount { get; set; }
    public bool IsTrialExpiringSoon { get; set; }
    public int DaysUntilTrialExpiry { get; set; }
}

/// <summary>
/// System dashboard data transfer object
/// </summary>
public class SystemDashboardDto
{
    public SystemHealthDto Health { get; set; } = new();
    public SystemMetricsDto Metrics { get; set; } = new();
    public List<TenantSummaryDto> RecentTenants { get; set; } = new();
    public List<AuditLogDto> RecentActivity { get; set; } = new();
    public Dictionary<string, int> TenantsByPlan { get; set; } = new();
    public Dictionary<string, int> UsersByStatus { get; set; } = new();
    public List<TenantUsageStatsDto> TopTenantsByUsage { get; set; } = new();
    public List<string> SystemAlerts { get; set; } = new();
    public bool MaintenanceMode { get; set; }
    public DateTime? ScheduledMaintenanceDate { get; set; }
}

/// <summary>
/// User invitation data transfer object
/// </summary>
public class UserInvitationDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Guid TenantId { get; set; }
    public string TenantName { get; set; } = string.Empty;
    public Guid InvitedByUserId { get; set; }
    public string InvitedByUserName { get; set; } = string.Empty;
    public DateTime InvitedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public InvitationStatus Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public List<Guid> RoleIds { get; set; } = new();
    public List<UserRoleDto> Roles { get; set; } = new();
    public string InvitationToken { get; set; } = string.Empty;
    public bool IsExpired { get; set; }
    public bool CanResend { get; set; }
    public int ResendCount { get; set; }
}

/// <summary>
/// Password reset data transfer object
/// </summary>
public class PasswordResetDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public string ResetToken { get; set; } = string.Empty;
    public DateTime RequestedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? UsedAt { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public bool IsExpired { get; set; }
    public bool IsUsed { get; set; }
    public bool IsValid { get; set; }
}
