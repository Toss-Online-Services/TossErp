namespace Toss.Domain.Entities.Orders;

/// <summary>
/// Represents a purchase order from a vendor (procurement/buying)
/// </summary>
public class PurchaseOrder : BaseAuditableEntity
{
    public PurchaseOrder()
    {
        PONumber = string.Empty;
        OrderDate = DateTimeOffset.UtcNow;
        Status = PurchaseOrderStatus.Draft;
        Subtotal = 0;
        TaxAmount = 0;
        ShippingCost = 0;
        Total = 0;
        IsPartOfGroupBuy = false;
        Items = new List<PurchaseOrderItem>();
        Receipts = new List<PurchaseReceipt>();
    }

    /// <summary>
    /// Gets or sets the purchase order number
    /// </summary>
    public string PONumber { get; set; }

    /// <summary>
    /// Gets or sets the shop ID
    /// </summary>
    public int ShopId { get; set; }
    public Store Shop { get; set; } = null!;

    /// <summary>
    /// Gets or sets the vendor ID
    /// </summary>
    public int VendorId { get; set; }
    public Vendor Vendor { get; set; } = null!;

    /// <summary>
    /// Gets or sets the order date
    /// </summary>
    public DateTimeOffset OrderDate { get; set; }

    /// <summary>
    /// Gets or sets the expected delivery date
    /// </summary>
    public DateTimeOffset? ExpectedDeliveryDate { get; set; }

    /// <summary>
    /// Gets or sets the required date
    /// </summary>
    public DateTime? RequiredDate { get; set; }

    /// <summary>
    /// Gets or sets the purchase order status
    /// </summary>
    public PurchaseOrderStatus Status { get; set; }

    // Approval tracking
    /// <summary>
    /// Gets or sets the approval date
    /// </summary>
    public DateTime? ApprovedDate { get; set; }

    /// <summary>
    /// Gets or sets who approved the order
    /// </summary>
    public string? ApprovedBy { get; set; }

    // Amounts
    /// <summary>
    /// Gets or sets the subtotal amount
    /// </summary>
    public decimal Subtotal { get; set; }

    /// <summary>
    /// Gets or sets the tax amount
    /// </summary>
    public decimal TaxAmount { get; set; }

    /// <summary>
    /// Gets or sets the shipping cost
    /// </summary>
    public decimal ShippingCost { get; set; }

    /// <summary>
    /// Gets or sets the total amount
    /// </summary>
    public decimal Total { get; set; }

    // Aliases for handlers
    /// <summary>
    /// Alias for Subtotal (used by handlers)
    /// </summary>
    public decimal SubTotal
    {
        get => Subtotal;
        set => Subtotal = value;
    }

    /// <summary>
    /// Alias for Total (used by handlers)
    /// </summary>
    public decimal TotalAmount
    {
        get => Total;
        set => Total = value;
    }

    // Group buying reference
    /// <summary>
    /// Gets or sets the group buy pool ID if this is part of group buying
    /// </summary>
    public int? GroupBuyPoolId { get; set; }

    /// <summary>
    /// Gets or sets whether this is part of a group buy
    /// </summary>
    public bool IsPartOfGroupBuy { get; set; }

    /// <summary>
    /// Gets or sets optional notes
    /// </summary>
    public string? Notes { get; set; }

    // Relationships
    /// <summary>
    /// Gets or sets the purchase order items
    /// </summary>
    public ICollection<PurchaseOrderItem> Items { get; set; }

    /// <summary>
    /// Gets or sets the purchase receipts
    /// </summary>
    public ICollection<PurchaseReceipt> Receipts { get; set; }
}

