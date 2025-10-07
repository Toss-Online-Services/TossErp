using TossErp.Domain.Common;

namespace TossErp.Domain.Entities.Collaboration;

/// <summary>
/// Collective purchase order for a buying group
/// </summary>
public class GroupPurchaseOrder : BaseEntity
{
    public int BuyingGroupId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    
    // Supplier
    public int SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    
    // Order Details
    public DateTime OrderDate { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public DateTime? ActualDeliveryDate { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Draft;
    
    // Amounts
    public decimal TotalAmount { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal NetAmount { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal GrandTotal { get; set; }
    
    // Cost Distribution
    public string? CostDistributionMethod { get; set; } // Equal, ProRata, Quantity
    public decimal SharedCostPerMember { get; set; }
    
    // Payment
    public bool IsFullyPaid { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal TotalDue { get; set; }
    
    // Metadata
    public string? Notes { get; set; }
    public string? Terms { get; set; }
    
    // Navigation
    public BuyingGroup BuyingGroup { get; set; } = null!;
    public ICollection<GroupPurchaseOrderItem> Items { get; set; } = new List<GroupPurchaseOrderItem>();
    public ICollection<MemberOrderAllocation> MemberAllocations { get; set; } = new List<MemberOrderAllocation>();
}

public enum OrderStatus
{
    Draft,
    Submitted,
    Confirmed,
    Ordered,
    Delivered,
    Cancelled
}

