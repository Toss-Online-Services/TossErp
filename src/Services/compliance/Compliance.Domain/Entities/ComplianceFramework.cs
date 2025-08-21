using System;
using System.Collections.Generic;
using TossErp.Shared.SeedWork;

namespace TossErp.Compliance.Domain.Entities;

/// <summary>
/// Represents a compliance framework implementation for enterprise-grade compliance
/// </summary>
public class ComplianceFramework : Entity, IAggregateRoot
{
    public string Name { get; private set; }
    public ComplianceType Type { get; private set; }
    public string Version { get; private set; }
    public string Description { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime ImplementationDate { get; private set; }
    public DateTime? CertificationDate { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    public ComplianceStatus Status { get; private set; }
    
    private readonly List<ComplianceControl> _controls;
    public IReadOnlyCollection<ComplianceControl> Controls => _controls.AsReadOnly();
    
    private readonly List<ComplianceAudit> _audits;
    public IReadOnlyCollection<ComplianceAudit> Audits => _audits.AsReadOnly();

    protected ComplianceFramework()
    {
        _controls = new List<ComplianceControl>();
        _audits = new List<ComplianceAudit>();
    }

    public ComplianceFramework(
        string name,
        ComplianceType type,
        string version,
        string description) : this()
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Type = type;
        Version = version ?? throw new ArgumentNullException(nameof(version));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        IsActive = false;
        Status = ComplianceStatus.NotImplemented;
        ImplementationDate = DateTime.UtcNow;
    }

    public void AddControl(ComplianceControl control)
    {
        if (control == null) throw new ArgumentNullException(nameof(control));
        _controls.Add(control);
    }

    public void Activate()
    {
        IsActive = true;
        Status = ComplianceStatus.Active;
    }

    public void Deactivate()
    {
        IsActive = false;
        Status = ComplianceStatus.Inactive;
    }

    public void SetCertified(DateTime certificationDate, DateTime? expiryDate = null)
    {
        CertificationDate = certificationDate;
        ExpiryDate = expiryDate;
        Status = ComplianceStatus.Certified;
    }

    public void AddAudit(ComplianceAudit audit)
    {
        if (audit == null) throw new ArgumentNullException(nameof(audit));
        _audits.Add(audit);
    }

    public bool IsCompliant()
    {
        return Status == ComplianceStatus.Certified && 
               (ExpiryDate == null || ExpiryDate > DateTime.UtcNow);
    }
}

public enum ComplianceType
{
    SOC2,
    GDPR,
    CCPA,
    HIPAA,
    ISO27001,
    PCI_DSS,
    FISMA,
    Custom
}

public enum ComplianceStatus
{
    NotImplemented,
    InProgress,
    Active,
    Certified,
    Expired,
    Inactive,
    NonCompliant
}
