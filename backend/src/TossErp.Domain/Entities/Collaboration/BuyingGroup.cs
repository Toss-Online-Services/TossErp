using TossErp.Domain.Common;
using TossErp.Domain.Events.Collaboration;

namespace TossErp.Domain.Entities.Collaboration;

/// <summary>
/// Group Buying - Collective purchasing group for bulk discounts
/// </summary>
public class BuyingGroup : BaseEntity
{
    public string GroupNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public GroupStatus Status { get; set; } = GroupStatus.Forming;
    public GroupType Type { get; set; }
    
    // Leadership
    public int OrganizerId { get; set; }
    public string OrganizerName { get; set; } = string.Empty;
    public string? OrganizerContact { get; set; }
    
    // Membership
    public int MinimumMembers { get; set; }
    public int MaximumMembers { get; set; }
    public int CurrentMemberCount { get; set; }
    public bool IsOpen { get; set; } = true;
    public bool RequiresApproval { get; set; } = true;
    
    // Purchasing
    public string? TargetCategories { get; set; } // JSON array of product categories
    public int? PreferredSupplierId { get; set; }
    public string? PreferredSupplierName { get; set; }
    public decimal? MinimumOrderValue { get; set; }
    public decimal? TargetDiscount { get; set; } // Target bulk discount percentage
    
    // Financials
    public decimal TotalPurchaseValue { get; set; }
    public decimal TotalSavings { get; set; }
    public string PaymentTerms { get; set; } = "Upfront"; // Upfront, OnDelivery, Split
    
    // Timing
    public DateTime? OrderDeadline { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public DateTime? ActualDeliveryDate { get; set; }
    
    // Rules
    public string? MembershipRules { get; set; }
    public string? PurchaseRules { get; set; }
    public decimal? MembershipFee { get; set; }
    
    // Metadata
    public string? Notes { get; set; }
    public string? Tags { get; set; } // JSON array for categorization
    
    // Navigation Properties
    public ICollection<GroupMember> Members { get; set; } = new List<GroupMember>();
    public ICollection<GroupPurchaseOrder> PurchaseOrders { get; set; } = new List<GroupPurchaseOrder>();
    
    // Business Methods
    public void Activate()
    {
        if (CurrentMemberCount < MinimumMembers)
            throw new InvalidOperationException($"Cannot activate group with less than {MinimumMembers} members");
        
        Status = GroupStatus.Active;
        IsOpen = false; // Close membership when activated
        AddDomainEvent(new BuyingGroupActivated(Id, GroupNumber, CurrentMemberCount));
    }
    
    public void AddMember(int customerId, string customerName, decimal commitment)
    {
        if (!IsOpen)
            throw new InvalidOperationException("Group is closed for new members");
        
        if (CurrentMemberCount >= MaximumMembers)
            throw new InvalidOperationException("Group has reached maximum members");
        
        CurrentMemberCount++;
        AddDomainEvent(new MemberJoinedGroup(Id, customerId, customerName, commitment));
    }
    
    public void RemoveMember(int memberId)
    {
        if (Status == GroupStatus.Active || Status == GroupStatus.Ordered)
            throw new InvalidOperationException("Cannot remove members from active or ordered groups");
        
        CurrentMemberCount--;
    }
    
    public void Close()
    {
        Status = GroupStatus.Closed;
        IsOpen = false;
        AddDomainEvent(new BuyingGroupClosed(Id, GroupNumber, TotalPurchaseValue, TotalSavings));
    }
}

public enum GroupStatus
{
    Forming,        // Collecting members
    Active,         // Ready to order
    Ordered,        // Order placed with supplier
    Delivered,      // Goods received
    Distributed,    // Distributed to members
    Closed,
    Cancelled
}

public enum GroupType
{
    OneTime,        // Single purchase event
    Recurring,      // Regular collective purchasing
    Standing        // Permanent buying group
}

