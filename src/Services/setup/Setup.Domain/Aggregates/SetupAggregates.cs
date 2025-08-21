using TossErp.Setup.Domain.Entities;
using TossErp.Setup.Domain.Enums;
using TossErp.Setup.Domain.Events;
using TossErp.Setup.Domain.SeedWork;
using TossErp.Setup.Domain.ValueObjects;

namespace TossErp.Setup.Domain.Aggregates;

/// <summary>
/// Tenant aggregate root for multi-tenant SaaS management
/// </summary>
public class Tenant : AggregateRoot
{
    public TenantId TenantId { get; private set; }
    public string Name { get; private set; }
    public string DisplayName { get; private set; }
    public OrganizationType OrganizationType { get; private set; }
    public IndustryType Industry { get; private set; }
    public TenantStatus Status { get; private set; }
    public ContactInfo ContactInfo { get; private set; }
    public SubscriptionDetails Subscription { get; private set; }
    public SystemConfiguration Configuration { get; private set; }
    public List<UserProfile> Users { get; private set; }
    public List<IntegrationConfiguration> Integrations { get; private set; }
    public AuditConfig AuditConfiguration { get; private set; }
    public DateTime CreatedDate { get; private set; }
    public DateTime? ActivatedDate { get; private set; }
    public DateTime? SuspendedDate { get; private set; }
    public string? SuspensionReason { get; private set; }
    public Dictionary<string, string> Metadata { get; private set; }
    public long StorageUsed { get; private set; } // In bytes
    public int ActiveUsers { get; private set; }
    public DateTime LastActivityDate { get; private set; }

    private Tenant()
    {
        TenantId = null!;
        Name = null!;
        DisplayName = null!;
        ContactInfo = null!;
        Subscription = null!;
        Configuration = null!;
        Users = null!;
        Integrations = null!;
        AuditConfiguration = null!;
        Metadata = null!;
    } // EF Core

    public Tenant(
        TenantId tenantId,
        string name,
        string displayName,
        OrganizationType organizationType,
        IndustryType industry,
        ContactInfo contactInfo,
        SubscriptionDetails subscription,
        string createdBy)
    {
        TenantId = tenantId;
        Name = name?.Trim() ?? throw new ArgumentException("Name cannot be empty");
        DisplayName = displayName?.Trim() ?? throw new ArgumentException("DisplayName cannot be empty");
        OrganizationType = organizationType;
        Industry = industry;
        Status = TenantStatus.Pending;
        ContactInfo = contactInfo;
        Subscription = subscription;
        Configuration = new SystemConfiguration(tenantId, createdBy);
        Users = new List<UserProfile>();
        Integrations = new List<IntegrationConfiguration>();
        AuditConfiguration = new AuditConfig(
            true,
            new List<AuditEventType> { AuditEventType.Login, AuditEventType.DataChange, AuditEventType.SystemAccess },
            DataRetentionPolicy.OneYear);
        CreatedDate = DateTime.UtcNow;
        Metadata = new Dictionary<string, string>();
        StorageUsed = 0;
        ActiveUsers = 0;
        LastActivityDate = DateTime.UtcNow;

        AddDomainEvent(new TenantCreatedEvent(TenantId, Name, ContactInfo.Email, Subscription.Plan));
    }

    public void Activate(string activatedBy)
    {
        if (Status != TenantStatus.Pending)
            throw new InvalidOperationException($"Cannot activate tenant in {Status} status");

        Status = TenantStatus.Active;
        ActivatedDate = DateTime.UtcNow;
        SuspendedDate = null;
        SuspensionReason = null;

        AddDomainEvent(new TenantActivatedEvent(TenantId, activatedBy));
    }

    public void Suspend(string reason, string suspendedBy)
    {
        if (Status != TenantStatus.Active)
            throw new InvalidOperationException($"Cannot suspend tenant in {Status} status");

        Status = TenantStatus.Suspended;
        SuspendedDate = DateTime.UtcNow;
        SuspensionReason = reason?.Trim();
        ActivatedDate = null;

        AddDomainEvent(new TenantSuspendedEvent(TenantId, reason, suspendedBy));
    }

    public void Deactivate(string reason, string deactivatedBy)
    {
        if (Status == TenantStatus.Inactive)
            throw new InvalidOperationException("Tenant is already inactive");

        Status = TenantStatus.Inactive;
        SuspensionReason = reason?.Trim();

        AddDomainEvent(new TenantDeactivatedEvent(TenantId, reason, deactivatedBy));
    }

    public void UpdateSubscription(SubscriptionDetails newSubscription, string updatedBy)
    {
        var oldPlan = Subscription.Plan;
        var oldStatus = Subscription.Status;
        
        Subscription = newSubscription;

        if (oldPlan != newSubscription.Plan)
        {
            AddDomainEvent(new SubscriptionPlanChangedEvent(TenantId, oldPlan, newSubscription.Plan, updatedBy));
        }

        if (oldStatus != newSubscription.Status)
        {
            AddDomainEvent(new SubscriptionStatusChangedEvent(TenantId, oldStatus, newSubscription.Status, updatedBy));
        }
    }

    public void UpdateContactInfo(ContactInfo contactInfo, string updatedBy)
    {
        ContactInfo = contactInfo;
        AddDomainEvent(new TenantContactUpdatedEvent(TenantId, contactInfo.Email, updatedBy));
    }

    public void AddUser(UserProfile user, string addedBy)
    {
        if (Users.Count >= Subscription.UserLimit)
            throw new InvalidOperationException($"User limit of {Subscription.UserLimit} reached");

        if (Users.Any(u => u.UserId == user.UserId))
            throw new InvalidOperationException($"User {user.UserId} already exists");

        Users.Add(user);
        RecalculateActiveUsers();

        AddDomainEvent(new UserAddedToTenantEvent(TenantId, user.UserId, user.Email, user.Role, addedBy));
    }

    public void RemoveUser(string userId, string removedBy)
    {
        var user = Users.FirstOrDefault(u => u.UserId == userId);
        if (user == null)
            throw new InvalidOperationException($"User {userId} not found");

        Users.Remove(user);
        RecalculateActiveUsers();

        AddDomainEvent(new UserRemovedFromTenantEvent(TenantId, userId, user.Email, removedBy));
    }

    public void UpdateUserRole(string userId, UserRole newRole, List<PermissionType> permissions, string updatedBy)
    {
        var user = Users.FirstOrDefault(u => u.UserId == userId);
        if (user == null)
            throw new InvalidOperationException($"User {userId} not found");

        var oldRole = user.Role;
        user.UpdateRole(newRole, permissions);

        AddDomainEvent(new UserRoleChangedEvent(TenantId, userId, oldRole, newRole, updatedBy));
    }

    public void AddIntegration(IntegrationConfiguration integration, string addedBy)
    {
        if (Integrations.Any(i => i.Name == integration.Name))
            throw new InvalidOperationException($"Integration {integration.Name} already exists");

        Integrations.Add(integration);
        AddDomainEvent(new IntegrationAddedEvent(TenantId, integration.Name, integration.Config.Type, addedBy));
    }

    public void RemoveIntegration(string integrationName, string removedBy)
    {
        var integration = Integrations.FirstOrDefault(i => i.Name == integrationName);
        if (integration == null)
            throw new InvalidOperationException($"Integration {integrationName} not found");

        Integrations.Remove(integration);
        AddDomainEvent(new IntegrationRemovedEvent(TenantId, integrationName, integration.Config.Type, removedBy));
    }

    public void UpdateConfiguration(ConfigurationValue setting, string updatedBy)
    {
        Configuration.UpdateSetting(setting, updatedBy);
        AddDomainEvent(new TenantConfigurationUpdatedEvent(TenantId, setting.Key, updatedBy));
    }

    public void UpdateFeatureFlag(FeatureConfiguration feature, string updatedBy)
    {
        Configuration.UpdateFeature(feature, updatedBy);
        AddDomainEvent(new FeatureFlagUpdatedEvent(TenantId, feature.Feature, feature.IsEnabled, updatedBy));
    }

    public void UpdateAuditConfiguration(AuditConfig auditConfig, string updatedBy)
    {
        AuditConfiguration = auditConfig;
        AddDomainEvent(new AuditConfigurationUpdatedEvent(TenantId, auditConfig.IsEnabled, updatedBy));
    }

    public void RecordActivity()
    {
        LastActivityDate = DateTime.UtcNow;
    }

    public void UpdateStorageUsage(long newUsageBytes)
    {
        var oldUsage = StorageUsed;
        StorageUsed = newUsageBytes;

        var storageLimit = Subscription.StorageLimit * 1024L * 1024L * 1024L; // Convert GB to bytes
        if (StorageUsed > storageLimit * 0.9) // 90% threshold
        {
            AddDomainEvent(new StorageThresholdExceededEvent(TenantId, StorageUsed, storageLimit));
        }
    }

    private void RecalculateActiveUsers()
    {
        ActiveUsers = Users.Count(u => u.IsActive);
    }

    // Business rules and queries
    public bool CanAddUser => Users.Count < Subscription.UserLimit && Status == TenantStatus.Active;
    public bool IsSubscriptionActive => Subscription.IsActive && !Subscription.IsExpired;
    public bool RequiresPayment => Subscription.RequiresPayment;
    public bool IsTrialExpired => Subscription.Plan == SubscriptionPlan.Trial && Subscription.IsExpired;
    public decimal StorageUsagePercentage => Subscription.StorageLimit > 0 
        ? (decimal)(StorageUsed / 1024 / 1024 / 1024) / Subscription.StorageLimit * 100 
        : 0;

    public UserProfile? GetUser(string userId)
    {
        return Users.FirstOrDefault(u => u.UserId == userId);
    }

    public IntegrationConfiguration? GetIntegration(string name)
    {
        return Integrations.FirstOrDefault(i => i.Name == name);
    }

    public bool IsFeatureEnabled(FeatureFlag feature)
    {
        return Configuration.IsFeatureEnabled(feature);
    }

    public List<UserProfile> GetActiveUsers()
    {
        return Users.Where(u => u.IsActive).ToList();
    }

    public List<IntegrationConfiguration> GetActiveIntegrations()
    {
        return Integrations.Where(i => i.IsHealthy).ToList();
    }

    public void AddMetadata(string key, string value)
    {
        Metadata[key] = value;
    }

    public string? GetMetadata(string key)
    {
        return Metadata.TryGetValue(key, out var value) ? value : null;
    }
}

/// <summary>
/// Organization aggregate for managing organizational structure and settings
/// </summary>
public class Organization : AggregateRoot
{
    public TenantId TenantId { get; private set; }
    public string Name { get; private set; }
    public string LegalName { get; private set; }
    public string? TaxId { get; private set; }
    public OrganizationType Type { get; private set; }
    public IndustryType Industry { get; private set; }
    public ContactInfo ContactInfo { get; private set; }
    public Address? BillingAddress { get; private set; }
    public Address? ShippingAddress { get; private set; }
    public CurrencyCode DefaultCurrency { get; private set; }
    public Language DefaultLanguage { get; private set; }
    public TimeZone DefaultTimeZone { get; private set; }
    public string? LogoUrl { get; private set; }
    public string? Website { get; private set; }
    public Dictionary<string, string> CustomFields { get; private set; }
    public List<ComplianceFramework> ComplianceRequirements { get; private set; }
    public Dictionary<SecurityPolicyType, SecurityPolicy> SecurityPolicies { get; private set; }
    public DateTime CreatedDate { get; private set; }
    public DateTime? LastModified { get; private set; }
    public string CreatedBy { get; private set; }

    private Organization()
    {
        TenantId = null!;
        Name = null!;
        LegalName = null!;
        ContactInfo = null!;
        CustomFields = null!;
        ComplianceRequirements = null!;
        SecurityPolicies = null!;
        CreatedBy = null!;
    } // EF Core

    public Organization(
        TenantId tenantId,
        string name,
        string legalName,
        OrganizationType type,
        IndustryType industry,
        ContactInfo contactInfo,
        string createdBy,
        CurrencyCode defaultCurrency = CurrencyCode.USD,
        Language defaultLanguage = Language.English,
        TimeZone defaultTimeZone = TimeZone.UTC)
    {
        TenantId = tenantId;
        Name = name?.Trim() ?? throw new ArgumentException("Name cannot be empty");
        LegalName = legalName?.Trim() ?? throw new ArgumentException("LegalName cannot be empty");
        Type = type;
        Industry = industry;
        ContactInfo = contactInfo;
        DefaultCurrency = defaultCurrency;
        DefaultLanguage = defaultLanguage;
        DefaultTimeZone = defaultTimeZone;
        CreatedBy = createdBy?.Trim() ?? throw new ArgumentException("CreatedBy cannot be empty");
        CreatedDate = DateTime.UtcNow;
        
        CustomFields = new Dictionary<string, string>();
        ComplianceRequirements = new List<ComplianceFramework>();
        SecurityPolicies = new Dictionary<SecurityPolicyType, SecurityPolicy>();

        // Add default security policies based on industry
        InitializeDefaultSecurityPolicies();

        AddDomainEvent(new OrganizationCreatedEvent(TenantId, Name, Type, Industry, createdBy));
    }

    public void UpdateBasicInfo(
        string name,
        string legalName,
        string? taxId,
        string? website,
        string modifiedBy)
    {
        Name = name?.Trim() ?? throw new ArgumentException("Name cannot be empty");
        LegalName = legalName?.Trim() ?? throw new ArgumentException("LegalName cannot be empty");
        TaxId = taxId?.Trim();
        Website = website?.Trim();
        LastModified = DateTime.UtcNow;

        AddDomainEvent(new OrganizationUpdatedEvent(TenantId, Name, modifiedBy));
    }

    public void UpdateContactInfo(ContactInfo contactInfo, string modifiedBy)
    {
        ContactInfo = contactInfo;
        LastModified = DateTime.UtcNow;

        AddDomainEvent(new OrganizationContactUpdatedEvent(TenantId, contactInfo.Email, modifiedBy));
    }

    public void UpdateAddresses(Address? billingAddress, Address? shippingAddress, string modifiedBy)
    {
        BillingAddress = billingAddress;
        ShippingAddress = shippingAddress;
        LastModified = DateTime.UtcNow;

        AddDomainEvent(new OrganizationAddressUpdatedEvent(TenantId, modifiedBy));
    }

    public void UpdateDefaults(
        CurrencyCode currency,
        Language language,
        TimeZone timeZone,
        string modifiedBy)
    {
        DefaultCurrency = currency;
        DefaultLanguage = language;
        DefaultTimeZone = timeZone;
        LastModified = DateTime.UtcNow;

        AddDomainEvent(new OrganizationDefaultsUpdatedEvent(TenantId, currency, language, timeZone, modifiedBy));
    }

    public void UpdateLogo(string logoUrl, string modifiedBy)
    {
        LogoUrl = logoUrl?.Trim();
        LastModified = DateTime.UtcNow;

        AddDomainEvent(new OrganizationLogoUpdatedEvent(TenantId, logoUrl, modifiedBy));
    }

    public void AddComplianceRequirement(ComplianceFramework framework, string addedBy)
    {
        if (!ComplianceRequirements.Contains(framework))
        {
            ComplianceRequirements.Add(framework);
            LastModified = DateTime.UtcNow;

            AddDomainEvent(new ComplianceRequirementAddedEvent(TenantId, framework, addedBy));
        }
    }

    public void RemoveComplianceRequirement(ComplianceFramework framework, string removedBy)
    {
        if (ComplianceRequirements.Remove(framework))
        {
            LastModified = DateTime.UtcNow;
            AddDomainEvent(new ComplianceRequirementRemovedEvent(TenantId, framework, removedBy));
        }
    }

    public void UpdateSecurityPolicy(SecurityPolicy policy, string updatedBy)
    {
        SecurityPolicies[policy.Type] = policy;
        LastModified = DateTime.UtcNow;

        AddDomainEvent(new SecurityPolicyUpdatedEvent(TenantId, policy.Type, updatedBy));
    }

    public void AddCustomField(string key, string value, string addedBy)
    {
        CustomFields[key] = value;
        LastModified = DateTime.UtcNow;

        AddDomainEvent(new CustomFieldAddedEvent(TenantId, key, addedBy));
    }

    public void RemoveCustomField(string key, string removedBy)
    {
        if (CustomFields.Remove(key))
        {
            LastModified = DateTime.UtcNow;
            AddDomainEvent(new CustomFieldRemovedEvent(TenantId, key, removedBy));
        }
    }

    private void InitializeDefaultSecurityPolicies()
    {
        // Password policy
        var passwordRules = new Dictionary<string, string>
        {
            ["MinLength"] = "8",
            ["RequireUppercase"] = "true",
            ["RequireLowercase"] = "true",
            ["RequireNumbers"] = "true",
            ["RequireSpecialChars"] = "false",
            ["MaxAge"] = "90"
        };
        SecurityPolicies[SecurityPolicyType.PasswordPolicy] = new SecurityPolicy(
            SecurityPolicyType.PasswordPolicy, passwordRules, CreatedBy);

        // Session policy
        var sessionRules = new Dictionary<string, string>
        {
            ["SessionTimeout"] = "60",
            ["MaxConcurrentSessions"] = "3",
            ["RequireReauthentication"] = "false"
        };
        SecurityPolicies[SecurityPolicyType.SessionManagement] = new SecurityPolicy(
            SecurityPolicyType.SessionManagement, sessionRules, CreatedBy);

        // Industry-specific policies
        if (Industry == IndustryType.Healthcare || Industry == IndustryType.Finance)
        {
            var dataProtectionRules = new Dictionary<string, string>
            {
                ["EncryptionRequired"] = "true",
                ["DataRetentionDays"] = "2555", // 7 years
                ["AccessLogging"] = "true",
                ["RequireApprovalForAccess"] = "true"
            };
            SecurityPolicies[SecurityPolicyType.DataProtection] = new SecurityPolicy(
                SecurityPolicyType.DataProtection, dataProtectionRules, CreatedBy);
        }
    }

    // Business queries
    public SecurityPolicy? GetSecurityPolicy(SecurityPolicyType type)
    {
        return SecurityPolicies.TryGetValue(type, out var policy) ? policy : null;
    }

    public string? GetCustomField(string key)
    {
        return CustomFields.TryGetValue(key, out var value) ? value : null;
    }

    public bool HasComplianceRequirement(ComplianceFramework framework)
    {
        return ComplianceRequirements.Contains(framework);
    }

    public bool IsHighSecurityIndustry => Industry == IndustryType.Finance || 
                                         Industry == IndustryType.Healthcare || 
                                         Industry == IndustryType.Government;

    public List<SecurityPolicyType> GetRequiredSecurityPolicies()
    {
        var required = new List<SecurityPolicyType>
        {
            SecurityPolicyType.PasswordPolicy,
            SecurityPolicyType.SessionManagement
        };

        if (IsHighSecurityIndustry)
        {
            required.AddRange(new[]
            {
                SecurityPolicyType.DataProtection,
                SecurityPolicyType.AccessControl,
                SecurityPolicyType.AuditLogging
            });
        }

        return required;
    }
}

/// <summary>
/// Application configuration aggregate for system-wide settings
/// </summary>
public class ApplicationConfiguration : AggregateRoot
{
    public TenantId TenantId { get; private set; }
    public string Name { get; private set; }
    public string Version { get; private set; }
    public Dictionary<ModuleName, bool> EnabledModules { get; private set; }
    public Dictionary<FeatureFlag, FeatureConfiguration> GlobalFeatures { get; private set; }
    public Dictionary<IntegrationType, IntegrationConfig> SystemIntegrations { get; private set; }
    public List<NotificationTemplate> NotificationTemplates { get; private set; }
    public List<ApiKey> ApiKeys { get; private set; }
    public Dictionary<RateLimitType, RateLimitConfig> RateLimits { get; private set; }
    public AuditConfig SystemAuditConfig { get; private set; }
    public Dictionary<string, ConfigurationValue> SystemSettings { get; private set; }
    public BackupConfiguration BackupConfig { get; private set; }
    public DateTime CreatedDate { get; private set; }
    public DateTime LastModified { get; private set; }
    public string ModifiedBy { get; private set; }

    private ApplicationConfiguration()
    {
        TenantId = null!;
        Name = null!;
        Version = null!;
        EnabledModules = null!;
        GlobalFeatures = null!;
        SystemIntegrations = null!;
        NotificationTemplates = null!;
        ApiKeys = null!;
        RateLimits = null!;
        SystemAuditConfig = null!;
        SystemSettings = null!;
        BackupConfig = null!;
        ModifiedBy = null!;
    } // EF Core

    public ApplicationConfiguration(TenantId tenantId, string name, string version, string createdBy)
    {
        TenantId = tenantId;
        Name = name?.Trim() ?? throw new ArgumentException("Name cannot be empty");
        Version = version?.Trim() ?? throw new ArgumentException("Version cannot be empty");
        ModifiedBy = createdBy?.Trim() ?? throw new ArgumentException("CreatedBy cannot be empty");
        CreatedDate = DateTime.UtcNow;
        LastModified = DateTime.UtcNow;

        EnabledModules = new Dictionary<ModuleName, bool>();
        GlobalFeatures = new Dictionary<FeatureFlag, FeatureConfiguration>();
        SystemIntegrations = new Dictionary<IntegrationType, IntegrationConfig>();
        NotificationTemplates = new List<NotificationTemplate>();
        ApiKeys = new List<ApiKey>();
        RateLimits = new Dictionary<RateLimitType, RateLimitConfig>();
        SystemSettings = new Dictionary<string, ConfigurationValue>();

        // Initialize default audit configuration
        SystemAuditConfig = new AuditConfig(
            true,
            Enum.GetValues<AuditEventType>().ToList(),
            DataRetentionPolicy.TwoYears,
            includeUserDetails: true,
            includeDataChanges: true,
            encryptLogs: true);

        // Initialize default backup configuration
        BackupConfig = new BackupConfiguration(
            BackupFrequency.Daily,
            retentionDays: 30,
            isEnabled: true);

        // Initialize default modules
        InitializeDefaultModules();

        // Initialize default rate limits
        InitializeDefaultRateLimits();

        AddDomainEvent(new ApplicationConfigurationCreatedEvent(TenantId, Name, Version, createdBy));
    }

    public void EnableModule(ModuleName module, string enabledBy)
    {
        EnabledModules[module] = true;
        LastModified = DateTime.UtcNow;
        ModifiedBy = enabledBy;

        AddDomainEvent(new ModuleEnabledEvent(TenantId, module, enabledBy));
    }

    public void DisableModule(ModuleName module, string disabledBy)
    {
        EnabledModules[module] = false;
        LastModified = DateTime.UtcNow;
        ModifiedBy = disabledBy;

        AddDomainEvent(new ModuleDisabledEvent(TenantId, module, disabledBy));
    }

    public void UpdateGlobalFeature(FeatureConfiguration feature, string updatedBy)
    {
        GlobalFeatures[feature.Feature] = feature;
        LastModified = DateTime.UtcNow;
        ModifiedBy = updatedBy;

        AddDomainEvent(new GlobalFeatureUpdatedEvent(TenantId, feature.Feature, feature.IsEnabled, updatedBy));
    }

    public void AddSystemIntegration(IntegrationConfig integration, string addedBy)
    {
        SystemIntegrations[integration.Type] = integration;
        LastModified = DateTime.UtcNow;
        ModifiedBy = addedBy;

        AddDomainEvent(new SystemIntegrationAddedEvent(TenantId, integration.Type, integration.Name, addedBy));
    }

    public void RemoveSystemIntegration(IntegrationType type, string removedBy)
    {
        if (SystemIntegrations.Remove(type))
        {
            LastModified = DateTime.UtcNow;
            ModifiedBy = removedBy;

            AddDomainEvent(new SystemIntegrationRemovedEvent(TenantId, type, removedBy));
        }
    }

    public void AddNotificationTemplate(NotificationTemplate template, string addedBy)
    {
        NotificationTemplates.Add(template);
        LastModified = DateTime.UtcNow;
        ModifiedBy = addedBy;

        AddDomainEvent(new NotificationTemplateAddedEvent(TenantId, template.Type, template.Name, addedBy));
    }

    public void RemoveNotificationTemplate(Guid templateId, string removedBy)
    {
        var template = NotificationTemplates.FirstOrDefault(t => t.Id == templateId);
        if (template != null)
        {
            NotificationTemplates.Remove(template);
            LastModified = DateTime.UtcNow;
            ModifiedBy = removedBy;

            AddDomainEvent(new NotificationTemplateRemovedEvent(TenantId, template.Type, template.Name, removedBy));
        }
    }

    public void CreateApiKey(ApiKey apiKey, string createdBy)
    {
        ApiKeys.Add(apiKey);
        LastModified = DateTime.UtcNow;
        ModifiedBy = createdBy;

        AddDomainEvent(new ApiKeyCreatedEvent(TenantId, apiKey.Name, apiKey.KeyPrefix, createdBy));
    }

    public void RevokeApiKey(Guid keyId, string revokedBy)
    {
        var apiKey = ApiKeys.FirstOrDefault(k => k.Id == keyId);
        if (apiKey != null)
        {
            apiKey.Deactivate();
            LastModified = DateTime.UtcNow;
            ModifiedBy = revokedBy;

            AddDomainEvent(new ApiKeyRevokedEvent(TenantId, apiKey.Name, apiKey.KeyPrefix, revokedBy));
        }
    }

    public void UpdateRateLimit(RateLimitConfig rateLimit, string updatedBy)
    {
        RateLimits[rateLimit.Type] = rateLimit;
        LastModified = DateTime.UtcNow;
        ModifiedBy = updatedBy;

        AddDomainEvent(new RateLimitUpdatedEvent(TenantId, rateLimit.Type, rateLimit.RequestLimit, updatedBy));
    }

    public void UpdateSystemSetting(ConfigurationValue setting, string updatedBy)
    {
        SystemSettings[setting.Key] = setting;
        LastModified = DateTime.UtcNow;
        ModifiedBy = updatedBy;

        AddDomainEvent(new SystemSettingUpdatedEvent(TenantId, setting.Key, updatedBy));
    }

    public void UpdateAuditConfiguration(AuditConfig auditConfig, string updatedBy)
    {
        SystemAuditConfig = auditConfig;
        LastModified = DateTime.UtcNow;
        ModifiedBy = updatedBy;

        AddDomainEvent(new SystemAuditConfigUpdatedEvent(TenantId, auditConfig.IsEnabled, updatedBy));
    }

    public void UpdateBackupConfiguration(BackupConfiguration backupConfig, string updatedBy)
    {
        BackupConfig = backupConfig;
        LastModified = DateTime.UtcNow;
        ModifiedBy = updatedBy;

        AddDomainEvent(new BackupConfigurationUpdatedEvent(TenantId, backupConfig.Frequency, updatedBy));
    }

    private void InitializeDefaultModules()
    {
        // Enable core modules by default
        EnabledModules[ModuleName.CRM] = true;
        EnabledModules[ModuleName.Accounts] = true;
        EnabledModules[ModuleName.Projects] = true;
        EnabledModules[ModuleName.Assets] = true;
        EnabledModules[ModuleName.HR] = true;
        EnabledModules[ModuleName.Manufacturing] = true;
        EnabledModules[ModuleName.Setup] = true;

        // Optional modules - disabled by default
        EnabledModules[ModuleName.Stock] = false;
        EnabledModules[ModuleName.Buying] = false;
        EnabledModules[ModuleName.Selling] = false;
        EnabledModules[ModuleName.Support] = false;
        EnabledModules[ModuleName.Website] = false;
    }

    private void InitializeDefaultRateLimits()
    {
        RateLimits[RateLimitType.ApiGeneral] = new RateLimitConfig(
            RateLimitType.ApiGeneral, 1000, TimeSpan.FromMinutes(15));
        
        RateLimits[RateLimitType.ApiUser] = new RateLimitConfig(
            RateLimitType.ApiUser, 100, TimeSpan.FromMinutes(15));
        
        RateLimits[RateLimitType.Login] = new RateLimitConfig(
            RateLimitType.Login, 5, TimeSpan.FromMinutes(15));
        
        RateLimits[RateLimitType.PasswordReset] = new RateLimitConfig(
            RateLimitType.PasswordReset, 3, TimeSpan.FromHours(1));
    }

    // Business queries
    public bool IsModuleEnabled(ModuleName module)
    {
        return EnabledModules.TryGetValue(module, out var enabled) && enabled;
    }

    public bool IsGlobalFeatureEnabled(FeatureFlag feature)
    {
        return GlobalFeatures.TryGetValue(feature, out var config) && config.IsEnabled;
    }

    public IntegrationConfig? GetSystemIntegration(IntegrationType type)
    {
        return SystemIntegrations.TryGetValue(type, out var config) ? config : null;
    }

    public RateLimitConfig? GetRateLimit(RateLimitType type)
    {
        return RateLimits.TryGetValue(type, out var config) ? config : null;
    }

    public ConfigurationValue? GetSystemSetting(string key)
    {
        return SystemSettings.TryGetValue(key, out var setting) ? setting : null;
    }

    public NotificationTemplate? GetNotificationTemplate(NotificationType type, Language language)
    {
        return NotificationTemplates.FirstOrDefault(t => t.Type == type && t.Language == language && t.IsActive);
    }

    public List<ApiKey> GetActiveApiKeys()
    {
        return ApiKeys.Where(k => k.CanBeUsed).ToList();
    }

    public List<ModuleName> GetEnabledModules()
    {
        return EnabledModules.Where(kvp => kvp.Value).Select(kvp => kvp.Key).ToList();
    }
}

/// <summary>
/// Backup configuration value object
/// </summary>
public class BackupConfiguration : ValueObject
{
    public BackupFrequency Frequency { get; private set; }
    public int RetentionDays { get; private set; }
    public bool IsEnabled { get; private set; }
    public List<string> IncludedTables { get; private set; }
    public List<string> ExcludedTables { get; private set; }
    public bool CompressBackups { get; private set; }
    public bool EncryptBackups { get; private set; }

    private BackupConfiguration()
    {
        IncludedTables = null!;
        ExcludedTables = null!;
    } // EF Core

    public BackupConfiguration(
        BackupFrequency frequency,
        int retentionDays,
        bool isEnabled,
        bool compressBackups = true,
        bool encryptBackups = true)
    {
        Frequency = frequency;
        RetentionDays = retentionDays > 0 ? retentionDays : throw new ArgumentException("Retention days must be positive");
        IsEnabled = isEnabled;
        CompressBackups = compressBackups;
        EncryptBackups = encryptBackups;
        IncludedTables = new List<string>();
        ExcludedTables = new List<string>();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Frequency;
        yield return RetentionDays;
        yield return IsEnabled;
        yield return CompressBackups;
        yield return EncryptBackups;
    }

    public override string ToString() => $"{Frequency} backups, {RetentionDays} day retention";
}
