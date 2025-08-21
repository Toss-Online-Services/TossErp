using TossErp.Setup.Domain.Enums;
using TossErp.Setup.Domain.SeedWork;

namespace TossErp.Setup.Domain.ValueObjects;

/// <summary>
/// Tenant identifier value object for multi-tenant isolation
/// </summary>
public class TenantId : ValueObject
{
    public string Value { get; private set; }

    private TenantId()
    {
        Value = null!;
    } // EF Core

    public TenantId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Tenant ID cannot be empty");
        
        value = value.Trim().ToLower();
        
        if (!IsValidFormat(value))
            throw new ArgumentException("Tenant ID must be 3-50 alphanumeric characters, hyphens, and underscores only");

        Value = value;
    }

    private static bool IsValidFormat(string tenantId)
    {
        return tenantId.Length >= 3 && tenantId.Length <= 50 && 
               tenantId.All(c => char.IsLetterOrDigit(c) || c == '-' || c == '_') &&
               char.IsLetter(tenantId[0]); // Must start with letter
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(TenantId tenantId) => tenantId.Value;
}

/// <summary>
/// Subscription details value object for SaaS billing
/// </summary>
public class SubscriptionDetails : ValueObject
{
    public SubscriptionPlan Plan { get; private set; }
    public SubscriptionStatus Status { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public DateTime NextBillingDate { get; private set; }
    public decimal MonthlyRate { get; private set; }
    public CurrencyCode Currency { get; private set; }
    public int UserLimit { get; private set; }
    public int StorageLimit { get; private set; } // In GB
    public bool IsTrialExtended { get; private set; }
    public int TrialDaysRemaining { get; private set; }

    private SubscriptionDetails() { } // EF Core

    public SubscriptionDetails(
        SubscriptionPlan plan,
        DateTime startDate,
        decimal monthlyRate,
        CurrencyCode currency,
        int userLimit,
        int storageLimit,
        DateTime? endDate = null)
    {
        Plan = plan;
        Status = plan == SubscriptionPlan.Trial ? SubscriptionStatus.Active : SubscriptionStatus.Active;
        StartDate = startDate.Date;
        EndDate = endDate?.Date;
        MonthlyRate = monthlyRate >= 0 ? monthlyRate : throw new ArgumentException("Monthly rate cannot be negative");
        Currency = currency;
        UserLimit = userLimit > 0 ? userLimit : throw new ArgumentException("User limit must be positive");
        StorageLimit = storageLimit > 0 ? storageLimit : throw new ArgumentException("Storage limit must be positive");
        IsTrialExtended = false;
        
        // Calculate next billing date
        NextBillingDate = plan == SubscriptionPlan.Trial 
            ? startDate.AddDays(30) 
            : startDate.AddMonths(1);
            
        TrialDaysRemaining = plan == SubscriptionPlan.Trial 
            ? Math.Max(0, (NextBillingDate - DateTime.UtcNow.Date).Days)
            : 0;
    }

    public SubscriptionDetails UpdateStatus(SubscriptionStatus status)
    {
        return new SubscriptionDetails(Plan, StartDate, MonthlyRate, Currency, UserLimit, StorageLimit, EndDate)
        {
            Status = status,
            NextBillingDate = NextBillingDate,
            IsTrialExtended = IsTrialExtended,
            TrialDaysRemaining = CalculateTrialDaysRemaining()
        };
    }

    public SubscriptionDetails ExtendTrial(int additionalDays)
    {
        if (Plan != SubscriptionPlan.Trial)
            throw new InvalidOperationException("Can only extend trial subscriptions");

        var newEndDate = NextBillingDate.AddDays(additionalDays);
        return new SubscriptionDetails(Plan, StartDate, MonthlyRate, Currency, UserLimit, StorageLimit, EndDate)
        {
            Status = Status,
            NextBillingDate = newEndDate,
            IsTrialExtended = true,
            TrialDaysRemaining = Math.Max(0, (newEndDate - DateTime.UtcNow.Date).Days)
        };
    }

    private int CalculateTrialDaysRemaining()
    {
        return Plan == SubscriptionPlan.Trial 
            ? Math.Max(0, (NextBillingDate - DateTime.UtcNow.Date).Days)
            : 0;
    }

    public bool IsActive => Status == SubscriptionStatus.Active;
    public bool IsExpired => Plan == SubscriptionPlan.Trial && TrialDaysRemaining <= 0;
    public bool RequiresPayment => Status == SubscriptionStatus.PastDue || Status == SubscriptionStatus.PaymentFailed;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Plan;
        yield return Status;
        yield return StartDate;
        yield return EndDate ?? DateTime.MinValue;
        yield return MonthlyRate;
        yield return Currency;
        yield return UserLimit;
        yield return StorageLimit;
    }

    public override string ToString() => $"{Plan} ({Status}) - {UserLimit} users, {StorageLimit}GB";
}

/// <summary>
/// Contact information value object
/// </summary>
public class ContactInfo : ValueObject
{
    public string Email { get; private set; }
    public string? Phone { get; private set; }
    public string? Website { get; private set; }
    public Address? Address { get; private set; }

    private ContactInfo()
    {
        Email = null!;
    } // EF Core

    public ContactInfo(string email, string? phone = null, string? website = null, Address? address = null)
    {
        Email = ValidateEmail(email);
        Phone = phone?.Trim();
        Website = website?.Trim();
        Address = address;
    }

    private static string ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty");

        email = email.Trim().ToLower();
        
        // Basic email validation
        if (!email.Contains('@') || !email.Contains('.'))
            throw new ArgumentException("Invalid email format");

        return email;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Email;
        yield return Phone ?? string.Empty;
        yield return Website ?? string.Empty;
        yield return Address ?? new Address("", "", "", "", "");
    }

    public override string ToString() => Email;
}

/// <summary>
/// Address value object for location information
/// </summary>
public class Address : ValueObject
{
    public string Street { get; private set; }
    public string City { get; private set; }
    public string State { get; private set; }
    public string PostalCode { get; private set; }
    public string Country { get; private set; }

    private Address()
    {
        Street = null!;
        City = null!;
        State = null!;
        PostalCode = null!;
        Country = null!;
    } // EF Core

    public Address(string street, string city, string state, string postalCode, string country)
    {
        Street = street?.Trim() ?? throw new ArgumentException("Street cannot be empty");
        City = city?.Trim() ?? throw new ArgumentException("City cannot be empty");
        State = state?.Trim() ?? throw new ArgumentException("State cannot be empty");
        PostalCode = postalCode?.Trim() ?? throw new ArgumentException("Postal code cannot be empty");
        Country = country?.Trim() ?? throw new ArgumentException("Country cannot be empty");
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return State;
        yield return PostalCode;
        yield return Country;
    }

    public override string ToString() => $"{Street}, {City}, {State} {PostalCode}, {Country}";
}

/// <summary>
/// System configuration value object
/// </summary>
public class ConfigurationValue : ValueObject
{
    public string Key { get; private set; }
    public string Value { get; private set; }
    public ConfigurationDataType DataType { get; private set; }
    public ConfigurationScope Scope { get; private set; }
    public bool IsEncrypted { get; private set; }
    public string? Description { get; private set; }

    private ConfigurationValue()
    {
        Key = null!;
        Value = null!;
    } // EF Core

    public ConfigurationValue(
        string key,
        string value,
        ConfigurationDataType dataType,
        ConfigurationScope scope,
        bool isEncrypted = false,
        string? description = null)
    {
        Key = key?.Trim() ?? throw new ArgumentException("Key cannot be empty");
        Value = value ?? throw new ArgumentNullException(nameof(value));
        DataType = dataType;
        Scope = scope;
        IsEncrypted = isEncrypted;
        Description = description?.Trim();
    }

    public T GetTypedValue<T>()
    {
        return DataType switch
        {
            ConfigurationDataType.String => (T)(object)Value,
            ConfigurationDataType.Integer => (T)(object)int.Parse(Value),
            ConfigurationDataType.Decimal => (T)(object)decimal.Parse(Value),
            ConfigurationDataType.Boolean => (T)(object)bool.Parse(Value),
            ConfigurationDataType.Date => (T)(object)DateTime.Parse(Value).Date,
            ConfigurationDataType.DateTime => (T)(object)DateTime.Parse(Value),
            _ => throw new InvalidOperationException($"Cannot convert {DataType} to {typeof(T)}")
        };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Key;
        yield return Value;
        yield return DataType;
        yield return Scope;
        yield return IsEncrypted;
    }

    public override string ToString() => $"{Key} = {(IsEncrypted ? "***" : Value)} ({DataType})";
}

/// <summary>
/// Feature configuration value object
/// </summary>
public class FeatureConfiguration : ValueObject
{
    public FeatureFlag Feature { get; private set; }
    public bool IsEnabled { get; private set; }
    public Dictionary<string, string> Settings { get; private set; }
    public DateTime? EnabledDate { get; private set; }
    public DateTime? DisabledDate { get; private set; }
    public string? EnabledBy { get; private set; }

    private FeatureConfiguration()
    {
        Settings = null!;
    } // EF Core

    public FeatureConfiguration(FeatureFlag feature, bool isEnabled, string? enabledBy = null)
    {
        Feature = feature;
        IsEnabled = isEnabled;
        Settings = new Dictionary<string, string>();
        EnabledBy = enabledBy?.Trim();
        
        if (isEnabled)
        {
            EnabledDate = DateTime.UtcNow;
            DisabledDate = null;
        }
        else
        {
            EnabledDate = null;
            DisabledDate = DateTime.UtcNow;
        }
    }

    public FeatureConfiguration WithSetting(string key, string value)
    {
        var newSettings = new Dictionary<string, string>(Settings) { [key] = value };
        return new FeatureConfiguration(Feature, IsEnabled, EnabledBy)
        {
            Settings = newSettings,
            EnabledDate = EnabledDate,
            DisabledDate = DisabledDate
        };
    }

    public string? GetSetting(string key)
    {
        return Settings.TryGetValue(key, out var value) ? value : null;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Feature;
        yield return IsEnabled;
        foreach (var setting in Settings.OrderBy(x => x.Key))
        {
            yield return setting.Key;
            yield return setting.Value;
        }
    }

    public override string ToString() => $"{Feature}: {(IsEnabled ? "Enabled" : "Disabled")}";
}

/// <summary>
/// Integration configuration value object
/// </summary>
public class IntegrationConfig : ValueObject
{
    public IntegrationType Type { get; private set; }
    public string Name { get; private set; }
    public string Endpoint { get; private set; }
    public Dictionary<string, string> Credentials { get; private set; }
    public Dictionary<string, string> Settings { get; private set; }
    public IntegrationStatus Status { get; private set; }
    public DateTime LastTested { get; private set; }
    public string? LastError { get; private set; }

    private IntegrationConfig()
    {
        Name = null!;
        Endpoint = null!;
        Credentials = null!;
        Settings = null!;
    } // EF Core

    public IntegrationConfig(
        IntegrationType type,
        string name,
        string endpoint,
        Dictionary<string, string>? credentials = null,
        Dictionary<string, string>? settings = null)
    {
        Type = type;
        Name = name?.Trim() ?? throw new ArgumentException("Name cannot be empty");
        Endpoint = endpoint?.Trim() ?? throw new ArgumentException("Endpoint cannot be empty");
        Credentials = credentials ?? new Dictionary<string, string>();
        Settings = settings ?? new Dictionary<string, string>();
        Status = IntegrationStatus.Configured;
        LastTested = DateTime.UtcNow;
    }

    public IntegrationConfig UpdateStatus(IntegrationStatus status, string? error = null)
    {
        return new IntegrationConfig(Type, Name, Endpoint, Credentials, Settings)
        {
            Status = status,
            LastTested = DateTime.UtcNow,
            LastError = error?.Trim()
        };
    }

    public string? GetCredential(string key)
    {
        return Credentials.TryGetValue(key, out var value) ? value : null;
    }

    public string? GetSetting(string key)
    {
        return Settings.TryGetValue(key, out var value) ? value : null;
    }

    public bool IsActive => Status == IntegrationStatus.Active;
    public bool HasError => Status == IntegrationStatus.Error;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Type;
        yield return Name;
        yield return Endpoint;
        yield return Status;
    }

    public override string ToString() => $"{Name} ({Type}) - {Status}";
}

/// <summary>
/// API rate limit configuration value object
/// </summary>
public class RateLimitConfig : ValueObject
{
    public RateLimitType Type { get; private set; }
    public int RequestLimit { get; private set; }
    public TimeSpan TimeWindow { get; private set; }
    public string? Scope { get; private set; }
    public bool IsEnabled { get; private set; }

    private RateLimitConfig() { } // EF Core

    public RateLimitConfig(RateLimitType type, int requestLimit, TimeSpan timeWindow, string? scope = null)
    {
        Type = type;
        RequestLimit = requestLimit > 0 ? requestLimit : throw new ArgumentException("Request limit must be positive");
        TimeWindow = timeWindow > TimeSpan.Zero ? timeWindow : throw new ArgumentException("Time window must be positive");
        Scope = scope?.Trim();
        IsEnabled = true;
    }

    public RateLimitConfig Disable()
    {
        return new RateLimitConfig(Type, RequestLimit, TimeWindow, Scope)
        {
            IsEnabled = false
        };
    }

    public decimal RequestsPerSecond => (decimal)RequestLimit / (decimal)TimeWindow.TotalSeconds;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Type;
        yield return RequestLimit;
        yield return TimeWindow;
        yield return Scope ?? string.Empty;
        yield return IsEnabled;
    }

    public override string ToString() => $"{RequestLimit} requests per {TimeWindow} ({Type})";
}

/// <summary>
/// Audit trail configuration value object
/// </summary>
public class AuditConfig : ValueObject
{
    public bool IsEnabled { get; private set; }
    public List<AuditEventType> TrackedEvents { get; private set; }
    public DataRetentionPolicy RetentionPolicy { get; private set; }
    public bool IncludeUserDetails { get; private set; }
    public bool IncludeDataChanges { get; private set; }
    public bool EncryptLogs { get; private set; }

    private AuditConfig()
    {
        TrackedEvents = null!;
    } // EF Core

    public AuditConfig(
        bool isEnabled,
        List<AuditEventType> trackedEvents,
        DataRetentionPolicy retentionPolicy,
        bool includeUserDetails = true,
        bool includeDataChanges = true,
        bool encryptLogs = false)
    {
        IsEnabled = isEnabled;
        TrackedEvents = trackedEvents ?? throw new ArgumentNullException(nameof(trackedEvents));
        RetentionPolicy = retentionPolicy;
        IncludeUserDetails = includeUserDetails;
        IncludeDataChanges = includeDataChanges;
        EncryptLogs = encryptLogs;
    }

    public bool ShouldTrack(AuditEventType eventType)
    {
        return IsEnabled && TrackedEvents.Contains(eventType);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return IsEnabled;
        yield return RetentionPolicy;
        yield return IncludeUserDetails;
        yield return IncludeDataChanges;
        yield return EncryptLogs;
        foreach (var eventType in TrackedEvents.OrderBy(x => x))
        {
            yield return eventType;
        }
    }

    public override string ToString() => $"Audit: {(IsEnabled ? "Enabled" : "Disabled")} - {TrackedEvents.Count} events tracked";
}

/// <summary>
/// Security policy configuration value object
/// </summary>
public class SecurityPolicy : ValueObject
{
    public SecurityPolicyType Type { get; private set; }
    public Dictionary<string, string> Rules { get; private set; }
    public bool IsEnforced { get; private set; }
    public DateTime LastUpdated { get; private set; }
    public string UpdatedBy { get; private set; }

    private SecurityPolicy()
    {
        Rules = null!;
        UpdatedBy = null!;
    } // EF Core

    public SecurityPolicy(SecurityPolicyType type, Dictionary<string, string> rules, string updatedBy)
    {
        Type = type;
        Rules = rules ?? throw new ArgumentNullException(nameof(rules));
        IsEnforced = true;
        LastUpdated = DateTime.UtcNow;
        UpdatedBy = updatedBy?.Trim() ?? throw new ArgumentException("UpdatedBy cannot be empty");
    }

    public string? GetRule(string key)
    {
        return Rules.TryGetValue(key, out var value) ? value : null;
    }

    public SecurityPolicy UpdateRule(string key, string value, string updatedBy)
    {
        var newRules = new Dictionary<string, string>(Rules) { [key] = value };
        return new SecurityPolicy(Type, newRules, updatedBy);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Type;
        yield return IsEnforced;
        foreach (var rule in Rules.OrderBy(x => x.Key))
        {
            yield return rule.Key;
            yield return rule.Value;
        }
    }

    public override string ToString() => $"{Type}: {Rules.Count} rules ({(IsEnforced ? "Enforced" : "Not Enforced")})";
}
