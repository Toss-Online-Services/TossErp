using Microsoft.Extensions.Logging;
using Setup.Application.Common.Interfaces;
using Setup.Domain.Aggregates.ApplicationConfigurationAggregate;
using Setup.Domain.ValueObjects;

namespace Setup.Infrastructure.Services;

public class SystemConfigService : ISystemConfigService
{
    private readonly ISetupUnitOfWork _unitOfWork;
    private readonly ILogger<SystemConfigService> _logger;

    public SystemConfigService(ISetupUnitOfWork unitOfWork, ILogger<SystemConfigService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<T?> GetConfigValueAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.SystemConfigRepository.GetConfigValueAsync<T>(key, cancellationToken);
    }

    public async Task SetConfigValueAsync<T>(string key, T value, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.SystemConfigRepository.SetConfigValueAsync(key, value, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Updated configuration value: {Key}", key);
    }

    public async Task<bool> IsFeatureEnabledAsync(string featureKey, string? userId = null, string? tenantId = null, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.SystemConfigRepository.IsFeatureEnabledAsync(featureKey, userId, tenantId, cancellationToken);
    }

    public async Task<FeatureFlag> CreateFeatureFlagAsync(string key, string name, string description, 
        bool isEnabled = false, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating feature flag: {Key}", key);

        var featureFlag = new FeatureFlag
        {
            Id = Guid.NewGuid().ToString(),
            Key = key,
            Name = name,
            Description = description,
            IsEnabled = isEnabled,
            EnabledForPercentage = 0,
            CreatedBy = "system",
            CreatedAt = DateTime.UtcNow,
            IsArchived = false,
            EnabledUsers = new List<string>(),
            EnabledRoles = new List<string>(),
            EnabledTenants = new List<string>(),
            Conditions = new Dictionary<string, object>(),
            Variants = new Dictionary<string, object>()
        };

        await _unitOfWork.SystemConfigRepository.AddFeatureFlagAsync(featureFlag, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully created feature flag: {Key}", key);
        return featureFlag;
    }

    public async Task<bool> UpdateFeatureFlagAsync(string key, bool isEnabled, decimal? enabledForPercentage = null, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating feature flag: {Key} to enabled: {IsEnabled}", key, isEnabled);

        var featureFlag = await _unitOfWork.SystemConfigRepository.GetFeatureFlagAsync(key, cancellationToken);
        if (featureFlag == null)
        {
            _logger.LogWarning("Feature flag not found: {Key}", key);
            return false;
        }

        featureFlag.IsEnabled = isEnabled;
        featureFlag.LastModifiedAt = DateTime.UtcNow;
        featureFlag.LastModifiedBy = "system";

        if (enabledForPercentage.HasValue)
        {
            featureFlag.EnabledForPercentage = enabledForPercentage.Value;
        }

        _unitOfWork.SystemConfigRepository.UpdateFeatureFlag(featureFlag);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully updated feature flag: {Key}", key);
        return true;
    }

    public async Task<bool> EnableFeatureForUserAsync(string featureKey, string userId, CancellationToken cancellationToken = default)
    {
        var featureFlag = await _unitOfWork.SystemConfigRepository.GetFeatureFlagAsync(featureKey, cancellationToken);
        if (featureFlag == null) return false;

        if (!featureFlag.EnabledUsers.Contains(userId))
        {
            featureFlag.EnabledUsers.Add(userId);
            _unitOfWork.SystemConfigRepository.UpdateFeatureFlag(featureFlag);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return true;
    }

    public async Task<bool> DisableFeatureForUserAsync(string featureKey, string userId, CancellationToken cancellationToken = default)
    {
        var featureFlag = await _unitOfWork.SystemConfigRepository.GetFeatureFlagAsync(featureKey, cancellationToken);
        if (featureFlag == null) return false;

        if (featureFlag.EnabledUsers.Contains(userId))
        {
            featureFlag.EnabledUsers.Remove(userId);
            _unitOfWork.SystemConfigRepository.UpdateFeatureFlag(featureFlag);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return true;
    }

    public async Task<NotificationTemplate> CreateNotificationTemplateAsync(string name, string type, string subject, 
        string body, bool isHtml = false, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating notification template: {Name}", name);

        var template = new NotificationTemplate
        {
            Id = Guid.NewGuid().ToString(),
            Name = name,
            Type = Enum.Parse<NotificationType>(type),
            Subject = subject,
            Body = body,
            IsHtml = isHtml,
            Language = "en-US",
            IsEnabled = true,
            Priority = NotificationPriority.Normal,
            CreatedBy = "system",
            CreatedAt = DateTime.UtcNow,
            UsageCount = 0,
            Version = 1,
            Variables = new List<string>(),
            Triggers = new List<string>(),
            Conditions = new Dictionary<string, object>(),
            Metadata = new Dictionary<string, object>(),
            NotificationSettings = new NotificationSettings
            {
                EnableEmail = true,
                EnableSms = false,
                EnablePush = false,
                EnableInApp = true,
                DeliveryDelay = 0,
                RetryAttempts = 3,
                RetryInterval = 300,
                ExpiryHours = 24
            }
        };

        await _unitOfWork.SystemConfigRepository.AddNotificationTemplateAsync(template, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully created notification template: {Name}", name);
        return template;
    }

    public async Task<ApiKeyConfiguration> CreateApiKeyAsync(string name, string owner, IEnumerable<string> scopes, 
        DateTime? expiresAt = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating API key: {Name} for owner: {Owner}", name, owner);

        var apiKey = GenerateApiKey();
        var keyHash = HashApiKey(apiKey);

        var apiKeyConfig = new ApiKeyConfiguration
        {
            Id = Guid.NewGuid().ToString(),
            Name = name,
            Description = $"API key for {owner}",
            KeyHash = keyHash,
            KeyPrefix = apiKey.Substring(0, 8),
            IsEnabled = true,
            ExpiresAt = expiresAt,
            Owner = owner,
            CreatedBy = "system",
            CreatedAt = DateTime.UtcNow,
            Environment = Domain.Enums.Environment.Production,
            IsRevoked = false,
            Scopes = scopes.ToList(),
            AllowedIpAddresses = new List<string>(),
            AllowedReferrers = new List<string>(),
            CustomClaims = new Dictionary<string, object>()
        };

        await _unitOfWork.SystemConfigRepository.AddApiKeyConfigAsync(apiKeyConfig, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully created API key: {Name}", name);
        return apiKeyConfig;
    }

    public async Task<bool> RevokeApiKeyAsync(string keyHash, string reason, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Revoking API key with hash: {KeyHash}", keyHash);

        var apiKeyConfig = await _unitOfWork.SystemConfigRepository.GetApiKeyConfigByHashAsync(keyHash, cancellationToken);
        if (apiKeyConfig == null)
        {
            _logger.LogWarning("API key not found: {KeyHash}", keyHash);
            return false;
        }

        apiKeyConfig.IsRevoked = true;
        apiKeyConfig.RevokedAt = DateTime.UtcNow;
        apiKeyConfig.RevokedBy = "system";
        apiKeyConfig.RevokedReason = reason;

        _unitOfWork.SystemConfigRepository.UpdateApiKeyConfig(apiKeyConfig);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully revoked API key: {KeyHash}", keyHash);
        return true;
    }

    public async Task<ModuleConfiguration> RegisterModuleAsync(string moduleName, string displayName, string version,
        bool isEnabled = true, bool isCore = false, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Registering module: {ModuleName}", moduleName);

        var moduleConfig = new ModuleConfiguration
        {
            Id = Guid.NewGuid().ToString(),
            ModuleName = moduleName,
            DisplayName = displayName,
            Version = version,
            IsEnabled = isEnabled,
            IsCore = isCore,
            LoadOrder = 100,
            InstallDate = DateTime.UtcNow,
            Dependencies = new List<string>(),
            Permissions = new List<string>(),
            MenuItems = new List<string>(),
            DatabaseTables = new List<string>(),
            ConfigurationSchema = new Dictionary<string, object>(),
            ConfigurationValue = new ConfigurationValue
            {
                Key = $"{moduleName}:config",
                Value = "{}",
                DataType = ConfigDataType.Json,
                IsEncrypted = false,
                IsReadOnly = false
            }
        };

        await _unitOfWork.SystemConfigRepository.AddModuleConfigAsync(moduleConfig, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully registered module: {ModuleName}", moduleName);
        return moduleConfig;
    }

    public async Task<bool> EnableModuleAsync(string moduleName, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Enabling module: {ModuleName}", moduleName);

        var moduleConfig = await _unitOfWork.SystemConfigRepository.GetModuleConfigAsync(moduleName, cancellationToken);
        if (moduleConfig == null)
        {
            _logger.LogWarning("Module not found: {ModuleName}", moduleName);
            return false;
        }

        moduleConfig.IsEnabled = true;
        moduleConfig.LastUpdateDate = DateTime.UtcNow;

        _unitOfWork.SystemConfigRepository.UpdateModuleConfig(moduleConfig);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully enabled module: {ModuleName}", moduleName);
        return true;
    }

    public async Task<bool> DisableModuleAsync(string moduleName, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Disabling module: {ModuleName}", moduleName);

        var moduleConfig = await _unitOfWork.SystemConfigRepository.GetModuleConfigAsync(moduleName, cancellationToken);
        if (moduleConfig == null)
        {
            _logger.LogWarning("Module not found: {ModuleName}", moduleName);
            return false;
        }

        if (moduleConfig.IsCore)
        {
            _logger.LogWarning("Cannot disable core module: {ModuleName}", moduleName);
            return false;
        }

        moduleConfig.IsEnabled = false;
        moduleConfig.LastUpdateDate = DateTime.UtcNow;

        _unitOfWork.SystemConfigRepository.UpdateModuleConfig(moduleConfig);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully disabled module: {ModuleName}", moduleName);
        return true;
    }

    public async Task<RateLimitRule> CreateRateLimitRuleAsync(string name, string endpoint, int requestsPerWindow,
        int windowSizeSeconds, string httpMethod = "*", CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating rate limit rule: {Name} for endpoint: {Endpoint}", name, endpoint);

        var rateLimitRule = new RateLimitRule
        {
            Id = Guid.NewGuid().ToString(),
            Name = name,
            Description = $"Rate limit for {endpoint}",
            Endpoint = endpoint,
            HttpMethod = httpMethod,
            ClientType = ClientType.All,
            IsEnabled = true,
            Priority = 100,
            QuotaExceededMessage = "Rate limit exceeded. Please try again later.",
            CreatedBy = "system",
            CreatedAt = DateTime.UtcNow,
            RateLimit = new RateLimit
            {
                RequestsPerWindow = requestsPerWindow,
                WindowSizeSeconds = windowSizeSeconds,
                BurstLimit = requestsPerWindow * 2,
                TimeoutSeconds = 30,
                ConcurrentRequestLimit = 10,
                SlidingWindow = true,
                ResetMode = RateLimitResetMode.Sliding
            },
            ClientIdentifiers = new List<string>(),
            ExcludedEndpoints = new List<string>(),
            WhitelistedIpAddresses = new List<string>(),
            BlacklistedIpAddresses = new List<string>(),
            Conditions = new Dictionary<string, object>()
        };

        await _unitOfWork.SystemConfigRepository.AddRateLimitRuleAsync(rateLimitRule, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully created rate limit rule: {Name}", name);
        return rateLimitRule;
    }

    public async Task<BackupConfiguration> CreateBackupConfigurationAsync(string name, string backupType, string schedule,
        string storageLocation, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating backup configuration: {Name}", name);

        var backupConfig = new BackupConfiguration
        {
            Id = Guid.NewGuid().ToString(),
            Name = name,
            Description = $"Backup configuration for {backupType}",
            BackupType = Enum.Parse<BackupType>(backupType),
            Schedule = schedule,
            IsEnabled = true,
            StorageLocation = storageLocation,
            CompressionLevel = 5,
            CreatedBy = "system",
            CreatedAt = DateTime.UtcNow,
            BackupRetention = new BackupRetention
            {
                DailyRetentionDays = 7,
                WeeklyRetentionWeeks = 4,
                MonthlyRetentionMonths = 12,
                YearlyRetentionYears = 3,
                MaxTotalBackups = 100,
                AutoPurge = true,
                ArchiveAfterDays = 30
            },
            IncludedDatabases = new List<string>(),
            ExcludedTables = new List<string>(),
            NotificationRecipients = new List<string>(),
            PreBackupScripts = new List<string>(),
            PostBackupScripts = new List<string>(),
            AdvancedOptions = new Dictionary<string, object>()
        };

        await _unitOfWork.SystemConfigRepository.AddBackupConfigAsync(backupConfig, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully created backup configuration: {Name}", name);
        return backupConfig;
    }

    public async Task<SystemMetrics> GetSystemMetricsAsync(CancellationToken cancellationToken = default)
    {
        var enabledModules = await _unitOfWork.SystemConfigRepository.GetEnabledModulesAsync(cancellationToken);
        var enabledFeatures = await _unitOfWork.SystemConfigRepository.GetEnabledFeatureFlagsAsync(cancellationToken);
        var activeApiKeys = await _unitOfWork.SystemConfigRepository.GetActiveApiKeysAsync(cancellationToken);
        var enabledRateLimits = await _unitOfWork.SystemConfigRepository.GetEnabledRateLimitRulesAsync(cancellationToken);
        var enabledBackups = await _unitOfWork.SystemConfigRepository.GetEnabledBackupConfigsAsync(cancellationToken);

        return new SystemMetrics
        {
            EnabledModulesCount = enabledModules.Count(),
            EnabledFeatureFlagsCount = enabledFeatures.Count(),
            ActiveApiKeysCount = activeApiKeys.Count(),
            RateLimitRulesCount = enabledRateLimits.Count(),
            BackupConfigurationsCount = enabledBackups.Count(),
            LastBackupDate = enabledBackups.Where(b => b.LastBackupAt.HasValue).MaxBy(b => b.LastBackupAt)?.LastBackupAt
        };
    }

    private static string GenerateApiKey()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        var key = new string(Enumerable.Repeat(chars, 32)
            .Select(s => s[random.Next(s.Length)]).ToArray());
        return $"toss_{key}";
    }

    private static string HashApiKey(string apiKey)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(apiKey));
        return Convert.ToBase64String(hashedBytes);
    }
}

public class SystemMetrics
{
    public int EnabledModulesCount { get; set; }
    public int EnabledFeatureFlagsCount { get; set; }
    public int ActiveApiKeysCount { get; set; }
    public int RateLimitRulesCount { get; set; }
    public int BackupConfigurationsCount { get; set; }
    public DateTime? LastBackupDate { get; set; }
}
