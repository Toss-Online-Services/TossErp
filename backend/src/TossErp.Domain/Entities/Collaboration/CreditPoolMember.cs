using TossErp.Domain.Common;

namespace TossErp.Domain.Entities.Collaboration;

public class CreditPoolMember : BaseEntity
{
    public int CreditPoolId { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    
    // Contribution
    public decimal ContributionAmount { get; set; }
    public DateTime ContributionDate { get; set; }
    public bool IsFullyPaid { get; set; }
    
    // Membership
    public DateTime JoinDate { get; set; }
    public MembershipStatus Status { get; set; } = MembershipStatus.Active;
    public bool IsGuarantor { get; set; }
    
    // Credit Score
    public int CreditScore { get; set; } // 1-100
    public string? CreditRating { get; set; } // A, B, C, D
    
    // Benefits
    public decimal MaximumBorrowingLimit { get; set; }
    public int TotalLoans { get; set; }
    public decimal TotalBorrowed { get; set; }
    public decimal TotalRepaid { get; set; }
    public decimal CurrentOutstanding { get; set; }
    public bool HasDefaulted { get; set; }
    
    // Metadata
    public string? Notes { get; set; }
    
    // Navigation
    public CreditPool CreditPool { get; set; } = null!;
}

