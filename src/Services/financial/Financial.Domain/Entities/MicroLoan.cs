using Financial.Domain.Enums;

namespace Financial.Domain.Entities;

/// <summary>
/// Represents a microfinance loan for small business financing
/// </summary>
public class MicroLoan
{
    public Guid Id { get; set; }
    public string TenantId { get; set; } = string.Empty;

    // Borrower Information
    public string BorrowerId { get; set; } = string.Empty;
    public string BorrowerName { get; set; } = string.Empty;
    public string BusinessName { get; set; } = string.Empty;
    public string BusinessType { get; set; } = string.Empty;

    // Loan Details
    public decimal PrincipalAmount { get; set; }
    public decimal InterestRate { get; set; }
    public int TermMonths { get; set; }
    public decimal MonthlyPayment { get; set; }
    public DateTime ApplicationDate { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public DateTime? DisbursementDate { get; set; }
    public DateTime? MaturityDate { get; set; }

    // Loan Status
    public LoanStatus Status { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public string SecurityOffered { get; set; } = string.Empty;

    // Payment Tracking
    public decimal OutstandingBalance { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal InterestPaid { get; set; }
    public decimal PrincipalPaid { get; set; }
    public int PaymentsMade { get; set; }
    public int PaymentsMissed { get; set; }
    public DateTime? LastPaymentDate { get; set; }
    public DateTime? NextPaymentDate { get; set; }

    // Credit Assessment
    public decimal CreditScore { get; set; }
    public string RiskRating { get; set; } = string.Empty;
    public decimal DebtToIncomeRatio { get; set; }
    public bool HasCollateral { get; set; }
    public string CollateralDescription { get; set; } = string.Empty;
    public decimal CollateralValue { get; set; }

    // Guarantor Information
    public string? GuarantorName { get; set; }
    public string? GuarantorContact { get; set; }
    public string? GuarantorRelationship { get; set; }

    // Documentation
    public string ApplicationDocuments { get; set; } = string.Empty; // JSON array of document URLs
    public string Notes { get; set; } = string.Empty;

    // Audit Trail
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // Navigation Properties
    public List<LoanPayment> Payments { get; set; } = new();
    public List<LoanReview> Reviews { get; set; } = new();
}
