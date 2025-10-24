namespace Toss.Domain.Entities.GroupBuying;

public class AggregatedPurchaseOrder : BaseAuditableEntity
{
    public string APONumber { get; set; } = string.Empty;
    
    // Alias for handlers
    public string PONumber => APONumber;
    
    public int GroupBuyPoolId { get; set; }
    public GroupBuyPool GroupBuyPool { get; set; } = null!;
    
    // Alias for handlers
    public int PoolId => GroupBuyPoolId;
    
    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;
    
    public int TotalQuantity { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal Total { get; set; }
    
    // Alias for handlers
    public decimal TotalAmount => Total;
    
    public DateTimeOffset OrderDate { get; set; }
    public DateTimeOffset? ExpectedDeliveryDate { get; set; }
    public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.Draft;
    
    // Alias for handlers
    public DateTimeOffset? RequiredDate => ExpectedDeliveryDate;
    
    public string? Notes { get; set; }
}

