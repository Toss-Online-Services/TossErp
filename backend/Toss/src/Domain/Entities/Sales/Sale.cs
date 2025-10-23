namespace Toss.Domain.Entities.Sales;

public class Sale : BaseAuditableEntity
{
    public string SaleNumber { get; set; } = string.Empty;
    public int ShopId { get; set; }
    public Shop Shop { get; set; } = null!;
    
    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }
    
    public DateTimeOffset SaleDate { get; set; }
    public SaleStatus Status { get; set; } = SaleStatus.Pending;
    
    // Amounts
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal Total { get; set; }
    
    // Payment
    public PaymentType PaymentMethod { get; set; }
    public string? PaymentReference { get; set; }
    
    public string? Notes { get; set; }
    
    // Relationships
    public ICollection<SaleItem> Items { get; private set; } = new List<SaleItem>();
    public Receipt? Receipt { get; set; }
}

