using TossErp.Domain.Common;

namespace TossErp.Domain.Entities.Inventory;

public enum ProductType
{
    Product,
    Service,
    Bundle
}

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Sku { get; set; }
    public string? Barcode { get; set; }
    public string? Description { get; set; }
    
    public ProductType Type { get; set; } = ProductType.Product;
    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    
    // Pricing (stored in cents)
    public decimal CostPrice { get; set; }
    public decimal SellingPrice { get; set; }
    public decimal? WholesalePrice { get; set; }
    
    // Inventory tracking
    public bool TrackInventory { get; set; } = true;
    public int? ReorderPoint { get; set; }
    public int? ReorderQuantity { get; set; }
    
    // Physical attributes
    public decimal? Weight { get; set; }
    public string? WeightUnit { get; set; }
    public string? Dimensions { get; set; }
    
    // Tax and accounting
    public bool IsTaxable { get; set; } = true;
    public decimal? TaxRate { get; set; }
    public string? TaxCategory { get; set; }
    
    // Status
    public bool IsActive { get; set; } = true;
    public bool IsFeatured { get; set; }
    
    // Images and media
    public string? ImageUrl { get; set; }
    public List<string> AdditionalImages { get; set; } = new();
    
    // Navigation properties
    public virtual ICollection<StockLevel> StockLevels { get; set; } = new List<StockLevel>();
    public virtual ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
    
    // Business logic
    public void UpdatePrice(decimal newSellingPrice, string updatedBy)
    {
        if (newSellingPrice <= 0)
            throw new ArgumentException("Selling price must be greater than zero", nameof(newSellingPrice));
        
        var oldPrice = SellingPrice;
        SellingPrice = newSellingPrice;
        UpdatedBy = updatedBy;
        UpdatedAt = DateTime.UtcNow;
        
        AddDomainEvent(new ProductPriceChangedEvent(Id, Name, oldPrice, newSellingPrice));
    }
    
    public void Activate()
    {
        IsActive = true;
        AddDomainEvent(new ProductActivatedEvent(Id, Name));
    }
    
    public void Deactivate()
    {
        IsActive = false;
        AddDomainEvent(new ProductDeactivatedEvent(Id, Name));
    }
}

// Domain Events
public class ProductPriceChangedEvent : DomainEvent
{
    public int ProductId { get; }
    public string ProductName { get; }
    public decimal OldPrice { get; }
    public decimal NewPrice { get; }
    
    public ProductPriceChangedEvent(int productId, string productName, decimal oldPrice, decimal newPrice)
    {
        ProductId = productId;
        ProductName = productName;
        OldPrice = oldPrice;
        NewPrice = newPrice;
    }
}

public class ProductActivatedEvent : DomainEvent
{
    public int ProductId { get; }
    public string ProductName { get; }
    
    public ProductActivatedEvent(int productId, string productName)
    {
        ProductId = productId;
        ProductName = productName;
    }
}

public class ProductDeactivatedEvent : DomainEvent
{
    public int ProductId { get; }
    public string ProductName { get; }
    
    public ProductDeactivatedEvent(int productId, string productName)
    {
        ProductId = productId;
        ProductName = productName;
    }
}

