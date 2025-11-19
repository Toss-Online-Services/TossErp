namespace Toss.Domain.Entities.Catalog;

/// <summary>
/// Represents a product tag for easier discovery
/// </summary>
public class ProductTag : BaseEntity
{
    public ProductTag()
    {
        Name = string.Empty;
        ProductProductTagMappings = new List<ProductProductTagMapping>();
    }

    /// <summary>
    /// Gets or sets the name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the product mappings
    /// </summary>
    public ICollection<ProductProductTagMapping> ProductProductTagMappings { get; set; }
}

