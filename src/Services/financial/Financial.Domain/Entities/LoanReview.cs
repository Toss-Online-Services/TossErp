namespace Financial.Domain.Entities;

/// <summary>
/// Represents a loan review or assessment event
/// </summary>
public class LoanReview
{
    public Guid Id { get; set; }
    public Guid LoanId { get; set; }
    public string TenantId { get; set; } = string.Empty;

    // Review Details
    public DateTime ReviewDate { get; set; }
    public string ReviewType { get; set; } = string.Empty; // Application, Periodic, Default, etc.
    public string ReviewerName { get; set; } = string.Empty;
    public string ReviewerRole { get; set; } = string.Empty;

    // Assessment
    public string RiskAssessment { get; set; } = string.Empty;
    public decimal RecommendedAmount { get; set; }
    public decimal RecommendedRate { get; set; }
    public string Decision { get; set; } = string.Empty; // Approve, Reject, Request More Info
    public string DecisionReason { get; set; } = string.Empty;

    // Comments and Documentation
    public string Comments { get; set; } = string.Empty;
    public string ReviewDocuments { get; set; } = string.Empty; // JSON array of document URLs

    // Audit Trail
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;

    // Navigation Properties
    public MicroLoan Loan { get; set; } = null!;
}
