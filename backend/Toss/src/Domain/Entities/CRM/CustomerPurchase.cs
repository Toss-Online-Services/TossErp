namespace Toss.Domain.Entities.CRM;

public class CustomerPurchase : BaseAuditableEntity
{
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    
    public int SaleId { get; set; }
    public Sale Sale { get; set; } = null!;
    
    public DateTimeOffset PurchaseDate { get; set; }
    public decimal PurchaseAmount { get; set; }
    
    // Alias for handlers
    public decimal TotalAmount => PurchaseAmount;
    
    // Insights
    public int ItemCount { get; set; }
    public string? TopProductCategory { get; set; }
}

