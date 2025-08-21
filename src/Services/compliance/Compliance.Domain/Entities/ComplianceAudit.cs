using System;
using System.Collections.Generic;
using TossErp.Shared.SeedWork;

namespace TossErp.Compliance.Domain.Entities;

/// <summary>
/// Represents a compliance audit session
/// </summary>
public class ComplianceAudit : Entity
{
    public string AuditName { get; private set; }
    public AuditType Type { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public AuditStatus Status { get; private set; }
    public string AuditorName { get; private set; }
    public string? AuditorOrganization { get; private set; }
    public string Scope { get; private set; }
    public AuditResult? Result { get; private set; }
    public string? ExecutiveSummary { get; private set; }
    public int TotalFindings { get; private set; }
    public int CriticalFindings { get; private set; }
    public int HighFindings { get; private set; }
    public int MediumFindings { get; private set; }
    public int LowFindings { get; private set; }
    
    private readonly List<AuditFinding> _findings;
    public IReadOnlyCollection<AuditFinding> Findings => _findings.AsReadOnly();

    protected ComplianceAudit()
    {
        _findings = new List<AuditFinding>();
    }

    public ComplianceAudit(
        string auditName,
        AuditType type,
        string auditorName,
        string scope,
        string? auditorOrganization = null) : this()
    {
        AuditName = auditName ?? throw new ArgumentNullException(nameof(auditName));
        Type = type;
        AuditorName = auditorName ?? throw new ArgumentNullException(nameof(auditorName));
        AuditorOrganization = auditorOrganization;
        Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        StartDate = DateTime.UtcNow;
        Status = AuditStatus.Planned;
    }

    public void StartAudit()
    {
        if (Status != AuditStatus.Planned)
            throw new InvalidOperationException("Audit can only be started from Planned status");
        
        Status = AuditStatus.InProgress;
        StartDate = DateTime.UtcNow;
    }

    public void CompleteAudit(AuditResult result, string executiveSummary)
    {
        if (Status != AuditStatus.InProgress)
            throw new InvalidOperationException("Only in-progress audits can be completed");
        
        Status = AuditStatus.Completed;
        EndDate = DateTime.UtcNow;
        Result = result;
        ExecutiveSummary = executiveSummary;
        CalculateFindingsStatistics();
    }

    public void AddFinding(AuditFinding finding)
    {
        if (finding == null) throw new ArgumentNullException(nameof(finding));
        if (Status != AuditStatus.InProgress)
            throw new InvalidOperationException("Findings can only be added to in-progress audits");
        
        _findings.Add(finding);
        TotalFindings = _findings.Count;
    }

    private void CalculateFindingsStatistics()
    {
        CriticalFindings = 0;
        HighFindings = 0;
        MediumFindings = 0;
        LowFindings = 0;

        foreach (var finding in _findings)
        {
            switch (finding.Severity)
            {
                case FindingSeverity.Critical:
                    CriticalFindings++;
                    break;
                case FindingSeverity.High:
                    HighFindings++;
                    break;
                case FindingSeverity.Medium:
                    MediumFindings++;
                    break;
                case FindingSeverity.Low:
                    LowFindings++;
                    break;
            }
        }
    }

    public bool HasCriticalFindings() => CriticalFindings > 0;
    public bool IsCompliant() => Result == AuditResult.Compliant;
}

public class AuditFinding : Entity
{
    public string Title { get; private set; }
    public string Description { get; private set; }
    public FindingSeverity Severity { get; private set; }
    public FindingStatus Status { get; private set; }
    public string ControlReference { get; private set; }
    public string Recommendation { get; private set; }
    public DateTime IdentifiedDate { get; private set; }
    public DateTime? TargetResolutionDate { get; private set; }
    public DateTime? ActualResolutionDate { get; private set; }
    public string? ResolutionNotes { get; private set; }
    public string? AssignedTo { get; private set; }

    protected AuditFinding() { }

    public AuditFinding(
        string title,
        string description,
        FindingSeverity severity,
        string controlReference,
        string recommendation)
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Severity = severity;
        ControlReference = controlReference ?? throw new ArgumentNullException(nameof(controlReference));
        Recommendation = recommendation ?? throw new ArgumentNullException(nameof(recommendation));
        Status = FindingStatus.Open;
        IdentifiedDate = DateTime.UtcNow;
        SetTargetResolutionDate();
    }

    public void AssignTo(string assignee, DateTime? targetDate = null)
    {
        AssignedTo = assignee;
        if (targetDate.HasValue)
            TargetResolutionDate = targetDate.Value;
    }

    public void Resolve(string resolutionNotes)
    {
        Status = FindingStatus.Resolved;
        ResolutionNotes = resolutionNotes;
        ActualResolutionDate = DateTime.UtcNow;
    }

    public void Accept()
    {
        Status = FindingStatus.Accepted;
    }

    private void SetTargetResolutionDate()
    {
        TargetResolutionDate = Severity switch
        {
            FindingSeverity.Critical => DateTime.UtcNow.AddDays(7),
            FindingSeverity.High => DateTime.UtcNow.AddDays(30),
            FindingSeverity.Medium => DateTime.UtcNow.AddDays(60),
            FindingSeverity.Low => DateTime.UtcNow.AddDays(90),
            _ => DateTime.UtcNow.AddDays(30)
        };
    }
}

public enum AuditType
{
    Internal,
    External,
    Certification,
    Regulatory,
    SelfAssessment
}

public enum AuditStatus
{
    Planned,
    InProgress,
    Completed,
    Cancelled,
    OnHold
}

public enum AuditResult
{
    Compliant,
    NonCompliant,
    PartiallyCompliant,
    Inconclusive
}

public enum FindingSeverity
{
    Critical,
    High,
    Medium,
    Low,
    Informational
}

public enum FindingStatus
{
    Open,
    InProgress,
    Resolved,
    Accepted,
    Rejected
}
