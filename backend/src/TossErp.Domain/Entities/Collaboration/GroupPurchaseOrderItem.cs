using TossErp.Domain.Common;

namespace TossErp.Domain.Entities.Collaboration;

/// <summary>
/// Item in a group purchase order
/// </summary>
public class GroupPurchaseOrderItem : BaseEntity
{
    public int GroupPurchaseOrderId { get; set; }
    public int LineNumber { get; set; }
    
    // Product
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ProductSku { get; set; }
    
    // Quantities
    public decimal TotalQuantity { get; set; } // Total for all members
    public string? UnitOfMeasure { get; set; }
    
    // Pricing
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal DiscountedPrice { get; set; }
    public decimal LineTotal { get; set; }
    
    // Metadata
    public string? Notes { get; set; }
    
    // Navigation
    public GroupPurchaseOrder GroupPurchaseOrder { get; set; } = null!;
}

