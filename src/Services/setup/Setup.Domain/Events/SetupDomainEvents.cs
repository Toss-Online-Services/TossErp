using TossErp.Setup.Domain.Enums;
using TossErp.Setup.Domain.SeedWork;
using TossErp.Setup.Domain.ValueObjects;

namespace TossErp.Setup.Domain.Events;

// ================== Tenant Events ==================

/// <summary>
/// Domain event raised when a new tenant is created
/// </summary>
public record TenantCreatedEvent(
    TenantId TenantId,
    string Name,
    string ContactEmail,
    SubscriptionPlan SubscriptionPlan) : DomainEvent;

/// <summary>
/// Domain event raised when a tenant is activated
/// </summary>
public record TenantActivatedEvent(
    TenantId TenantId,
    string ActivatedBy) : DomainEvent;

/// <summary>
/// Domain event raised when a tenant is suspended
/// </summary>
public record TenantSuspendedEvent(
    TenantId TenantId,
    string Reason,
    string SuspendedBy) : DomainEvent;

/// <summary>
/// Domain event raised when a tenant is deactivated
/// </summary>
public record TenantDeactivatedEvent(
    TenantId TenantId,
    string Reason,
    string DeactivatedBy) : DomainEvent;

/// <summary>
/// Domain event raised when tenant contact information is updated
/// </summary>
public record TenantContactUpdatedEvent(
    TenantId TenantId,
    string NewEmail,
    string UpdatedBy) : DomainEvent;

/// <summary>
/// Domain event raised when tenant configuration is updated
/// </summary>
public record TenantConfigurationUpdatedEvent(
    TenantId TenantId,
    string ConfigurationKey,
    string UpdatedBy) : DomainEvent;

/// <summary>
/// Domain event raised when storage threshold is exceeded
/// </summary>
public record StorageThresholdExceededEvent(
    TenantId TenantId,
    long CurrentUsage,
    long StorageLimit) : DomainEvent;

// ================== Subscription Events ==================

/// <summary>
/// Domain event raised when subscription plan changes
/// </summary>
public record SubscriptionPlanChangedEvent(
    TenantId TenantId,
    SubscriptionPlan OldPlan,
    SubscriptionPlan NewPlan,
    string ChangedBy) : DomainEvent;

/// <summary>
/// Domain event raised when subscription status changes
/// </summary>
public record SubscriptionStatusChangedEvent(
    TenantId TenantId,
    SubscriptionStatus OldStatus,
    SubscriptionStatus NewStatus,
    string ChangedBy) : DomainEvent;

/// <summary>
/// Domain event raised when subscription is about to expire
/// </summary>
public record SubscriptionExpiringEvent(
    TenantId TenantId,
    DateTime ExpiryDate,
    int DaysRemaining) : DomainEvent;

/// <summary>
/// Domain event raised when subscription expires
/// </summary>
public record SubscriptionExpiredEvent(
    TenantId TenantId,
    SubscriptionPlan Plan,
    DateTime ExpiryDate) : DomainEvent;

/// <summary>
/// Domain event raised when trial is extended
/// </summary>
public record TrialExtendedEvent(
    TenantId TenantId,
    int AdditionalDays,
    DateTime NewExpiryDate,
    string ExtendedBy) : DomainEvent;

// ================== User Management Events ==================

/// <summary>
/// Domain event raised when a user is added to a tenant
/// </summary>
public record UserAddedToTenantEvent(
    TenantId TenantId,
    string UserId,
    string Email,
    UserRole Role,
    string AddedBy) : DomainEvent;

/// <summary>
/// Domain event raised when a user is removed from a tenant
/// </summary>
public record UserRemovedFromTenantEvent(
    TenantId TenantId,
    string UserId,
    string Email,
    string RemovedBy) : DomainEvent;

/// <summary>
/// Domain event raised when a user's role changes
/// </summary>
public record UserRoleChangedEvent(
    TenantId TenantId,
    string UserId,
    UserRole OldRole,
    UserRole NewRole,
    string ChangedBy) : DomainEvent;

/// <summary>
/// Domain event raised when a user's status changes
/// </summary>
public record UserStatusChangedEvent(
    TenantId TenantId,
    string UserId,
    UserStatus OldStatus,
    UserStatus NewStatus,
    string ChangedBy) : DomainEvent;

/// <summary>
/// Domain event raised when a user logs in
/// </summary>
public record UserLoginEvent(
    TenantId TenantId,
    string UserId,
    string Email,
    DateTime LoginTime,
    string IpAddress,
    string UserAgent) : DomainEvent;

/// <summary>
/// Domain event raised when a user's email is verified
/// </summary>
public record UserEmailVerifiedEvent(
    TenantId TenantId,
    string UserId,
    string Email,
    DateTime VerifiedAt) : DomainEvent;

/// <summary>
/// Domain event raised when password reset is required
/// </summary>
public record PasswordResetRequiredEvent(
    TenantId TenantId,
    string UserId,
    string Email,
    string RequiredBy) : DomainEvent;

// ================== Feature Flag Events ==================

/// <summary>
/// Domain event raised when a feature flag is updated
/// </summary>
public record FeatureFlagUpdatedEvent(
    TenantId TenantId,
    FeatureFlag Feature,
    bool IsEnabled,
    string UpdatedBy) : DomainEvent;

/// <summary>
/// Domain event raised when a global feature is updated
/// </summary>
public record GlobalFeatureUpdatedEvent(
    TenantId TenantId,
    FeatureFlag Feature,
    bool IsEnabled,
    string UpdatedBy) : DomainEvent;

/// <summary>
/// Domain event raised when feature configuration changes
/// </summary>
public record FeatureConfigurationChangedEvent(
    TenantId TenantId,
    FeatureFlag Feature,
    Dictionary<string, string> NewSettings,
    string ChangedBy) : DomainEvent;

// ================== Integration Events ==================

/// <summary>
/// Domain event raised when an integration is added
/// </summary>
public record IntegrationAddedEvent(
    TenantId TenantId,
    string IntegrationName,
    IntegrationType Type,
    string AddedBy) : DomainEvent;

/// <summary>
/// Domain event raised when an integration is removed
/// </summary>
public record IntegrationRemovedEvent(
    TenantId TenantId,
    string IntegrationName,
    IntegrationType Type,
    string RemovedBy) : DomainEvent;

/// <summary>
/// Domain event raised when integration status changes
/// </summary>
public record IntegrationStatusChangedEvent(
    TenantId TenantId,
    string IntegrationName,
    IntegrationStatus OldStatus,
    IntegrationStatus NewStatus,
    string? Error = null) : DomainEvent;

/// <summary>
/// Domain event raised when integration test completes
/// </summary>
public record IntegrationTestCompletedEvent(
    TenantId TenantId,
    string IntegrationName,
    bool Success,
    string? Error,
    DateTime TestedAt) : DomainEvent;

/// <summary>
/// Domain event raised when integration sync completes
/// </summary>
public record IntegrationSyncCompletedEvent(
    TenantId TenantId,
    string IntegrationName,
    bool Success,
    int RecordsProcessed,
    DateTime CompletedAt) : DomainEvent;

/// <summary>
/// Domain event raised when system integration is added
/// </summary>
public record SystemIntegrationAddedEvent(
    TenantId TenantId,
    IntegrationType Type,
    string Name,
    string AddedBy) : DomainEvent;

/// <summary>
/// Domain event raised when system integration is removed
/// </summary>
public record SystemIntegrationRemovedEvent(
    TenantId TenantId,
    IntegrationType Type,
    string RemovedBy) : DomainEvent;

// ================== Audit Configuration Events ==================

/// <summary>
/// Domain event raised when audit configuration is updated
/// </summary>
public record AuditConfigurationUpdatedEvent(
    TenantId TenantId,
    bool IsEnabled,
    string UpdatedBy) : DomainEvent;

/// <summary>
/// Domain event raised when system audit configuration is updated
/// </summary>
public record SystemAuditConfigUpdatedEvent(
    TenantId TenantId,
    bool IsEnabled,
    string UpdatedBy) : DomainEvent;

/// <summary>
/// Domain event raised when audit event occurs
/// </summary>
public record AuditEventOccurredEvent(
    TenantId TenantId,
    AuditEventType EventType,
    string EntityType,
    string EntityId,
    string UserId,
    string Action,
    AuditSeverity Severity,
    DateTime Timestamp) : DomainEvent;

// ================== Organization Events ==================

/// <summary>
/// Domain event raised when an organization is created
/// </summary>
public record OrganizationCreatedEvent(
    TenantId TenantId,
    string Name,
    OrganizationType Type,
    IndustryType Industry,
    string CreatedBy) : DomainEvent;

/// <summary>
/// Domain event raised when organization information is updated
/// </summary>
public record OrganizationUpdatedEvent(
    TenantId TenantId,
    string Name,
    string UpdatedBy) : DomainEvent;

/// <summary>
/// Domain event raised when organization contact is updated
/// </summary>
public record OrganizationContactUpdatedEvent(
    TenantId TenantId,
    string NewEmail,
    string UpdatedBy) : DomainEvent;

/// <summary>
/// Domain event raised when organization address is updated
/// </summary>
public record OrganizationAddressUpdatedEvent(
    TenantId TenantId,
    string UpdatedBy) : DomainEvent;

/// <summary>
/// Domain event raised when organization defaults are updated
/// </summary>
public record OrganizationDefaultsUpdatedEvent(
    TenantId TenantId,
    CurrencyCode Currency,
    Language Language,
    TimeZone TimeZone,
    string UpdatedBy) : DomainEvent;

/// <summary>
/// Domain event raised when organization logo is updated
/// </summary>
public record OrganizationLogoUpdatedEvent(
    TenantId TenantId,
    string? LogoUrl,
    string UpdatedBy) : DomainEvent;

// ================== Compliance Events ==================

/// <summary>
/// Domain event raised when compliance requirement is added
/// </summary>
public record ComplianceRequirementAddedEvent(
    TenantId TenantId,
    ComplianceFramework Framework,
    string AddedBy) : DomainEvent;

/// <summary>
/// Domain event raised when compliance requirement is removed
/// </summary>
public record ComplianceRequirementRemovedEvent(
    TenantId TenantId,
    ComplianceFramework Framework,
    string RemovedBy) : DomainEvent;

/// <summary>
/// Domain event raised when compliance audit starts
/// </summary>
public record ComplianceAuditStartedEvent(
    TenantId TenantId,
    ComplianceFramework Framework,
    string StartedBy,
    DateTime ScheduledDate) : DomainEvent;

/// <summary>
/// Domain event raised when compliance audit completes
/// </summary>
public record ComplianceAuditCompletedEvent(
    TenantId TenantId,
    ComplianceFramework Framework,
    bool Passed,
    List<string> Issues,
    DateTime CompletedDate) : DomainEvent;

// ================== Security Policy Events ==================

/// <summary>
/// Domain event raised when security policy is updated
/// </summary>
public record SecurityPolicyUpdatedEvent(
    TenantId TenantId,
    SecurityPolicyType PolicyType,
    string UpdatedBy) : DomainEvent;

/// <summary>
/// Domain event raised when security policy violation occurs
/// </summary>
public record SecurityPolicyViolationEvent(
    TenantId TenantId,
    SecurityPolicyType PolicyType,
    string UserId,
    string ViolationDetails,
    DateTime OccurredAt) : DomainEvent;

/// <summary>
/// Domain event raised when security incident is detected
/// </summary>
public record SecurityIncidentDetectedEvent(
    TenantId TenantId,
    string IncidentType,
    string Description,
    AuditSeverity Severity,
    string DetectedBy,
    DateTime DetectedAt) : DomainEvent;

// ================== Custom Fields Events ==================

/// <summary>
/// Domain event raised when custom field is added
/// </summary>
public record CustomFieldAddedEvent(
    TenantId TenantId,
    string FieldName,
    string AddedBy) : DomainEvent;

/// <summary>
/// Domain event raised when custom field is removed
/// </summary>
public record CustomFieldRemovedEvent(
    TenantId TenantId,
    string FieldName,
    string RemovedBy) : DomainEvent;

/// <summary>
/// Domain event raised when custom field value is updated
/// </summary>
public record CustomFieldUpdatedEvent(
    TenantId TenantId,
    string FieldName,
    string? OldValue,
    string NewValue,
    string UpdatedBy) : DomainEvent;

// ================== Module Management Events ==================

/// <summary>
/// Domain event raised when a module is enabled
/// </summary>
public record ModuleEnabledEvent(
    TenantId TenantId,
    ModuleName Module,
    string EnabledBy) : DomainEvent;

/// <summary>
/// Domain event raised when a module is disabled
/// </summary>
public record ModuleDisabledEvent(
    TenantId TenantId,
    ModuleName Module,
    string DisabledBy) : DomainEvent;

/// <summary>
/// Domain event raised when module access is requested
/// </summary>
public record ModuleAccessRequestedEvent(
    TenantId TenantId,
    ModuleName Module,
    string UserId,
    bool AccessGranted,
    DateTime RequestedAt) : DomainEvent;

// ================== Application Configuration Events ==================

/// <summary>
/// Domain event raised when application configuration is created
/// </summary>
public record ApplicationConfigurationCreatedEvent(
    TenantId TenantId,
    string Name,
    string Version,
    string CreatedBy) : DomainEvent;

/// <summary>
/// Domain event raised when system setting is updated
/// </summary>
public record SystemSettingUpdatedEvent(
    TenantId TenantId,
    string SettingKey,
    string UpdatedBy) : DomainEvent;

/// <summary>
/// Domain event raised when rate limit is updated
/// </summary>
public record RateLimitUpdatedEvent(
    TenantId TenantId,
    RateLimitType Type,
    int NewLimit,
    string UpdatedBy) : DomainEvent;

/// <summary>
/// Domain event raised when rate limit is exceeded
/// </summary>
public record RateLimitExceededEvent(
    TenantId TenantId,
    RateLimitType Type,
    string UserId,
    string IpAddress,
    DateTime OccurredAt) : DomainEvent;

// ================== Notification Events ==================

/// <summary>
/// Domain event raised when notification template is added
/// </summary>
public record NotificationTemplateAddedEvent(
    TenantId TenantId,
    NotificationType Type,
    string Name,
    string AddedBy) : DomainEvent;

/// <summary>
/// Domain event raised when notification template is removed
/// </summary>
public record NotificationTemplateRemovedEvent(
    TenantId TenantId,
    NotificationType Type,
    string Name,
    string RemovedBy) : DomainEvent;

/// <summary>
/// Domain event raised when notification is sent
/// </summary>
public record NotificationSentEvent(
    TenantId TenantId,
    NotificationType Type,
    NotificationChannel Channel,
    string Recipient,
    bool Success,
    string? Error,
    DateTime SentAt) : DomainEvent;

/// <summary>
/// Domain event raised when notification fails
/// </summary>
public record NotificationFailedEvent(
    TenantId TenantId,
    NotificationType Type,
    NotificationChannel Channel,
    string Recipient,
    string Error,
    int RetryCount,
    DateTime FailedAt) : DomainEvent;

// ================== API Key Events ==================

/// <summary>
/// Domain event raised when API key is created
/// </summary>
public record ApiKeyCreatedEvent(
    TenantId TenantId,
    string Name,
    string KeyPrefix,
    string CreatedBy) : DomainEvent;

/// <summary>
/// Domain event raised when API key is revoked
/// </summary>
public record ApiKeyRevokedEvent(
    TenantId TenantId,
    string Name,
    string KeyPrefix,
    string RevokedBy) : DomainEvent;

/// <summary>
/// Domain event raised when API key is used
/// </summary>
public record ApiKeyUsedEvent(
    TenantId TenantId,
    string KeyPrefix,
    string Endpoint,
    string IpAddress,
    bool Success,
    DateTime UsedAt) : DomainEvent;

/// <summary>
/// Domain event raised when API key expires
/// </summary>
public record ApiKeyExpiredEvent(
    TenantId TenantId,
    string Name,
    string KeyPrefix,
    DateTime ExpiredAt) : DomainEvent;

// ================== Backup Events ==================

/// <summary>
/// Domain event raised when backup configuration is updated
/// </summary>
public record BackupConfigurationUpdatedEvent(
    TenantId TenantId,
    BackupFrequency Frequency,
    string UpdatedBy) : DomainEvent;

/// <summary>
/// Domain event raised when backup starts
/// </summary>
public record BackupStartedEvent(
    TenantId TenantId,
    BackupFrequency Type,
    DateTime StartedAt) : DomainEvent;

/// <summary>
/// Domain event raised when backup completes
/// </summary>
public record BackupCompletedEvent(
    TenantId TenantId,
    BackupFrequency Type,
    bool Success,
    long BackupSize,
    TimeSpan Duration,
    string? Error,
    DateTime CompletedAt) : DomainEvent;

/// <summary>
/// Domain event raised when backup restoration starts
/// </summary>
public record BackupRestorationStartedEvent(
    TenantId TenantId,
    string BackupId,
    DateTime BackupDate,
    string StartedBy,
    DateTime StartedAt) : DomainEvent;

/// <summary>
/// Domain event raised when backup restoration completes
/// </summary>
public record BackupRestorationCompletedEvent(
    TenantId TenantId,
    string BackupId,
    bool Success,
    string? Error,
    DateTime CompletedAt) : DomainEvent;

// ================== System Events ==================

/// <summary>
/// Domain event raised when system maintenance starts
/// </summary>
public record SystemMaintenanceStartedEvent(
    TenantId TenantId,
    string MaintenanceType,
    DateTime ScheduledStart,
    TimeSpan EstimatedDuration,
    string StartedBy) : DomainEvent;

/// <summary>
/// Domain event raised when system maintenance completes
/// </summary>
public record SystemMaintenanceCompletedEvent(
    TenantId TenantId,
    string MaintenanceType,
    bool Success,
    TimeSpan ActualDuration,
    string? Notes,
    DateTime CompletedAt) : DomainEvent;

/// <summary>
/// Domain event raised when system health check fails
/// </summary>
public record SystemHealthCheckFailedEvent(
    TenantId TenantId,
    string Component,
    string Error,
    AuditSeverity Severity,
    DateTime CheckedAt) : DomainEvent;

/// <summary>
/// Domain event raised when license validation occurs
/// </summary>
public record LicenseValidationEvent(
    TenantId TenantId,
    bool IsValid,
    DateTime ExpiryDate,
    List<string> Features,
    DateTime ValidatedAt) : DomainEvent;
