using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.ProductAggregate;

/// <summary>
/// Represents a product category in the POS system
/// </summary>
public class ProductCategory : Entity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public bool IsActive { get; private set; }
    public Guid? ParentCategoryId { get; private set; }
    public ProductCategory? ParentCategory { get; private set; }
    private readonly List<ProductCategory> _subCategories = new();
    public IReadOnlyCollection<ProductCategory> SubCategories => _subCategories.AsReadOnly();

    private ProductCategory() { Name = string.Empty; Description = string.Empty; }

    public ProductCategory(string name, string description, ProductCategory? parentCategory = null)
    {
        Name = Guard.Against.NullOrWhiteSpace(name, nameof(name));
        Description = Guard.Against.NullOrWhiteSpace(description, nameof(description));
        ParentCategory = parentCategory;
        ParentCategoryId = parentCategory?.Id;
        IsActive = true;
    }

    public void UpdateDetails(string name, string description)
    {
        Name = Guard.Against.NullOrWhiteSpace(name, nameof(name));
        Description = Guard.Against.NullOrWhiteSpace(description, nameof(description));
    }

    public void Deactivate()
    {
        if (!IsActive) return;
        IsActive = false;
    }

    public void Activate()
    {
        if (IsActive) return;
        IsActive = true;
    }

    public void SetParentCategory(ProductCategory? parentCategory)
    {
        ParentCategory = parentCategory;
        ParentCategoryId = parentCategory?.Id;
    }

    public void AddSubCategory(ProductCategory subCategory)
    {
        Guard.Against.Null(subCategory, nameof(subCategory));
        _subCategories.Add(subCategory);
    }
} 
