namespace Toss.Domain.Entities.Suppliers;

public class SupplierProduct : BaseAuditableEntity
{
    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;
    
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    
    public string? SupplierSKU { get; set; }
    public decimal BasePrice { get; set; }
    public string Currency { get; set; } = "ZAR";
    
    public int MinOrderQuantity { get; set; } = 1;
    public int? LeadTimeDays { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    // Relationships
    public ICollection<SupplierPricing> PricingTiers { get; private set; } = new List<SupplierPricing>();
}

