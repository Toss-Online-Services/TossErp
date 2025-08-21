namespace Financial.Domain.Entities;

/// <summary>
/// Represents a payment made against a micro loan
/// </summary>
public class LoanPayment
{
    public Guid Id { get; set; }
    public Guid LoanId { get; set; }
    public string TenantId { get; set; } = string.Empty;

    // Payment Details
    public decimal Amount { get; set; }
    public decimal PrincipalAmount { get; set; }
    public decimal InterestAmount { get; set; }
    public decimal PenaltyAmount { get; set; }
    public DateTime PaymentDate { get; set; }
    public DateTime DueDate { get; set; }

    // Payment Information
    public string PaymentMethod { get; set; } = string.Empty; // Cash, Bank Transfer, Mobile Money, etc.
    public string TransactionReference { get; set; } = string.Empty;
    public string ReceivedBy { get; set; } = string.Empty;

    // Status
    public bool IsLate { get; set; }
    public int DaysLate { get; set; }
    public string Notes { get; set; } = string.Empty;

    // Balance Information
    public decimal RemainingBalance { get; set; }

    // Audit Trail
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;

    // Navigation Properties
    public MicroLoan Loan { get; set; } = null!;
}
