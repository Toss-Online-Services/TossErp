namespace Toss.Domain.Entities.Sales;

public class SaleItem : BaseEntity
{
    public int SaleId { get; set; }
    public Sale Sale { get; set; } = null!;
    
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    
    public string ProductName { get; set; } = string.Empty; // Snapshot at time of sale
    public string? ProductSKU { get; set; }
    
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal LineTotal { get; set; }
}

