namespace Toss.Domain.Entities.GroupBuying;

public class AggregatedPurchaseOrder : BaseAuditableEntity
{
    public string APONumber { get; set; } = string.Empty;
    
    // Alias for handlers
    public string PONumber
    {
        get => APONumber;
        set => APONumber = value;
    }
    
    public int GroupBuyPoolId { get; set; }
    public GroupBuyPool GroupBuyPool { get; set; } = null!;
    
    // Alias for handlers
    public int PoolId
    {
        get => GroupBuyPoolId;
        set => GroupBuyPoolId = value;
    }
    
    public int VendorId { get; set; }
    public Vendor Vendor { get; set; } = null!;
    
    public int TotalQuantity { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal Total { get; set; }
    
    // Alias for handlers
    public decimal TotalAmount
    {
        get => Total;
        set => Total = value;
    }
    
    public DateTimeOffset OrderDate { get; set; }
    public DateTimeOffset? ExpectedDeliveryDate { get; set; }
    public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.Draft;
    
    // Alias for handlers
    public DateTimeOffset? RequiredDate
    {
        get => ExpectedDeliveryDate;
        set => ExpectedDeliveryDate = value;
    }
    
    public string? Notes { get; set; }
}

