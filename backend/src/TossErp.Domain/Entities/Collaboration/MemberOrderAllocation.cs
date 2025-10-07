using TossErp.Domain.Common;

namespace TossErp.Domain.Entities.Collaboration;

/// <summary>
/// Individual member's share of a group purchase order
/// </summary>
public class MemberOrderAllocation : BaseEntity
{
    public int GroupPurchaseOrderId { get; set; }
    public int GroupMemberId { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    
    // Allocation
    public decimal AllocatedAmount { get; set; }
    public decimal ShippingShare { get; set; }
    public decimal TotalDue { get; set; }
    
    // Payment
    public bool IsPaid { get; set; }
    public decimal PaidAmount { get; set; }
    public DateTime? PaidDate { get; set; }
    public string? PaymentMethod { get; set; }
    
    // Delivery
    public bool IsCollected { get; set; }
    public DateTime? CollectionDate { get; set; }
    public string? CollectedBy { get; set; }
    
    // Metadata
    public string? Notes { get; set; }
    public string? ItemsAllocated { get; set; } // JSON: [{productId, quantity, amount}]
    
    // Navigation
    public GroupPurchaseOrder GroupPurchaseOrder { get; set; } = null!;
}

