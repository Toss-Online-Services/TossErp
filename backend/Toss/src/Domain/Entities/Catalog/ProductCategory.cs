using Toss.Domain.Common;
using Toss.Domain.Entities.Localization;

namespace Toss.Domain.Entities.Catalog;

/// <summary>
/// Represents a product category with hierarchical support
/// </summary>
public class ProductCategory : BaseAuditableEntity, IMetaTagsSupported, ILocalizedEntity
{
    public ProductCategory()
    {
        Name = string.Empty;
        IsActive = true;
        SubCategories = new List<ProductCategory>();
        Products = new List<Product>();
    }

    /// <summary>
    /// Gets or sets the category name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the category description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the parent category ID for hierarchical categories
    /// </summary>
    public int? ParentCategoryId { get; set; }
    public ProductCategory? ParentCategory { get; set; }

    /// <summary>
    /// Gets or sets whether the category is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets the sub-categories under this category
    /// </summary>
    public ICollection<ProductCategory> SubCategories { get; set; }

    /// <summary>
    /// Gets or sets the products in this category
    /// </summary>
    public ICollection<Product> Products { get; set; }

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

