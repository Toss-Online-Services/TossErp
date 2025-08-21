using TossErp.Setup.Domain.Enums;
using TossErp.Setup.Domain.SeedWork;
using TossErp.Setup.Domain.ValueObjects;

namespace TossErp.Setup.Domain.Entities;

/// <summary>
/// System configuration entity for tenant-specific settings
/// </summary>
public class SystemConfiguration : Entity
{
    public TenantId TenantId { get; private set; }
    public List<ConfigurationValue> Settings { get; private set; }
    public Dictionary<FeatureFlag, FeatureConfiguration> Features { get; private set; }
    public DateTime LastModified { get; private set; }
    public string ModifiedBy { get; private set; }

    private SystemConfiguration()
    {
        TenantId = null!;
        Settings = null!;
        Features = null!;
        ModifiedBy = null!;
    } // EF Core

    public SystemConfiguration(TenantId tenantId, string modifiedBy)
    {
        TenantId = tenantId;
        Settings = new List<ConfigurationValue>();
        Features = new Dictionary<FeatureFlag, FeatureConfiguration>();
        LastModified = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void UpdateSetting(ConfigurationValue setting, string modifiedBy)
    {
        var existingIndex = Settings.FindIndex(s => s.Key == setting.Key);
        if (existingIndex >= 0)
        {
            Settings[existingIndex] = setting;
        }
        else
        {
            Settings.Add(setting);
        }
        
        LastModified = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void RemoveSetting(string key, string modifiedBy)
    {
        Settings.RemoveAll(s => s.Key == key);
        LastModified = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public ConfigurationValue? GetSetting(string key)
    {
        return Settings.FirstOrDefault(s => s.Key == key);
    }

    public void UpdateFeature(FeatureConfiguration feature, string modifiedBy)
    {
        Features[feature.Feature] = feature;
        LastModified = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public bool IsFeatureEnabled(FeatureFlag feature)
    {
        return Features.TryGetValue(feature, out var config) && config.IsEnabled;
    }

    public FeatureConfiguration? GetFeatureConfiguration(FeatureFlag feature)
    {
        return Features.TryGetValue(feature, out var config) ? config : null;
    }
}

/// <summary>
/// User profile entity for user-specific information
/// </summary>
public class UserProfile : Entity
{
    public TenantId TenantId { get; private set; }
    public string UserId { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public string? Phone { get; private set; }
    public string? AvatarUrl { get; private set; }
    public UserRole Role { get; private set; }
    public UserStatus Status { get; private set; }
    public Language PreferredLanguage { get; private set; }
    public TimeZone TimeZone { get; private set; }
    public List<PermissionType> Permissions { get; private set; }
    public Dictionary<string, string> Preferences { get; private set; }
    public DateTime CreatedDate { get; private set; }
    public DateTime LastLoginDate { get; private set; }
    public DateTime? LastPasswordChange { get; private set; }
    public bool IsEmailVerified { get; private set; }
    public bool IsPhoneVerified { get; private set; }
    public bool RequiresPasswordReset { get; private set; }

    private UserProfile()
    {
        TenantId = null!;
        UserId = null!;
        FirstName = null!;
        LastName = null!;
        Email = null!;
        Permissions = null!;
        Preferences = null!;
    } // EF Core

    public UserProfile(
        TenantId tenantId,
        string userId,
        string firstName,
        string lastName,
        string email,
        UserRole role,
        Language preferredLanguage = Language.English,
        TimeZone timeZone = TimeZone.UTC)
    {
        TenantId = tenantId;
        UserId = userId?.Trim() ?? throw new ArgumentException("UserId cannot be empty");
        FirstName = firstName?.Trim() ?? throw new ArgumentException("FirstName cannot be empty");
        LastName = lastName?.Trim() ?? throw new ArgumentException("LastName cannot be empty");
        Email = email?.Trim().ToLower() ?? throw new ArgumentException("Email cannot be empty");
        Role = role;
        Status = UserStatus.Active;
        PreferredLanguage = preferredLanguage;
        TimeZone = timeZone;
        Permissions = new List<PermissionType>();
        Preferences = new Dictionary<string, string>();
        CreatedDate = DateTime.UtcNow;
        LastLoginDate = DateTime.MinValue;
        IsEmailVerified = false;
        IsPhoneVerified = false;
        RequiresPasswordReset = false;
    }

    public void UpdateBasicInfo(string firstName, string lastName, string? phone = null)
    {
        FirstName = firstName?.Trim() ?? throw new ArgumentException("FirstName cannot be empty");
        LastName = lastName?.Trim() ?? throw new ArgumentException("LastName cannot be empty");
        Phone = phone?.Trim();
    }

    public void UpdateRole(UserRole role, List<PermissionType> permissions)
    {
        Role = role;
        Permissions = permissions ?? new List<PermissionType>();
    }

    public void UpdateStatus(UserStatus status)
    {
        Status = status;
    }

    public void MarkEmailVerified()
    {
        IsEmailVerified = true;
    }

    public void MarkPhoneVerified()
    {
        IsPhoneVerified = true;
    }

    public void RecordLogin()
    {
        LastLoginDate = DateTime.UtcNow;
        if (RequiresPasswordReset)
        {
            RequiresPasswordReset = false;
        }
    }

    public void RequirePasswordReset()
    {
        RequiresPasswordReset = true;
        LastPasswordChange = DateTime.UtcNow;
    }

    public void UpdatePreferences(Dictionary<string, string> preferences)
    {
        Preferences = preferences ?? new Dictionary<string, string>();
    }

    public void SetPreference(string key, string value)
    {
        Preferences[key] = value;
    }

    public string? GetPreference(string key)
    {
        return Preferences.TryGetValue(key, out var value) ? value : null;
    }

    public bool HasPermission(PermissionType permission)
    {
        return Permissions.Contains(permission);
    }

    public string FullName => $"{FirstName} {LastName}";
    public bool IsActive => Status == UserStatus.Active;
    public bool CanLogin => IsActive && IsEmailVerified && !RequiresPasswordReset;
}

/// <summary>
/// Integration configuration entity
/// </summary>
public class IntegrationConfiguration : Entity
{
    public TenantId TenantId { get; private set; }
    public string Name { get; private set; }
    public IntegrationConfig Config { get; private set; }
    public List<string> EnabledModules { get; private set; }
    public Dictionary<string, string> MappingRules { get; private set; }
    public IntegrationStatus Status { get; private set; }
    public DateTime CreatedDate { get; private set; }
    public DateTime LastTested { get; private set; }
    public string? LastError { get; private set; }
    public int FailureCount { get; private set; }
    public DateTime? LastSuccessfulSync { get; private set; }

    private IntegrationConfiguration()
    {
        TenantId = null!;
        Name = null!;
        Config = null!;
        EnabledModules = null!;
        MappingRules = null!;
    } // EF Core

    public IntegrationConfiguration(TenantId tenantId, string name, IntegrationConfig config)
    {
        TenantId = tenantId;
        Name = name?.Trim() ?? throw new ArgumentException("Name cannot be empty");
        Config = config;
        EnabledModules = new List<string>();
        MappingRules = new Dictionary<string, string>();
        Status = IntegrationStatus.Configured;
        CreatedDate = DateTime.UtcNow;
        LastTested = DateTime.UtcNow;
        FailureCount = 0;
    }

    public void UpdateConfig(IntegrationConfig config)
    {
        Config = config;
        Status = IntegrationStatus.Configured;
        LastTested = DateTime.UtcNow;
    }

    public void EnableModule(string moduleName)
    {
        if (!EnabledModules.Contains(moduleName))
        {
            EnabledModules.Add(moduleName);
        }
    }

    public void DisableModule(string moduleName)
    {
        EnabledModules.Remove(moduleName);
    }

    public void UpdateMappingRule(string sourceField, string targetField)
    {
        MappingRules[sourceField] = targetField;
    }

    public void RecordTestResult(bool success, string? error = null)
    {
        LastTested = DateTime.UtcNow;
        if (success)
        {
            Status = IntegrationStatus.Active;
            FailureCount = 0;
            LastError = null;
        }
        else
        {
            Status = IntegrationStatus.Error;
            FailureCount++;
            LastError = error;
        }
    }

    public void RecordSuccessfulSync()
    {
        LastSuccessfulSync = DateTime.UtcNow;
        Status = IntegrationStatus.Active;
        FailureCount = 0;
        LastError = null;
    }

    public bool IsModuleEnabled(string moduleName)
    {
        return EnabledModules.Contains(moduleName);
    }

    public string? GetMapping(string sourceField)
    {
        return MappingRules.TryGetValue(sourceField, out var targetField) ? targetField : null;
    }

    public bool IsHealthy => Status == IntegrationStatus.Active && FailureCount < 3;
    public bool RequiresAttention => FailureCount >= 3 || Status == IntegrationStatus.Error;
}

/// <summary>
/// Audit log entry entity
/// </summary>
public class AuditLogEntry : Entity
{
    public TenantId TenantId { get; private set; }
    public AuditEventType EventType { get; private set; }
    public string EntityType { get; private set; }
    public string EntityId { get; private set; }
    public string UserId { get; private set; }
    public string UserName { get; private set; }
    public string Action { get; private set; }
    public string? OldValues { get; private set; }
    public string? NewValues { get; private set; }
    public Dictionary<string, string> Metadata { get; private set; }
    public DateTime Timestamp { get; private set; }
    public string IpAddress { get; private set; }
    public string UserAgent { get; private set; }
    public AuditSeverity Severity { get; private set; }

    private AuditLogEntry()
    {
        TenantId = null!;
        EntityType = null!;
        EntityId = null!;
        UserId = null!;
        UserName = null!;
        Action = null!;
        Metadata = null!;
        IpAddress = null!;
        UserAgent = null!;
    } // EF Core

    public AuditLogEntry(
        TenantId tenantId,
        AuditEventType eventType,
        string entityType,
        string entityId,
        string userId,
        string userName,
        string action,
        string ipAddress,
        string userAgent,
        AuditSeverity severity = AuditSeverity.Info)
    {
        TenantId = tenantId;
        EventType = eventType;
        EntityType = entityType?.Trim() ?? throw new ArgumentException("EntityType cannot be empty");
        EntityId = entityId?.Trim() ?? throw new ArgumentException("EntityId cannot be empty");
        UserId = userId?.Trim() ?? throw new ArgumentException("UserId cannot be empty");
        UserName = userName?.Trim() ?? throw new ArgumentException("UserName cannot be empty");
        Action = action?.Trim() ?? throw new ArgumentException("Action cannot be empty");
        IpAddress = ipAddress?.Trim() ?? throw new ArgumentException("IpAddress cannot be empty");
        UserAgent = userAgent?.Trim() ?? throw new ArgumentException("UserAgent cannot be empty");
        Severity = severity;
        Metadata = new Dictionary<string, string>();
        Timestamp = DateTime.UtcNow;
    }

    public void AddMetadata(string key, string value)
    {
        Metadata[key] = value;
    }

    public void SetDataChanges(string? oldValues, string? newValues)
    {
        OldValues = oldValues;
        NewValues = newValues;
    }

    public bool IsSecurityEvent => EventType == AuditEventType.SecurityEvent || 
                                  EventType == AuditEventType.LoginAttempt ||
                                  EventType == AuditEventType.PermissionChange;

    public bool IsHighSeverity => Severity == AuditSeverity.Warning || Severity == AuditSeverity.Error;
}

/// <summary>
/// Notification template entity
/// </summary>
public class NotificationTemplate : Entity
{
    public TenantId TenantId { get; private set; }
    public NotificationType Type { get; private set; }
    public string Name { get; private set; }
    public string Subject { get; private set; }
    public string Body { get; private set; }
    public NotificationChannel Channel { get; private set; }
    public Language Language { get; private set; }
    public Dictionary<string, string> Variables { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedDate { get; private set; }
    public DateTime? LastModified { get; private set; }
    public string CreatedBy { get; private set; }

    private NotificationTemplate()
    {
        TenantId = null!;
        Name = null!;
        Subject = null!;
        Body = null!;
        Variables = null!;
        CreatedBy = null!;
    } // EF Core

    public NotificationTemplate(
        TenantId tenantId,
        NotificationType type,
        string name,
        string subject,
        string body,
        NotificationChannel channel,
        string createdBy,
        Language language = Language.English)
    {
        TenantId = tenantId;
        Type = type;
        Name = name?.Trim() ?? throw new ArgumentException("Name cannot be empty");
        Subject = subject?.Trim() ?? throw new ArgumentException("Subject cannot be empty");
        Body = body?.Trim() ?? throw new ArgumentException("Body cannot be empty");
        Channel = channel;
        Language = language;
        CreatedBy = createdBy?.Trim() ?? throw new ArgumentException("CreatedBy cannot be empty");
        Variables = new Dictionary<string, string>();
        IsActive = true;
        CreatedDate = DateTime.UtcNow;
    }

    public void UpdateContent(string subject, string body, string modifiedBy)
    {
        Subject = subject?.Trim() ?? throw new ArgumentException("Subject cannot be empty");
        Body = body?.Trim() ?? throw new ArgumentException("Body cannot be empty");
        LastModified = DateTime.UtcNow;
    }

    public void AddVariable(string name, string description)
    {
        Variables[name] = description;
    }

    public void RemoveVariable(string name)
    {
        Variables.Remove(name);
    }

    public void Activate()
    {
        IsActive = true;
        LastModified = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        LastModified = DateTime.UtcNow;
    }

    public string ProcessTemplate(Dictionary<string, string> values)
    {
        var processedBody = Body;
        foreach (var variable in Variables.Keys)
        {
            if (values.TryGetValue(variable, out var value))
            {
                processedBody = processedBody.Replace($"{{{variable}}}", value);
            }
        }
        return processedBody;
    }

    public List<string> GetMissingVariables(Dictionary<string, string> values)
    {
        return Variables.Keys.Where(variable => !values.ContainsKey(variable)).ToList();
    }
}

/// <summary>
/// API key entity for external integrations
/// </summary>
public class ApiKey : Entity
{
    public TenantId TenantId { get; private set; }
    public string Name { get; private set; }
    public string KeyHash { get; private set; }
    public string KeyPrefix { get; private set; }
    public List<PermissionType> Permissions { get; private set; }
    public List<string> AllowedIpRanges { get; private set; }
    public RateLimitConfig? RateLimit { get; private set; }
    public DateTime CreatedDate { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    public DateTime? LastUsed { get; private set; }
    public long UsageCount { get; private set; }
    public bool IsActive { get; private set; }
    public string CreatedBy { get; private set; }

    private ApiKey()
    {
        TenantId = null!;
        Name = null!;
        KeyHash = null!;
        KeyPrefix = null!;
        Permissions = null!;
        AllowedIpRanges = null!;
        CreatedBy = null!;
    } // EF Core

    public ApiKey(
        TenantId tenantId,
        string name,
        string keyHash,
        string keyPrefix,
        List<PermissionType> permissions,
        string createdBy,
        DateTime? expiryDate = null)
    {
        TenantId = tenantId;
        Name = name?.Trim() ?? throw new ArgumentException("Name cannot be empty");
        KeyHash = keyHash?.Trim() ?? throw new ArgumentException("KeyHash cannot be empty");
        KeyPrefix = keyPrefix?.Trim() ?? throw new ArgumentException("KeyPrefix cannot be empty");
        Permissions = permissions ?? new List<PermissionType>();
        AllowedIpRanges = new List<string>();
        CreatedBy = createdBy?.Trim() ?? throw new ArgumentException("CreatedBy cannot be empty");
        CreatedDate = DateTime.UtcNow;
        ExpiryDate = expiryDate;
        IsActive = true;
        UsageCount = 0;
    }

    public void AddIpRange(string ipRange)
    {
        if (!AllowedIpRanges.Contains(ipRange))
        {
            AllowedIpRanges.Add(ipRange);
        }
    }

    public void RemoveIpRange(string ipRange)
    {
        AllowedIpRanges.Remove(ipRange);
    }

    public void SetRateLimit(RateLimitConfig rateLimit)
    {
        RateLimit = rateLimit;
    }

    public void RecordUsage()
    {
        LastUsed = DateTime.UtcNow;
        UsageCount++;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public bool HasPermission(PermissionType permission)
    {
        return Permissions.Contains(permission);
    }

    public bool IsValidForIp(string ipAddress)
    {
        return !AllowedIpRanges.Any() || AllowedIpRanges.Any(range => IsIpInRange(ipAddress, range));
    }

    public bool IsExpired => ExpiryDate.HasValue && ExpiryDate.Value < DateTime.UtcNow;
    public bool CanBeUsed => IsActive && !IsExpired;

    private static bool IsIpInRange(string ipAddress, string ipRange)
    {
        // Simplified IP range checking - in real implementation would use proper CIDR logic
        return ipRange == "*" || ipRange == ipAddress || ipAddress.StartsWith(ipRange.TrimEnd('*'));
    }
}
