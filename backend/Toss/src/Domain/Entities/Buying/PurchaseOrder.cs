namespace Toss.Domain.Entities.Buying;

public class PurchaseOrder : BaseAuditableEntity
{
    public string PONumber { get; set; } = string.Empty;
    
    public int ShopId { get; set; }
    public Shop Shop { get; set; } = null!;
    
    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;
    
    public DateTimeOffset OrderDate { get; set; }
    public DateTimeOffset? ExpectedDeliveryDate { get; set; }
    public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.Draft;
    
    // Amounts
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal Total { get; set; }
    
    // Group buying reference
    public int? GroupBuyPoolId { get; set; }
    public bool IsPartOfGroupBuy { get; set; }
    
    public string? Notes { get; set; }
    
    // Relationships
    public ICollection<PurchaseOrderItem> Items { get; private set; } = new List<PurchaseOrderItem>();
    public ICollection<PurchaseReceipt> Receipts { get; private set; } = new List<PurchaseReceipt>();
}

