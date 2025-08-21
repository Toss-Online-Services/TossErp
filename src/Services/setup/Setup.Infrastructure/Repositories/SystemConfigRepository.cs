using Microsoft.EntityFrameworkCore;
using Setup.Application.Common.Interfaces;
using Setup.Domain.Aggregates.ApplicationConfigurationAggregate;
using Setup.Infrastructure.Data;

namespace Setup.Infrastructure.Repositories;

public class SystemConfigRepository : ISystemConfigRepository
{
    private readonly SetupDbContext _context;

    public SystemConfigRepository(SetupDbContext context)
    {
        _context = context;
    }

    // Application Configuration methods
    public async Task<ApplicationConfiguration?> GetApplicationConfigAsync(string applicationName, string environment, string instanceName, CancellationToken cancellationToken = default)
    {
        return await _context.ApplicationConfigurations
            .Include(ac => ac.ModuleConfigurations)
            .Include(ac => ac.FeatureFlags)
            .Include(ac => ac.NotificationTemplates)
            .Include(ac => ac.ApiKeyConfigurations)
            .Include(ac => ac.RateLimitRules)
            .Include(ac => ac.BackupConfigurations)
            .FirstOrDefaultAsync(ac => ac.ApplicationName == applicationName && 
                                      ac.Environment.ToString() == environment && 
                                      ac.InstanceName == instanceName, cancellationToken);
    }

    public async Task<ApplicationConfiguration?> GetApplicationConfigByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _context.ApplicationConfigurations
            .Include(ac => ac.ModuleConfigurations)
            .Include(ac => ac.FeatureFlags)
            .Include(ac => ac.NotificationTemplates)
            .Include(ac => ac.ApiKeyConfigurations)
            .Include(ac => ac.RateLimitRules)
            .Include(ac => ac.BackupConfigurations)
            .FirstOrDefaultAsync(ac => ac.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<ApplicationConfiguration>> GetAllApplicationConfigsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ApplicationConfigurations
            .Include(ac => ac.ModuleConfigurations)
            .Include(ac => ac.FeatureFlags)
            .OrderBy(ac => ac.ApplicationName)
            .ThenBy(ac => ac.Environment)
            .ToListAsync(cancellationToken);
    }

    public async Task AddApplicationConfigAsync(ApplicationConfiguration config, CancellationToken cancellationToken = default)
    {
        await _context.ApplicationConfigurations.AddAsync(config, cancellationToken);
    }

    public void UpdateApplicationConfig(ApplicationConfiguration config)
    {
        _context.ApplicationConfigurations.Update(config);
    }

    public void RemoveApplicationConfig(ApplicationConfiguration config)
    {
        _context.ApplicationConfigurations.Remove(config);
    }

    // Module Configuration methods
    public async Task<ModuleConfiguration?> GetModuleConfigAsync(string moduleName, CancellationToken cancellationToken = default)
    {
        return await _context.ModuleConfigurations
            .FirstOrDefaultAsync(mc => mc.ModuleName == moduleName, cancellationToken);
    }

    public async Task<IEnumerable<ModuleConfiguration>> GetEnabledModulesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ModuleConfigurations
            .Where(mc => mc.IsEnabled)
            .OrderBy(mc => mc.LoadOrder)
            .ThenBy(mc => mc.ModuleName)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ModuleConfiguration>> GetModulesByCategoryAsync(string category, CancellationToken cancellationToken = default)
    {
        return await _context.ModuleConfigurations
            .Where(mc => mc.Category == category)
            .OrderBy(mc => mc.DisplayName)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ModuleConfiguration>> GetCoreModulesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ModuleConfigurations
            .Where(mc => mc.IsCore)
            .OrderBy(mc => mc.LoadOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task AddModuleConfigAsync(ModuleConfiguration moduleConfig, CancellationToken cancellationToken = default)
    {
        await _context.ModuleConfigurations.AddAsync(moduleConfig, cancellationToken);
    }

    public void UpdateModuleConfig(ModuleConfiguration moduleConfig)
    {
        _context.ModuleConfigurations.Update(moduleConfig);
    }

    public void RemoveModuleConfig(ModuleConfiguration moduleConfig)
    {
        _context.ModuleConfigurations.Remove(moduleConfig);
    }

    // Feature Flag methods
    public async Task<FeatureFlag?> GetFeatureFlagAsync(string key, CancellationToken cancellationToken = default)
    {
        return await _context.FeatureFlags
            .FirstOrDefaultAsync(ff => ff.Key == key, cancellationToken);
    }

    public async Task<IEnumerable<FeatureFlag>> GetEnabledFeatureFlagsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.FeatureFlags
            .Where(ff => ff.IsEnabled && !ff.IsArchived)
            .OrderBy(ff => ff.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<FeatureFlag>> GetFeatureFlagsByEnvironmentAsync(string environment, CancellationToken cancellationToken = default)
    {
        return await _context.FeatureFlags
            .Where(ff => ff.Environment.ToString() == environment || ff.Environment == null)
            .OrderBy(ff => ff.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<FeatureFlag>> GetFeatureFlagsByOwnerAsync(string owner, CancellationToken cancellationToken = default)
    {
        return await _context.FeatureFlags
            .Where(ff => ff.Owner == owner)
            .OrderBy(ff => ff.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsFeatureEnabledAsync(string key, string? userId = null, string? tenantId = null, CancellationToken cancellationToken = default)
    {
        var feature = await GetFeatureFlagAsync(key, cancellationToken);
        if (feature == null || !feature.IsEnabled || feature.IsArchived)
            return false;

        // Check date range
        var now = DateTime.UtcNow;
        if (feature.StartDate.HasValue && now < feature.StartDate.Value)
            return false;
        if (feature.EndDate.HasValue && now > feature.EndDate.Value)
            return false;

        // Check user-specific enablement
        if (!string.IsNullOrEmpty(userId) && feature.EnabledUsers.Contains(userId))
            return true;

        // Check tenant-specific enablement
        if (!string.IsNullOrEmpty(tenantId) && feature.EnabledTenants.Contains(tenantId))
            return true;

        // Check percentage rollout
        if (feature.EnabledForPercentage > 0)
        {
            var hash = Math.Abs(key.GetHashCode() + (userId ?? tenantId ?? "").GetHashCode());
            var percentage = hash % 100;
            return percentage < feature.EnabledForPercentage;
        }

        return feature.IsEnabled;
    }

    public async Task AddFeatureFlagAsync(FeatureFlag featureFlag, CancellationToken cancellationToken = default)
    {
        await _context.FeatureFlags.AddAsync(featureFlag, cancellationToken);
    }

    public void UpdateFeatureFlag(FeatureFlag featureFlag)
    {
        _context.FeatureFlags.Update(featureFlag);
    }

    public void RemoveFeatureFlag(FeatureFlag featureFlag)
    {
        _context.FeatureFlags.Remove(featureFlag);
    }

    // Notification Template methods
    public async Task<NotificationTemplate?> GetNotificationTemplateAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.NotificationTemplates
            .FirstOrDefaultAsync(nt => nt.Name == name, cancellationToken);
    }

    public async Task<IEnumerable<NotificationTemplate>> GetNotificationTemplatesByTypeAsync(string type, CancellationToken cancellationToken = default)
    {
        return await _context.NotificationTemplates
            .Where(nt => nt.Type.ToString() == type && nt.IsEnabled)
            .OrderBy(nt => nt.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<NotificationTemplate>> GetNotificationTemplatesByCategoryAsync(string category, CancellationToken cancellationToken = default)
    {
        return await _context.NotificationTemplates
            .Where(nt => nt.Category == category && nt.IsEnabled)
            .OrderBy(nt => nt.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task AddNotificationTemplateAsync(NotificationTemplate template, CancellationToken cancellationToken = default)
    {
        await _context.NotificationTemplates.AddAsync(template, cancellationToken);
    }

    public void UpdateNotificationTemplate(NotificationTemplate template)
    {
        _context.NotificationTemplates.Update(template);
    }

    public void RemoveNotificationTemplate(NotificationTemplate template)
    {
        _context.NotificationTemplates.Remove(template);
    }

    // API Key Configuration methods
    public async Task<ApiKeyConfiguration?> GetApiKeyConfigByHashAsync(string keyHash, CancellationToken cancellationToken = default)
    {
        return await _context.ApiKeyConfigurations
            .FirstOrDefaultAsync(ak => ak.KeyHash == keyHash && ak.IsEnabled && !ak.IsRevoked, cancellationToken);
    }

    public async Task<IEnumerable<ApiKeyConfiguration>> GetApiKeyConfigsByOwnerAsync(string owner, CancellationToken cancellationToken = default)
    {
        return await _context.ApiKeyConfigurations
            .Where(ak => ak.Owner == owner)
            .OrderByDescending(ak => ak.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ApiKeyConfiguration>> GetActiveApiKeysAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _context.ApiKeyConfigurations
            .Where(ak => ak.IsEnabled && 
                        !ak.IsRevoked && 
                        (ak.ExpiresAt == null || ak.ExpiresAt > now))
            .OrderBy(ak => ak.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task AddApiKeyConfigAsync(ApiKeyConfiguration apiKeyConfig, CancellationToken cancellationToken = default)
    {
        await _context.ApiKeyConfigurations.AddAsync(apiKeyConfig, cancellationToken);
    }

    public void UpdateApiKeyConfig(ApiKeyConfiguration apiKeyConfig)
    {
        _context.ApiKeyConfigurations.Update(apiKeyConfig);
    }

    public void RemoveApiKeyConfig(ApiKeyConfiguration apiKeyConfig)
    {
        _context.ApiKeyConfigurations.Remove(apiKeyConfig);
    }

    // Rate Limit Rule methods
    public async Task<IEnumerable<RateLimitRule>> GetRateLimitRulesForEndpointAsync(string endpoint, string httpMethod, CancellationToken cancellationToken = default)
    {
        return await _context.RateLimitRules
            .Where(rl => rl.IsEnabled && 
                        (rl.Endpoint == endpoint || rl.Endpoint == "*") &&
                        (rl.HttpMethod == httpMethod || rl.HttpMethod == "*"))
            .OrderBy(rl => rl.Priority)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<RateLimitRule>> GetEnabledRateLimitRulesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.RateLimitRules
            .Where(rl => rl.IsEnabled)
            .OrderBy(rl => rl.Priority)
            .ToListAsync(cancellationToken);
    }

    public async Task AddRateLimitRuleAsync(RateLimitRule rateLimitRule, CancellationToken cancellationToken = default)
    {
        await _context.RateLimitRules.AddAsync(rateLimitRule, cancellationToken);
    }

    public void UpdateRateLimitRule(RateLimitRule rateLimitRule)
    {
        _context.RateLimitRules.Update(rateLimitRule);
    }

    public void RemoveRateLimitRule(RateLimitRule rateLimitRule)
    {
        _context.RateLimitRules.Remove(rateLimitRule);
    }

    // Backup Configuration methods
    public async Task<IEnumerable<BackupConfiguration>> GetEnabledBackupConfigsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.BackupConfigurations
            .Where(bc => bc.IsEnabled)
            .OrderBy(bc => bc.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<BackupConfiguration>> GetBackupConfigsDueAsync(DateTime beforeDate, CancellationToken cancellationToken = default)
    {
        return await _context.BackupConfigurations
            .Where(bc => bc.IsEnabled && bc.NextBackupAt <= beforeDate)
            .OrderBy(bc => bc.NextBackupAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddBackupConfigAsync(BackupConfiguration backupConfig, CancellationToken cancellationToken = default)
    {
        await _context.BackupConfigurations.AddAsync(backupConfig, cancellationToken);
    }

    public void UpdateBackupConfig(BackupConfiguration backupConfig)
    {
        _context.BackupConfigurations.Update(backupConfig);
    }

    public void RemoveBackupConfig(BackupConfiguration backupConfig)
    {
        _context.BackupConfigurations.Remove(backupConfig);
    }

    // Generic configuration value methods
    public async Task<T?> GetConfigValueAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        // This would typically look in a generic configuration store
        // For now, we'll implement it using custom settings in ApplicationConfiguration
        var appConfig = await _context.ApplicationConfigurations
            .FirstOrDefaultAsync(cancellationToken);

        if (appConfig?.CustomSettings?.ContainsKey(key) == true)
        {
            var value = appConfig.CustomSettings[key];
            if (value is T typedValue)
                return typedValue;
            
            // Try to convert
            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return default(T);
            }
        }

        return default(T);
    }

    public async Task SetConfigValueAsync<T>(string key, T value, CancellationToken cancellationToken = default)
    {
        var appConfig = await _context.ApplicationConfigurations
            .FirstOrDefaultAsync(cancellationToken);

        if (appConfig != null)
        {
            appConfig.CustomSettings ??= new Dictionary<string, object>();
            appConfig.CustomSettings[key] = value!;
            appConfig.LastModifiedAt = DateTime.UtcNow;
            UpdateApplicationConfig(appConfig);
        }
    }
}
