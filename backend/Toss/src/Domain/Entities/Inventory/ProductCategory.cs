namespace Toss.Domain.Entities.Inventory;

public class ProductCategory : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ParentCategoryId { get; set; }
    public ProductCategory? ParentCategory { get; set; }
    public bool IsActive { get; set; } = true;
    
    public ICollection<ProductCategory> SubCategories { get; private set; } = new List<ProductCategory>();
    public ICollection<Product> Products { get; private set; } = new List<Product>();
}

