using System;
using System.Collections.Generic;
using TossErp.Shared.SeedWork;

namespace TossErp.Compliance.Domain.Entities;

/// <summary>
/// Represents a specific compliance control within a framework
/// </summary>
public class ComplianceControl : Entity
{
    public string ControlId { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public ControlType Type { get; private set; }
    public ControlSeverity Severity { get; private set; }
    public bool IsAutomated { get; private set; }
    public string? AutomationScript { get; private set; }
    public ControlStatus Status { get; private set; }
    public DateTime LastAssessment { get; private set; }
    public DateTime? NextAssessment { get; private set; }
    public string? Evidence { get; private set; }
    public string? Remediation { get; private set; }
    
    private readonly List<ControlEvidence> _evidenceItems;
    public IReadOnlyCollection<ControlEvidence> EvidenceItems => _evidenceItems.AsReadOnly();

    protected ComplianceControl()
    {
        _evidenceItems = new List<ControlEvidence>();
    }

    public ComplianceControl(
        string controlId,
        string name,
        string description,
        ControlType type,
        ControlSeverity severity) : this()
    {
        ControlId = controlId ?? throw new ArgumentNullException(nameof(controlId));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Type = type;
        Severity = severity;
        Status = ControlStatus.NotAssessed;
        LastAssessment = DateTime.UtcNow;
    }

    public void SetAutomated(string automationScript)
    {
        IsAutomated = true;
        AutomationScript = automationScript;
    }

    public void AssessControl(ControlStatus status, string? evidence = null, string? remediation = null)
    {
        Status = status;
        Evidence = evidence;
        Remediation = remediation;
        LastAssessment = DateTime.UtcNow;
        ScheduleNextAssessment();
    }

    public void AddEvidence(ControlEvidence evidence)
    {
        if (evidence == null) throw new ArgumentNullException(nameof(evidence));
        _evidenceItems.Add(evidence);
    }

    private void ScheduleNextAssessment()
    {
        NextAssessment = Severity switch
        {
            ControlSeverity.Critical => DateTime.UtcNow.AddDays(30),
            ControlSeverity.High => DateTime.UtcNow.AddDays(60),
            ControlSeverity.Medium => DateTime.UtcNow.AddDays(90),
            ControlSeverity.Low => DateTime.UtcNow.AddDays(180),
            _ => DateTime.UtcNow.AddDays(90)
        };
    }

    public bool RequiresAssessment()
    {
        return NextAssessment.HasValue && NextAssessment.Value <= DateTime.UtcNow;
    }
}

public class ControlEvidence : Entity
{
    public string Type { get; private set; }
    public string Description { get; private set; }
    public string FilePath { get; private set; }
    public DateTime CollectedAt { get; private set; }
    public string CollectedBy { get; private set; }

    protected ControlEvidence() { }

    public ControlEvidence(string type, string description, string filePath, string collectedBy)
    {
        Type = type ?? throw new ArgumentNullException(nameof(type));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        CollectedBy = collectedBy ?? throw new ArgumentNullException(nameof(collectedBy));
        CollectedAt = DateTime.UtcNow;
    }
}

public enum ControlType
{
    Technical,
    Administrative,
    Physical,
    Procedural
}

public enum ControlSeverity
{
    Critical,
    High,
    Medium,
    Low
}

public enum ControlStatus
{
    NotAssessed,
    Compliant,
    NonCompliant,
    PartiallyCompliant,
    NotApplicable,
    InRemediation
}
