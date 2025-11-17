namespace Toss.Domain.Entities.Vendors;

/// <summary>
/// Represents a vendor-product association
/// </summary>
public class VendorProduct : BaseAuditableEntity
{
    public VendorProduct()
    {
        Currency = "ZAR";
        MinOrderQuantity = 1;
        IsActive = true;
        PricingTiers = new List<VendorPricing>();
    }

    /// <summary>
    /// Gets or sets the vendor ID
    /// </summary>
    public int VendorId { get; set; }
    public Vendor Vendor { get; set; } = null!;

    /// <summary>
    /// Gets or sets the product ID
    /// </summary>
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    /// <summary>
    /// Gets or sets the vendor's SKU for this product
    /// </summary>
    public string? VendorSKU { get; set; }

    /// <summary>
    /// Gets or sets the base price from this vendor
    /// </summary>
    public decimal BasePrice { get; set; }

    /// <summary>
    /// Gets or sets the currency
    /// </summary>
    public string Currency { get; set; }

    /// <summary>
    /// Gets or sets the minimum order quantity
    /// </summary>
    public int MinOrderQuantity { get; set; }

    /// <summary>
    /// Gets or sets the lead time in days
    /// </summary>
    public int? LeadTimeDays { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this association is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets the pricing tiers for volume discounts
    /// </summary>
    public ICollection<VendorPricing> PricingTiers { get; set; }
}

