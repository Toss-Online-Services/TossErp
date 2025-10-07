using TossErp.Domain.Common;
using TossErp.Domain.Events.Collaboration;

namespace TossErp.Domain.Entities.Collaboration;

/// <summary>
/// Individual credit allocation from pool to member
/// </summary>
public class CreditAllocation : BaseEntity
{
    public string AllocationNumber { get; set; } = string.Empty;
    public int CreditPoolId { get; set; }
    public int BorrowerId { get; set; }
    public string BorrowerName { get; set; } = string.Empty;
    
    // Loan Details
    public decimal PrincipalAmount { get; set; }
    public decimal InterestRate { get; set; }
    public int TermMonths { get; set; }
    public decimal MonthlyPayment { get; set; }
    public decimal TotalRepayableAmount { get; set; }
    
    // Dates
    public DateTime DisbursementDate { get; set; }
    public DateTime MaturityDate { get; set; }
    public DateTime? FullyRepaidDate { get; set; }
    
    // Repayment
    public decimal AmountRepaid { get; set; }
    public decimal OutstandingBalance { get; set; }
    public int PaymentsMade { get; set; }
    public int PaymentsMissed { get; set; }
    public DateTime? LastPaymentDate { get; set; }
    public DateTime? NextPaymentDate { get; set; }
    
    // Status
    public AllocationStatus Status { get; set; } = AllocationStatus.Approved;
    public bool IsFullyRepaid { get; set; }
    public bool IsOverdue { get; set; }
    public int DaysOverdue { get; set; }
    
    // Purpose
    public string Purpose { get; set; } = string.Empty;
    public string? PurposeCategory { get; set; } // Inventory, Equipment, Working Capital
    
    // Guarantors
    public string? GuarantorIds { get; set; } // JSON array of customer IDs
    public string? CollateralDescription { get; set; }
    
    // Risk Assessment
    public string? RiskLevel { get; set; } // Low, Medium, High
    public int? CreditScore { get; set; }
    
    // Metadata
    public string? Notes { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedDate { get; set; }
    
    // Navigation
    public CreditPool CreditPool { get; set; } = null!;
    
    // Business Methods
    public void Disburse()
    {
        if (Status != AllocationStatus.Approved)
            throw new InvalidOperationException("Allocation must be approved");
        
        Status = AllocationStatus.Disbursed;
        DisbursementDate = DateTime.UtcNow;
        OutstandingBalance = TotalRepayableAmount;
        AddDomainEvent(new CreditAllocated(Id, CreditPoolId, BorrowerId, PrincipalAmount));
    }
    
    public void RecordPayment(decimal amount)
    {
        AmountRepaid += amount;
        OutstandingBalance -= amount;
        PaymentsMade++;
        LastPaymentDate = DateTime.UtcNow;
        
        if (OutstandingBalance <= 0)
        {
            IsFullyRepaid = true;
            FullyRepaidDate = DateTime.UtcNow;
            Status = AllocationStatus.Closed;
        }
    }
}

public enum AllocationStatus
{
    Requested,
    UnderReview,
    Approved,
    Disbursed,
    Active,
    Defaulted,
    Closed
}

