using Financial.Domain.Enums;

namespace Financial.Domain.Entities;

/// <summary>
/// Represents an insurance policy for business protection
/// </summary>
public class InsurancePolicy
{
    public Guid Id { get; set; }
    public string TenantId { get; set; } = string.Empty;

    // Policy Details
    public string PolicyNumber { get; set; } = string.Empty;
    public InsuranceType Type { get; set; }
    public PolicyStatus Status { get; set; }
    public string PolicyName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // Coverage Information
    public decimal CoverageAmount { get; set; }
    public decimal PremiumAmount { get; set; }
    public decimal Deductible { get; set; }
    public string Currency { get; set; } = "USD";

    // Dates
    public DateTime EffectiveDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public DateTime? RenewalDate { get; set; }
    public DateTime ApplicationDate { get; set; }

    // Insurer Information
    public string InsurerName { get; set; } = string.Empty;
    public string InsurerContact { get; set; } = string.Empty;
    public string AgentName { get; set; } = string.Empty;
    public string AgentContact { get; set; } = string.Empty;

    // Policy Holder Information
    public string PolicyHolderName { get; set; } = string.Empty;
    public string BusinessName { get; set; } = string.Empty;
    public string BusinessType { get; set; } = string.Empty;
    public string BusinessAddress { get; set; } = string.Empty;

    // Coverage Details (JSON for flexibility)
    public string CoverageDetails { get; set; } = string.Empty;
    public string Exclusions { get; set; } = string.Empty;
    public string Terms { get; set; } = string.Empty;

    // Payment Information
    public string PaymentFrequency { get; set; } = string.Empty; // Monthly, Quarterly, Annually
    public DateTime? NextPaymentDate { get; set; }
    public decimal OutstandingPremium { get; set; }
    public bool AutoRenewal { get; set; }

    // Risk Assessment
    public string RiskFactors { get; set; } = string.Empty;
    public decimal RiskScore { get; set; }
    public string RiskCategory { get; set; } = string.Empty;

    // Documentation
    public string PolicyDocuments { get; set; } = string.Empty; // JSON array of document URLs
    public string Notes { get; set; } = string.Empty;

    // Audit Trail
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // Navigation Properties
    public List<InsuranceClaim> Claims { get; set; } = new();
    public List<PolicyPayment> Payments { get; set; } = new();
}
