namespace Toss.Domain.Entities.Catalog;

/// <summary>
/// Represents a product attribute (e.g., Size, Color, Brand)
/// </summary>
public class ProductAttribute : BaseEntity
{
    public ProductAttribute()
    {
        Name = string.Empty;
        Description = string.Empty;
        ProductAttributeValues = new List<ProductAttributeValue>();
    }

    /// <summary>
    /// Gets or sets the name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the description
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the product attribute values
    /// </summary>
    public ICollection<ProductAttributeValue> ProductAttributeValues { get; set; }
}

