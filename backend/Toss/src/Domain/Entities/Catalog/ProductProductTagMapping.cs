namespace Toss.Domain.Entities.Catalog;

/// <summary>
/// Represents a product-tag mapping
/// </summary>
public class ProductProductTagMapping : BaseEntity
{
    /// <summary>
    /// Gets or sets the product ID
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Gets or sets the product tag ID
    /// </summary>
    public int ProductTagId { get; set; }
    public ProductTag? ProductTag { get; set; }
}

