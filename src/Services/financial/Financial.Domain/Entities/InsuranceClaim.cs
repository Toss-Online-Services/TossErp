using Financial.Domain.Enums;

namespace Financial.Domain.Entities;

/// <summary>
/// Represents an insurance claim filed against a policy
/// </summary>
public class InsuranceClaim
{
    public Guid Id { get; set; }
    public Guid PolicyId { get; set; }
    public string TenantId { get; set; } = string.Empty;

    // Claim Details
    public string ClaimNumber { get; set; } = string.Empty;
    public ClaimStatus Status { get; set; }
    public DateTime IncidentDate { get; set; }
    public DateTime ClaimDate { get; set; }
    public DateTime? SettlementDate { get; set; }

    // Incident Information
    public string IncidentDescription { get; set; } = string.Empty;
    public string IncidentLocation { get; set; } = string.Empty;
    public string IncidentType { get; set; } = string.Empty;
    public string CauseOfLoss { get; set; } = string.Empty;

    // Financial Information
    public decimal ClaimedAmount { get; set; }
    public decimal ApprovedAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal Deductible { get; set; }
    public string Currency { get; set; } = "USD";

    // Processing Information
    public string AssignedAdjuster { get; set; } = string.Empty;
    public string AdjusterContact { get; set; } = string.Empty;
    public DateTime? AdjusterAssignedDate { get; set; }
    public DateTime? InvestigationStartDate { get; set; }
    public DateTime? InvestigationEndDate { get; set; }

    // Documentation and Evidence
    public string SupportingDocuments { get; set; } = string.Empty; // JSON array of document URLs
    public string Photos { get; set; } = string.Empty; // JSON array of photo URLs
    public string WitnessInformation { get; set; } = string.Empty;
    public string PoliceReport { get; set; } = string.Empty;

    // Settlement Information
    public string SettlementReason { get; set; } = string.Empty;
    public string RejectionReason { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public string PaymentReference { get; set; } = string.Empty;

    // Communication Log
    public string CommunicationLog { get; set; } = string.Empty; // JSON array of communications

    // Notes and Comments
    public string Notes { get; set; } = string.Empty;
    public string InternalNotes { get; set; } = string.Empty;

    // Audit Trail
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // Navigation Properties
    public InsurancePolicy Policy { get; set; } = null!;
}
