namespace Toss.Domain.Entities.GroupBuying;

public class AggregatedPurchaseOrder : BaseAuditableEntity
{
    public string APONumber { get; set; } = string.Empty;
    
    public int GroupBuyPoolId { get; set; }
    public GroupBuyPool GroupBuyPool { get; set; } = null!;
    
    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;
    
    public int TotalQuantity { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal Total { get; set; }
    
    public DateTimeOffset OrderDate { get; set; }
    public DateTimeOffset? ExpectedDeliveryDate { get; set; }
    public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.Draft;
    
    public string? Notes { get; set; }
}

