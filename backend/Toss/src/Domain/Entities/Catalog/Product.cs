using Toss.Domain.Common;
using Toss.Domain.Entities.Localization;

namespace Toss.Domain.Entities.Catalog;

/// <summary>
/// Represents a product (merged from Inventory and Catalog)
/// </summary>
public class Product : BaseAuditableEntity, IMetaTagsSupported, ILocalizedEntity, IBusinessScopedEntity
{
    public Product()
    {
        SKU = string.Empty;
        Name = string.Empty;
        Currency = "ZAR";
        MinimumStockLevel = 10;
        IsActive = true;
        IsTaxable = true;
        StockLevels = new List<StockLevel>();
        StockMovements = new List<StockMovement>();
        ProductReviews = new List<ProductReview>();
        ProductProductTagMappings = new List<ProductProductTagMapping>();
    }

    /// <summary>
    /// Gets or sets the SKU (Stock Keeping Unit)
    /// </summary>
    public string SKU { get; set; }

    /// <summary>
    /// Gets or sets the barcode
    /// </summary>
    public string? Barcode { get; set; }

    /// <summary>
    /// Gets or sets the product name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the product description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the business owner of this product definition.
    /// </summary>
    public int BusinessId { get; set; }
    public Business Business { get; set; } = null!;

    /// <summary>
    /// Gets or sets the category ID
    /// </summary>
    public int? CategoryId { get; set; }
    public ProductCategory? Category { get; set; }

    // Pricing
    /// <summary>
    /// Gets or sets the base selling price
    /// </summary>
    public decimal BasePrice { get; set; }

    /// <summary>
    /// Gets or sets the cost price (for margin calculation)
    /// </summary>
    public decimal? CostPrice { get; set; }

    /// <summary>
    /// Gets or sets the currency code
    /// </summary>
    public string Currency { get; set; }

    // Inventory Management
    /// <summary>
    /// Gets or sets the unit of measure (e.g., "piece", "kg", "liter")
    /// </summary>
    public string? Unit { get; set; }

    /// <summary>
    /// Gets or sets the minimum stock level before reorder alert
    /// </summary>
    public int MinimumStockLevel { get; set; }

    /// <summary>
    /// Gets or sets the quantity to reorder when stock is low
    /// </summary>
    public int? ReorderQuantity { get; set; }

    // Status
    /// <summary>
    /// Gets or sets whether the product is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets whether the product is taxable
    /// </summary>
    public bool IsTaxable { get; set; }

    // Stock Management Relationships
    /// <summary>
    /// Gets or sets the stock levels across different shops
    /// </summary>
    public ICollection<StockLevel> StockLevels { get; set; }

    /// <summary>
    /// Gets or sets the stock movements (transactions)
    /// </summary>
    public ICollection<StockMovement> StockMovements { get; set; }

    // Catalog Relationships
    /// <summary>
    /// Gets or sets the product reviews
    /// </summary>
    public ICollection<ProductReview> ProductReviews { get; set; }

    /// <summary>
    /// Gets or sets the product-tag mappings
    /// </summary>
    public ICollection<ProductProductTagMapping> ProductProductTagMappings { get; set; }

    // SEO Properties
    /// <summary>
    /// Gets or sets the meta title for SEO
    /// </summary>
    public string? MetaTitle { get; set; }

    /// <summary>
    /// Gets or sets the meta keywords for SEO
    /// </summary>
    public string? MetaKeywords { get; set; }

    /// <summary>
    /// Gets or sets the meta description for SEO
    /// </summary>
    public string? MetaDescription { get; set; }
}

