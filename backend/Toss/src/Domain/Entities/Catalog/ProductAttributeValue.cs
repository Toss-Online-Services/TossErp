namespace Toss.Domain.Entities.Catalog;

/// <summary>
/// Represents a product attribute value (e.g., "Red", "Large", "500ml")
/// </summary>
public class ProductAttributeValue : BaseEntity
{
    public ProductAttributeValue()
    {
        Name = string.Empty;
        PriceAdjustment = 0;
        DisplayOrder = 0;
    }

    /// <summary>
    /// Gets or sets the product attribute ID
    /// </summary>
    public int ProductAttributeId { get; set; }
    public ProductAttribute? ProductAttribute { get; set; }

    /// <summary>
    /// Gets or sets the name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the price adjustment
    /// </summary>
    public decimal PriceAdjustment { get; set; }

    /// <summary>
    /// Gets or sets the display order
    /// </summary>
    public int DisplayOrder { get; set; }
}

