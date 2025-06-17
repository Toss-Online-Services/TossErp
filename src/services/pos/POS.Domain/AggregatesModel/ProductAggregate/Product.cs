#nullable enable
using POS.Domain.AggregatesModel.StoreAggregate;
using POS.Domain.SeedWork;
using POS.Domain.Common.ValueObjects;
using POS.Domain.Events;
using POS.Domain.Common;
using POS.Domain.Models;
using POS.Domain.AggregatesModel.ProductAggregate.Events;
using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.ProductAggregate;

public class Product : Entity
{
    public Guid StoreId { get; private set; }
    public Store Store { get; private set; } = null!;
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string SKU { get; private set; }
    public string Barcode { get; private set; }
    public decimal Price { get; private set; }
    public decimal CostPrice { get; private set; }
    public int CategoryId { get; private set; }
    public bool IsActive { get; private set; }
    public int StockQuantity { get; private set; }
    public int LowStockThreshold { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }

    private Product()
    {
        Name = string.Empty;
        Description = string.Empty;
        SKU = string.Empty;
        Barcode = string.Empty;
    } // For EF Core

    public Product(
        string name,
        string description,
        string sku,
        string barcode,
        decimal price,
        decimal costPrice,
        int categoryId,
        Guid storeId,
        int stockQuantity,
        int lowStockThreshold)
    {
        Name = name;
        Description = description;
        SKU = sku;
        Barcode = barcode;
        Price = price;
        CostPrice = costPrice;
        CategoryId = categoryId;
        StoreId = storeId;
        StockQuantity = stockQuantity;
        LowStockThreshold = lowStockThreshold;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        AddDomainEvent(new ProductCreatedDomainEvent(Id, name, sku));
    }

    public void UpdateDetails(
        string name,
        string description,
        decimal price,
        decimal costPrice,
        int categoryId)
    {
        Name = name;
        Description = description;
        Price = price;
        CostPrice = costPrice;
        CategoryId = categoryId;
        LastModifiedAt = DateTime.UtcNow;
        AddDomainEvent(new ProductUpdatedDomainEvent(Id, name, description, SKU, Barcode));
    }

    public void UpdateStock(int quantity)
    {
        StockQuantity = quantity;
        LastModifiedAt = DateTime.UtcNow;
        AddDomainEvent(new ProductStockUpdatedDomainEvent(Id, quantity));
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
        IsActive = false;
        LastModifiedAt = DateTime.UtcNow;
        AddDomainEvent(new ProductDeactivatedDomainEvent(Id));
    }

    public void Reactivate()
    {
        IsActive = true;
        LastModifiedAt = DateTime.UtcNow;
        AddDomainEvent(new ProductReactivatedDomainEvent(Id));
    }

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        // Implementation of AddDomainEvent method
    }
} 
