using TossErp.Domain.Common;

namespace TossErp.Domain.Entities.Collaboration;

/// <summary>
/// Member of a buying group
/// </summary>
public class GroupMember : BaseEntity
{
    public int BuyingGroupId { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    
    // Membership
    public MembershipStatus Status { get; set; } = MembershipStatus.Pending;
    public DateTime JoinedDate { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public int? ApprovedBy { get; set; }
    public string? ApprovedByName { get; set; }
    
    // Commitment
    public decimal CommitmentAmount { get; set; }
    public decimal ActualPurchaseAmount { get; set; }
    public bool HasPaid { get; set; }
    public DateTime? PaidDate { get; set; }
    
    // Savings
    public decimal SavingsAmount { get; set; }
    
    // Role
    public bool IsAdmin { get; set; }
    public string? Role { get; set; } // Organizer, Coordinator, Member
    
    // Metadata
    public string? Notes { get; set; }
    
    // Navigation Properties
    public BuyingGroup BuyingGroup { get; set; } = null!;
}

public enum MembershipStatus
{
    Pending,
    Approved,
    Active,
    Suspended,
    Removed
}

