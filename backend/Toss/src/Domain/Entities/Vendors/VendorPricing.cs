namespace Toss.Domain.Entities.Vendors;

/// <summary>
/// Represents vendor pricing tiers for volume discounts
/// </summary>
public class VendorPricing : BaseAuditableEntity
{
    public VendorPricing()
    {
        MinQuantity = 1;
        IsActive = true;
    }

    /// <summary>
    /// Gets or sets the vendor product ID
    /// </summary>
    public int VendorProductId { get; set; }
    public VendorProduct VendorProduct { get; set; } = null!;

    /// <summary>
    /// Gets or sets the minimum quantity for this price tier
    /// </summary>
    public int MinQuantity { get; set; }

    /// <summary>
    /// Gets or sets the maximum quantity for this price tier
    /// </summary>
    public int? MaxQuantity { get; set; }

    /// <summary>
    /// Gets or sets the unit price at this tier
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Gets or sets the discount percentage at this tier
    /// </summary>
    public decimal? DiscountPercentage { get; set; }

    /// <summary>
    /// Gets or sets the valid from date
    /// </summary>
    public DateTimeOffset? ValidFrom { get; set; }

    /// <summary>
    /// Gets or sets the valid to date
    /// </summary>
    public DateTimeOffset? ValidTo { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this pricing tier is active
    /// </summary>
    public bool IsActive { get; set; }

    // Aliases for compatibility
    public decimal BasePrice
    {
        get => UnitPrice;
        set => UnitPrice = value;
    }

    public DateTimeOffset? EffectiveDate
    {
        get => ValidFrom;
        set => ValidFrom = value;
    }
}

