using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TossErp.Compliance.Domain.Entities;

namespace TossErp.Compliance.Domain.Services;

/// <summary>
/// Service for automated compliance monitoring and assessment
/// </summary>
public interface IComplianceMonitoringService
{
    Task<ComplianceAssessmentResult> AssessFrameworkComplianceAsync(
        Guid frameworkId, 
        CancellationToken cancellationToken = default);
    
    Task<List<ComplianceAlert>> MonitorContinuousComplianceAsync(
        CancellationToken cancellationToken = default);
    
    Task<ComplianceReport> GenerateComplianceReportAsync(
        ComplianceType type,
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default);
    
    Task<bool> ValidateDataSubjectRightsAsync(
        string dataSubjectId,
        DataSubjectRightType rightType,
        CancellationToken cancellationToken = default);
    
    Task<List<DataRetentionViolation>> CheckDataRetentionPoliciesAsync(
        CancellationToken cancellationToken = default);
    
    Task<SecurityAssessmentResult> PerformSecurityAssessmentAsync(
        CancellationToken cancellationToken = default);
}

public class ComplianceAssessmentResult
{
    public Guid FrameworkId { get; set; }
    public ComplianceType Type { get; set; }
    public DateTime AssessmentDate { get; set; }
    public ComplianceStatus OverallStatus { get; set; }
    public decimal CompliancePercentage { get; set; }
    public List<ControlAssessment> ControlAssessments { get; set; } = new();
    public List<ComplianceGap> Gaps { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
}

public class ControlAssessment
{
    public string ControlId { get; set; } = string.Empty;
    public string ControlName { get; set; } = string.Empty;
    public ControlStatus Status { get; set; }
    public string? Evidence { get; set; }
    public string? Remediation { get; set; }
    public DateTime AssessmentDate { get; set; }
    public bool IsAutomated { get; set; }
}

public class ComplianceGap
{
    public string ControlId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ControlSeverity Severity { get; set; }
    public string RecommendedAction { get; set; } = string.Empty;
    public DateTime TargetResolutionDate { get; set; }
}

public class ComplianceAlert
{
    public Guid Id { get; set; }
    public AlertType Type { get; set; }
    public AlertSeverity Severity { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsResolved { get; set; }
    public string? ResolutionNotes { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class ComplianceReport
{
    public ComplianceType Type { get; set; }
    public DateTime GeneratedAt { get; set; }
    public DateTime ReportPeriodStart { get; set; }
    public DateTime ReportPeriodEnd { get; set; }
    public ComplianceStatus OverallStatus { get; set; }
    public List<ComplianceFramework> Frameworks { get; set; } = new();
    public List<ComplianceMetric> Metrics { get; set; } = new();
    public List<ComplianceViolation> Violations { get; set; } = new();
    public string ExecutiveSummary { get; set; } = string.Empty;
}

public class ComplianceMetric
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class ComplianceViolation
{
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; }
    public ControlSeverity Severity { get; set; }
    public string? RemediationStatus { get; set; }
}

public class DataRetentionViolation
{
    public string DataType { get; set; } = string.Empty;
    public string RecordId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime RetentionDeadline { get; set; }
    public int DaysOverdue { get; set; }
    public string RecommendedAction { get; set; } = string.Empty;
}

public class SecurityAssessmentResult
{
    public DateTime AssessmentDate { get; set; }
    public SecurityPosture OverallPosture { get; set; }
    public List<SecurityControl> Controls { get; set; } = new();
    public List<SecurityThreat> IdentifiedThreats { get; set; } = new();
    public List<SecurityRecommendation> Recommendations { get; set; } = new();
    public decimal SecurityScore { get; set; }
}

public class SecurityControl
{
    public string ControlId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public SecurityControlStatus Status { get; set; }
    public DateTime LastTested { get; set; }
    public string? Evidence { get; set; }
}

public class SecurityThreat
{
    public string ThreatId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ThreatSeverity Severity { get; set; }
    public ThreatLikelihood Likelihood { get; set; }
    public DateTime IdentifiedAt { get; set; }
    public string? MitigationStatus { get; set; }
}

public class SecurityRecommendation
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public RecommendationPriority Priority { get; set; }
    public string Category { get; set; } = string.Empty;
    public int EstimatedEffort { get; set; }
}

public enum DataSubjectRightType
{
    Access,
    Rectification,
    Erasure,
    Portability,
    Restriction,
    Objection
}

public enum AlertType
{
    ComplianceViolation,
    DataRetention,
    SecurityThreat,
    AuditRequired,
    CertificationExpiry,
    PolicyUpdate
}

public enum AlertSeverity
{
    Critical,
    High,
    Medium,
    Low,
    Informational
}

public enum SecurityPosture
{
    Excellent,
    Good,
    Fair,
    Poor,
    Critical
}

public enum SecurityControlStatus
{
    Implemented,
    PartiallyImplemented,
    NotImplemented,
    Failed,
    NotApplicable
}

public enum ThreatSeverity
{
    Critical,
    High,
    Medium,
    Low
}

public enum ThreatLikelihood
{
    VeryHigh,
    High,
    Medium,
    Low,
    VeryLow
}

public enum RecommendationPriority
{
    Immediate,
    High,
    Medium,
    Low,
    WhenPossible
}
