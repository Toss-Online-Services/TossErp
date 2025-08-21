using Financial.Domain.Enums;

namespace Financial.Application.DTOs;

/// <summary>
/// DTO for displaying insurance policy information
/// </summary>
public class InsurancePolicyDto
{
    public Guid Id { get; set; }
    public string PolicyNumber { get; set; } = string.Empty;
    public InsuranceType Type { get; set; }
    public PolicyStatus Status { get; set; }
    public string PolicyName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal CoverageAmount { get; set; }
    public decimal PremiumAmount { get; set; }
    public decimal Deductible { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime EffectiveDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string InsurerName { get; set; } = string.Empty;
    public string AgentName { get; set; } = string.Empty;
    public string PaymentFrequency { get; set; } = string.Empty;
    public DateTime? NextPaymentDate { get; set; }
    public decimal OutstandingPremium { get; set; }
    public bool AutoRenewal { get; set; }
    public int ClaimsCount { get; set; }
    public decimal TotalClaimsAmount { get; set; }
}

/// <summary>
/// DTO for creating a new insurance policy application
/// </summary>
public class CreateInsurancePolicyDto
{
    public InsuranceType Type { get; set; }
    public string PolicyName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal CoverageAmount { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime EffectiveDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string InsurerName { get; set; } = string.Empty;
    public string AgentName { get; set; } = string.Empty;
    public string AgentContact { get; set; } = string.Empty;
    public string PolicyHolderName { get; set; } = string.Empty;
    public string BusinessName { get; set; } = string.Empty;
    public string BusinessType { get; set; } = string.Empty;
    public string BusinessAddress { get; set; } = string.Empty;
    public string PaymentFrequency { get; set; } = string.Empty;
    public bool AutoRenewal { get; set; }
    public List<string> PolicyDocuments { get; set; } = new();
}

/// <summary>
/// DTO for insurance claim information
/// </summary>
public class InsuranceClaimDto
{
    public Guid Id { get; set; }
    public Guid PolicyId { get; set; }
    public string ClaimNumber { get; set; } = string.Empty;
    public ClaimStatus Status { get; set; }
    public DateTime IncidentDate { get; set; }
    public DateTime ClaimDate { get; set; }
    public DateTime? SettlementDate { get; set; }
    public string IncidentDescription { get; set; } = string.Empty;
    public string IncidentLocation { get; set; } = string.Empty;
    public string IncidentType { get; set; } = string.Empty;
    public decimal ClaimedAmount { get; set; }
    public decimal ApprovedAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal Deductible { get; set; }
    public string AssignedAdjuster { get; set; } = string.Empty;
    public string PolicyName { get; set; } = string.Empty;
    public string PolicyNumber { get; set; } = string.Empty;
}

/// <summary>
/// DTO for filing a new insurance claim
/// </summary>
public class CreateInsuranceClaimDto
{
    public Guid PolicyId { get; set; }
    public DateTime IncidentDate { get; set; }
    public string IncidentDescription { get; set; } = string.Empty;
    public string IncidentLocation { get; set; } = string.Empty;
    public string IncidentType { get; set; } = string.Empty;
    public string CauseOfLoss { get; set; } = string.Empty;
    public decimal ClaimedAmount { get; set; }
    public string Currency { get; set; } = "USD";
    public string WitnessInformation { get; set; } = string.Empty;
    public string PoliceReport { get; set; } = string.Empty;
    public List<string> SupportingDocuments { get; set; } = new();
    public List<string> Photos { get; set; } = new();
}

/// <summary>
/// DTO for policy payment information
/// </summary>
public class PolicyPaymentDto
{
    public Guid Id { get; set; }
    public Guid PolicyId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime PaymentDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime CoverageStartDate { get; set; }
    public DateTime CoverageEndDate { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string TransactionReference { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public bool IsLate { get; set; }
    public int DaysLate { get; set; }
    public decimal LateFee { get; set; }
    public string ReceiptNumber { get; set; } = string.Empty;
}
