namespace TossErp.Setup.Domain.Enums;

/// <summary>
/// Tenant status enumeration for SaaS multi-tenancy
/// </summary>
public enum TenantStatus
{
    Provisioning,
    Active,
    Suspended,
    Cancelled,
    Delinquent,
    Migrating,
    Archived
}

/// <summary>
/// Subscription plan types for SaaS billing
/// </summary>
public enum SubscriptionPlan
{
    Trial,
    Starter,
    Professional,
    Enterprise,
    Custom
}

/// <summary>
/// Subscription status for billing management
/// </summary>
public enum SubscriptionStatus
{
    Active,
    PastDue,
    Cancelled,
    Suspended,
    TrialExpired,
    PaymentFailed,
    Upgrade,
    Downgrade
}

/// <summary>
/// Organization types for business classification
/// </summary>
public enum OrganizationType
{
    Corporation,
    LLC,
    Partnership,
    SoleProprietorship,
    NonProfit,
    Government,
    Educational,
    Other
}

/// <summary>
/// Industry categories for business classification
/// </summary>
public enum IndustryType
{
    Technology,
    Healthcare,
    Finance,
    Manufacturing,
    Retail,
    Education,
    RealEstate,
    Legal,
    Consulting,
    Marketing,
    Construction,
    Agriculture,
    Transportation,
    Energy,
    Entertainment,
    Hospitality,
    Government,
    NonProfit,
    Other
}

/// <summary>
/// User roles within tenant organizations
/// </summary>
public enum UserRole
{
    SystemAdmin,
    TenantAdmin,
    Manager,
    User,
    ReadOnly,
    Guest
}

/// <summary>
/// User status for access control
/// </summary>
public enum UserStatus
{
    Active,
    Inactive,
    Suspended,
    PendingInvitation,
    Locked,
    Archived
}

/// <summary>
/// Permission types for role-based access control
/// </summary>
public enum PermissionType
{
    Read,
    Write,
    Delete,
    Admin,
    Execute,
    Approve,
    Configure,
    Report
}

/// <summary>
/// Module names available in the ERP system
/// </summary>
public enum ModuleName
{
    Accounts,
    CRM,
    Projects,
    HR,
    Assets,
    Inventory,
    Manufacturing,
    Purchasing,
    Sales,
    Support,
    Reports,
    Settings,
    API
}

/// <summary>
/// Feature flags for module capabilities
/// </summary>
public enum FeatureFlag
{
    AdvancedReporting,
    APIAccess,
    CustomFields,
    Workflows,
    MultiCurrency,
    TaxCompliance,
    AuditTrail,
    BulkOperations,
    DataExport,
    ThirdPartyIntegrations,
    WhiteLabeling,
    CustomDashboards
}

/// <summary>
/// Integration types for third-party services
/// </summary>
public enum IntegrationType
{
    OAuth2,
    APIKey,
    Webhook,
    SAML,
    LDAP,
    Database,
    FileSync,
    Email,
    SMS,
    Payment,
    Accounting,
    CRM,
    Marketing
}

/// <summary>
/// Integration status tracking
/// </summary>
public enum IntegrationStatus
{
    Configured,
    Active,
    Error,
    Disabled,
    Testing,
    Deprecated
}

/// <summary>
/// Notification types for system communications
/// </summary>
public enum NotificationType
{
    Email,
    SMS,
    InApp,
    Push,
    Webhook,
    Slack,
    Teams
}

/// <summary>
/// Notification priority levels
/// </summary>
public enum NotificationPriority
{
    Low,
    Normal,
    High,
    Critical,
    Emergency
}

/// <summary>
/// Audit event types for compliance tracking
/// </summary>
public enum AuditEventType
{
    Login,
    Logout,
    Create,
    Update,
    Delete,
    View,
    Export,
    Import,
    Configure,
    Approve,
    Reject,
    SystemChange
}

/// <summary>
/// Data retention policies for compliance
/// </summary>
public enum DataRetentionPolicy
{
    Days30,
    Days90,
    Months6,
    Year1,
    Years3,
    Years7,
    Years10,
    Indefinite,
    Custom
}

/// <summary>
/// Backup frequency options
/// </summary>
public enum BackupFrequency
{
    Hourly,
    Daily,
    Weekly,
    Monthly,
    Manual,
    RealTime
}

/// <summary>
/// Backup status tracking
/// </summary>
public enum BackupStatus
{
    Scheduled,
    InProgress,
    Completed,
    Failed,
    Partial,
    Cancelled
}

/// <summary>
/// Currency codes for multi-currency support
/// </summary>
public enum CurrencyCode
{
    USD,
    EUR,
    GBP,
    CAD,
    AUD,
    JPY,
    CHF,
    CNY,
    INR,
    BRL,
    MXN,
    ZAR,
    SGD,
    HKD,
    SEK,
    NOK,
    DKK,
    PLN,
    CZK,
    HUF
}

/// <summary>
/// Timezone identifiers for global operations
/// </summary>
public enum TimezoneId
{
    UTC,
    EST,
    CST,
    MST,
    PST,
    GMT,
    CET,
    JST,
    AEST,
    IST,
    CST_China,
    MSK,
    AST,
    HST,
    AKST,
    NST
}

/// <summary>
/// Localization options for internationalization
/// </summary>
public enum LocaleCode
{
    en_US,
    en_GB,
    en_CA,
    en_AU,
    fr_FR,
    fr_CA,
    de_DE,
    es_ES,
    es_MX,
    it_IT,
    pt_BR,
    ja_JP,
    ko_KR,
    zh_CN,
    zh_TW,
    ru_RU,
    ar_SA,
    hi_IN,
    nl_NL,
    sv_SE
}

/// <summary>
/// Configuration scopes for settings management
/// </summary>
public enum ConfigurationScope
{
    System,
    Tenant,
    User,
    Module,
    Feature,
    Integration
}

/// <summary>
/// Configuration data types
/// </summary>
public enum ConfigurationDataType
{
    String,
    Integer,
    Decimal,
    Boolean,
    Date,
    DateTime,
    JSON,
    XML,
    Encrypted,
    File
}

/// <summary>
/// System health status indicators
/// </summary>
public enum HealthStatus
{
    Healthy,
    Warning,
    Critical,
    Down,
    Maintenance,
    Unknown
}

/// <summary>
/// Maintenance window types
/// </summary>
public enum MaintenanceType
{
    Scheduled,
    Emergency,
    Security,
    Performance,
    Feature,
    Compliance
}

/// <summary>
/// Compliance framework types
/// </summary>
public enum ComplianceFramework
{
    GDPR,
    CCPA,
    SOX,
    HIPAA,
    PCI_DSS,
    ISO27001,
    SOC2,
    FISMA,
    Custom
}

/// <summary>
/// Security policy types
/// </summary>
public enum SecurityPolicyType
{
    PasswordPolicy,
    SessionPolicy,
    AccessPolicy,
    DataPolicy,
    NetworkPolicy,
    EncryptionPolicy,
    AuditPolicy,
    CompliancePolicy
}

/// <summary>
/// License types for software compliance
/// </summary>
public enum LicenseType
{
    Perpetual,
    Subscription,
    Concurrent,
    Named,
    Site,
    Enterprise,
    Trial,
    Development
}

/// <summary>
/// API rate limit types
/// </summary>
public enum RateLimitType
{
    PerSecond,
    PerMinute,
    PerHour,
    PerDay,
    PerMonth,
    PerUser,
    PerTenant,
    PerEndpoint
}

/// <summary>
/// Theme and UI customization options
/// </summary>
public enum ThemeMode
{
    Light,
    Dark,
    Auto,
    Custom
}

/// <summary>
/// Import/Export formats
/// </summary>
public enum DataFormat
{
    CSV,
    Excel,
    JSON,
    XML,
    PDF,
    SQL,
    API,
    Custom
}

/// <summary>
/// Workflow approval types
/// </summary>
public enum ApprovalType
{
    Single,
    Sequential,
    Parallel,
    Conditional,
    Matrix,
    Escalating
}

/// <summary>
/// System environment types
/// </summary>
public enum EnvironmentType
{
    Development,
    Testing,
    Staging,
    Production,
    Sandbox,
    Training,
    Demo
}
