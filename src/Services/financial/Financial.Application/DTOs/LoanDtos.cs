using Financial.Domain.Enums;

namespace Financial.Application.DTOs;

/// <summary>
/// DTO for displaying loan information
/// </summary>
public class MicroLoanDto
{
    public Guid Id { get; set; }
    public string BorrowerName { get; set; } = string.Empty;
    public string BusinessName { get; set; } = string.Empty;
    public string BusinessType { get; set; } = string.Empty;
    public decimal PrincipalAmount { get; set; }
    public decimal InterestRate { get; set; }
    public int TermMonths { get; set; }
    public decimal MonthlyPayment { get; set; }
    public DateTime ApplicationDate { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public DateTime? DisbursementDate { get; set; }
    public LoanStatus Status { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public decimal OutstandingBalance { get; set; }
    public decimal TotalPaid { get; set; }
    public int PaymentsMade { get; set; }
    public int PaymentsMissed { get; set; }
    public DateTime? NextPaymentDate { get; set; }
    public decimal CreditScore { get; set; }
    public string RiskRating { get; set; } = string.Empty;
}

/// <summary>
/// DTO for creating a new loan application
/// </summary>
public class CreateLoanApplicationDto
{
    public string BorrowerName { get; set; } = string.Empty;
    public string BusinessName { get; set; } = string.Empty;
    public string BusinessType { get; set; } = string.Empty;
    public decimal RequestedAmount { get; set; }
    public int TermMonths { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public string SecurityOffered { get; set; } = string.Empty;
    public decimal DebtToIncomeRatio { get; set; }
    public bool HasCollateral { get; set; }
    public string CollateralDescription { get; set; } = string.Empty;
    public decimal CollateralValue { get; set; }
    public string? GuarantorName { get; set; }
    public string? GuarantorContact { get; set; }
    public string? GuarantorRelationship { get; set; }
    public List<string> ApplicationDocuments { get; set; } = new();
}

/// <summary>
/// DTO for loan payment information
/// </summary>
public class LoanPaymentDto
{
    public Guid Id { get; set; }
    public Guid LoanId { get; set; }
    public decimal Amount { get; set; }
    public decimal PrincipalAmount { get; set; }
    public decimal InterestAmount { get; set; }
    public decimal PenaltyAmount { get; set; }
    public DateTime PaymentDate { get; set; }
    public DateTime DueDate { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string TransactionReference { get; set; } = string.Empty;
    public bool IsLate { get; set; }
    public int DaysLate { get; set; }
    public decimal RemainingBalance { get; set; }
}

/// <summary>
/// DTO for recording a loan payment
/// </summary>
public class RecordLoanPaymentDto
{
    public Guid LoanId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string TransactionReference { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}
