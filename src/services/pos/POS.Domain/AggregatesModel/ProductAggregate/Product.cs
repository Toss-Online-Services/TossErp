#nullable enable
using POS.Domain.AggregatesModel.StoreAggregate;
using POS.Domain.SeedWork;
using POS.Domain.Common;
using POS.Domain.Events;
using POS.Domain.Models;
using POS.Domain.AggregatesModel.ProductAggregate.Events;
using POS.Domain.Common.Events;
using POS.Domain.ValueObjects;

namespace POS.Domain.AggregatesModel.ProductAggregate;

/// <summary>
/// Represents a product in the POS system
/// </summary>
public class Product : Entity, IAggregateRoot
{
    public Guid StoreId { get; private set; }
    public Store Store { get; private set; } = null!;
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string SKU { get; private set; }
    public string Barcode { get; private set; }
    public Money Price { get; private set; }
    public decimal CostPrice { get; private set; }
    public int CategoryId { get; private set; }
    public bool IsActive { get; private set; }
    public int StockQuantity { get; private set; }
    public int LowStockThreshold { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public ProductCategory Category { get; private set; }

    private Product()
    {
        Name = string.Empty;
        Description = string.Empty;
        SKU = string.Empty;
        Barcode = string.Empty;
        Price = new Money(0, "USD");
        Category = new ProductCategory("Default", "Default Category");
        Store = null!; // Required for EF Core
    } // For EF Core

    public Product(
        string name,
        string description,
        string sku,
        string barcode,
        Money price,
        decimal costPrice,
        int categoryId,
        Guid storeId,
        int stockQuantity,
        int lowStockThreshold)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Product name cannot be empty");
        if (string.IsNullOrWhiteSpace(sku))
            throw new DomainException("Product SKU cannot be empty");
        if (string.IsNullOrWhiteSpace(barcode))
            throw new DomainException("Product barcode cannot be empty");
        Name = name;
        Description = description;
        SKU = sku;
        Barcode = barcode;
        Price = price;
        CostPrice = costPrice;
        CategoryId = categoryId;
        Category = new ProductCategory("Default", "Default Category"); // Initialize with default, will be updated by EF Core
        StoreId = storeId;
        StockQuantity = stockQuantity;
        LowStockThreshold = lowStockThreshold;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        AddDomainEvent(new ProductCreatedDomainEvent(this));
    }

    public void UpdateDetails(
        string name,
        string description,
        Money price,
        decimal costPrice,
        int categoryId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Product name cannot be empty");
        Name = name;
        Description = description;
        Price = price;
        CostPrice = costPrice;
        CategoryId = categoryId;
        LastModifiedAt = DateTime.UtcNow;
        AddDomainEvent(new ProductUpdatedDomainEvent(this));
    }

    public void UpdateStock(int quantity)
    {
        StockQuantity = Guard.Against.Negative(quantity, nameof(quantity));
        LastModifiedAt = DateTime.UtcNow;
        AddDomainEvent(new ProductStockUpdatedDomainEvent(this));
    }

    public void AdjustStock(int adjustment)
    {
        StockQuantity += adjustment;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void SetLowStockThreshold(int threshold)
    {
        LowStockThreshold = threshold;
        LastModifiedAt = DateTime.UtcNow;
    }

    public bool IsLowStock() => StockQuantity <= LowStockThreshold;

    public void Deactivate()
    {
        if (!IsActive)
            throw new DomainException("Product is already inactive");
        IsActive = false;
        LastModifiedAt = DateTime.UtcNow;
        AddDomainEvent(new ProductDeactivatedDomainEvent(this));
    }

    public void Activate()
    {
        if (IsActive)
            throw new DomainException("Product is already active");
        IsActive = true;
        LastModifiedAt = DateTime.UtcNow;
        AddDomainEvent(new ProductActivatedDomainEvent(this));
    }

    public void UpdatePrice(Money newPrice)
    {
        Price = Guard.Against.Null(newPrice, nameof(newPrice));
    }

    public void UpdateCategory(ProductCategory newCategory)
    {
        Category = Guard.Against.Null(newCategory, nameof(newCategory));
    }
} 
