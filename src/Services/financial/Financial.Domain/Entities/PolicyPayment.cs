namespace Financial.Domain.Entities;

/// <summary>
/// Represents a premium payment for an insurance policy
/// </summary>
public class PolicyPayment
{
    public Guid Id { get; set; }
    public Guid PolicyId { get; set; }
    public string TenantId { get; set; } = string.Empty;

    // Payment Details
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime PaymentDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime CoverageStartDate { get; set; }
    public DateTime CoverageEndDate { get; set; }

    // Payment Information
    public string PaymentMethod { get; set; } = string.Empty; // Credit Card, Bank Transfer, Cash, etc.
    public string TransactionReference { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty; // Pending, Completed, Failed, Refunded

    // Late Payment Information
    public bool IsLate { get; set; }
    public int DaysLate { get; set; }
    public decimal LateFee { get; set; }

    // Receipt Information
    public string ReceiptNumber { get; set; } = string.Empty;
    public string ReceiptUrl { get; set; } = string.Empty;

    // Notes
    public string Notes { get; set; } = string.Empty;

    // Audit Trail
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // Navigation Properties
    public InsurancePolicy Policy { get; set; } = null!;
}
