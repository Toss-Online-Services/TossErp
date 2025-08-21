using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TossErp.Security.Domain.Services;

/// <summary>
/// Zero Trust Architecture service for continuous authentication and authorization
/// </summary>
public interface IZeroTrustService
{
    Task<ZeroTrustAssessment> EvaluateRequestAsync(
        ZeroTrustContext context, 
        CancellationToken cancellationToken = default);
    
    Task<bool> ValidateContinuousAuthenticationAsync(
        string userId,
        string sessionId,
        CancellationToken cancellationToken = default);
    
    Task<TrustScore> CalculateTrustScoreAsync(
        string userId,
        DeviceContext deviceContext,
        LocationContext locationContext,
        BehaviorContext behaviorContext,
        CancellationToken cancellationToken = default);
    
    Task<List<SecurityPolicy>> GetApplicablePoliciesAsync(
        string userId,
        string resourceId,
        CancellationToken cancellationToken = default);
    
    Task<bool> EnforceNetworkSegmentationAsync(
        string sourceServiceId,
        string targetServiceId,
        string operation,
        CancellationToken cancellationToken = default);
    
    Task<EncryptionRequirement> GetEncryptionRequirementsAsync(
        string dataClassification,
        CancellationToken cancellationToken = default);
}

public class ZeroTrustContext
{
    public string UserId { get; set; } = string.Empty;
    public string SessionId { get; set; } = string.Empty;
    public string ResourceId { get; set; } = string.Empty;
    public string Operation { get; set; } = string.Empty;
    public DeviceContext Device { get; set; } = new();
    public LocationContext Location { get; set; } = new();
    public BehaviorContext Behavior { get; set; } = new();
    public DateTime RequestTime { get; set; }
    public Dictionary<string, object> AdditionalContext { get; set; } = new();
}

public class DeviceContext
{
    public string DeviceId { get; set; } = string.Empty;
    public string DeviceType { get; set; } = string.Empty;
    public string OperatingSystem { get; set; } = string.Empty;
    public string BrowserType { get; set; } = string.Empty;
    public bool IsManagedDevice { get; set; }
    public bool IsEncrypted { get; set; }
    public bool HasAntiVirus { get; set; }
    public DateTime LastSecurityScan { get; set; }
    public List<string> SecurityVulnerabilities { get; set; } = new();
    public DeviceTrustLevel TrustLevel { get; set; }
}

public class LocationContext
{
    public string IpAddress { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public bool IsVpn { get; set; }
    public bool IsTor { get; set; }
    public bool IsKnownLocation { get; set; }
    public LocationRiskLevel RiskLevel { get; set; }
    public string ISP { get; set; } = string.Empty;
    public List<string> ThreatIntelligenceFlags { get; set; } = new();
}

public class BehaviorContext
{
    public TimeSpan TypicalLoginTime { get; set; }
    public List<string> UsualLocations { get; set; } = new();
    public List<string> UsualDevices { get; set; } = new();
    public int FailedLoginAttempts { get; set; }
    public DateTime LastSuccessfulLogin { get; set; }
    public bool IsAnomalousActivity { get; set; }
    public decimal BehaviorScore { get; set; }
    public List<BehaviorAnomaly> DetectedAnomalies { get; set; } = new();
}

public class BehaviorAnomaly
{
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Severity { get; set; }
    public DateTime DetectedAt { get; set; }
}

public class ZeroTrustAssessment
{
    public string RequestId { get; set; } = string.Empty;
    public DateTime AssessmentTime { get; set; }
    public AccessDecision Decision { get; set; }
    public TrustScore TrustScore { get; set; } = new();
    public List<PolicyEvaluation> PolicyResults { get; set; } = new();
    public List<SecurityRequirement> AdditionalRequirements { get; set; } = new();
    public string? DenialReason { get; set; }
    public TimeSpan? SessionDuration { get; set; }
    public bool RequiresStepUp { get; set; }
    public List<string> RecommendedActions { get; set; } = new();
}

public class TrustScore
{
    public decimal OverallScore { get; set; }
    public decimal DeviceScore { get; set; }
    public decimal LocationScore { get; set; }
    public decimal BehaviorScore { get; set; }
    public decimal CredentialScore { get; set; }
    public TrustLevel Level { get; set; }
    public DateTime CalculatedAt { get; set; }
    public List<TrustFactor> Factors { get; set; } = new();
}

public class TrustFactor
{
    public string Name { get; set; } = string.Empty;
    public decimal Weight { get; set; }
    public decimal Score { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class PolicyEvaluation
{
    public string PolicyId { get; set; } = string.Empty;
    public string PolicyName { get; set; } = string.Empty;
    public PolicyResult Result { get; set; }
    public string? Reason { get; set; }
    public List<string> RequiredConditions { get; set; } = new();
    public List<string> FailedConditions { get; set; } = new();
}

public class SecurityRequirement
{
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsMandatory { get; set; }
    public TimeSpan? TimeLimit { get; set; }
    public Dictionary<string, object> Parameters { get; set; } = new();
}

public class SecurityPolicy
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public PolicyType Type { get; set; }
    public List<PolicyCondition> Conditions { get; set; } = new();
    public List<PolicyAction> Actions { get; set; } = new();
    public int Priority { get; set; }
    public bool IsEnabled { get; set; }
}

public class PolicyCondition
{
    public string Field { get; set; } = string.Empty;
    public ComparisonOperator Operator { get; set; }
    public object Value { get; set; } = new();
    public LogicalOperator? LogicalOperator { get; set; }
}

public class PolicyAction
{
    public ActionType Type { get; set; }
    public Dictionary<string, object> Parameters { get; set; } = new();
}

public class EncryptionRequirement
{
    public string Algorithm { get; set; } = string.Empty;
    public int KeyLength { get; set; }
    public string Mode { get; set; } = string.Empty;
    public bool RequiresHardwareSecurityModule { get; set; }
    public bool RequiresKeyRotation { get; set; }
    public TimeSpan KeyRotationInterval { get; set; }
}

public enum AccessDecision
{
    Allow,
    Deny,
    AllowWithConditions,
    RequireStepUp,
    RequireReauthentication
}

public enum TrustLevel
{
    VeryHigh,
    High,
    Medium,
    Low,
    VeryLow,
    Untrusted
}

public enum DeviceTrustLevel
{
    Trusted,
    Managed,
    Unmanaged,
    Compromised,
    Blocked
}

public enum LocationRiskLevel
{
    VeryLow,
    Low,
    Medium,
    High,
    VeryHigh,
    Blocked
}

public enum PolicyResult
{
    Allow,
    Deny,
    NotApplicable,
    RequiresEvaluation
}

public enum PolicyType
{
    Authentication,
    Authorization,
    DataAccess,
    NetworkAccess,
    DeviceCompliance,
    LocationBased,
    TimeBased,
    RiskBased
}

public enum ComparisonOperator
{
    Equals,
    NotEquals,
    GreaterThan,
    LessThan,
    GreaterThanOrEqual,
    LessThanOrEqual,
    Contains,
    NotContains,
    In,
    NotIn,
    Matches,
    NotMatches
}

public enum LogicalOperator
{
    And,
    Or,
    Not
}

public enum ActionType
{
    Allow,
    Deny,
    RequireMFA,
    RequirePasswordChange,
    LimitSessionTime,
    RequireDeviceRegistration,
    BlockDevice,
    LogEvent,
    SendAlert,
    RequireApproval
}
