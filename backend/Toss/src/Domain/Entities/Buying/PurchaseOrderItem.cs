namespace Toss.Domain.Entities.Buying;

public class PurchaseOrderItem : BaseEntity
{
    public int PurchaseOrderId { get; set; }
    public PurchaseOrder PurchaseOrder { get; set; } = null!;
    
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    
    public string ProductName { get; set; } = string.Empty;
    public string? ProductSKU { get; set; }
    
    public int QuantityOrdered { get; set; }
    public int QuantityReceived { get; set; }
    
    // Alias for handlers
    public int Quantity
    {
        get => QuantityOrdered;
        set => QuantityOrdered = value;
    }
    
    public decimal UnitPrice { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal LineTotal { get; set; }
}

