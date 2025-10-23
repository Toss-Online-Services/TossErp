namespace Toss.Domain.Entities.Suppliers;

public class SupplierPricing : BaseAuditableEntity
{
    public int SupplierProductId { get; set; }
    public SupplierProduct SupplierProduct { get; set; } = null!;
    
    public int MinQuantity { get; set; }
    public int? MaxQuantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal? DiscountPercentage { get; set; }
    
    public DateTimeOffset? ValidFrom { get; set; }
    public DateTimeOffset? ValidTo { get; set; }
    public bool IsActive { get; set; } = true;
}

