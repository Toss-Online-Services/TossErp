#nullable enable
using POS.Domain.AggregatesModel.StoreAggregate;
using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.ProductAggregate;

public class Product : AggregateRoot
{
    public Guid StoreId { get; private set; }
    public Store Store { get; private set; } = null!;
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public string? Barcode { get; private set; }
    public string? SKU { get; private set; }
    public string? Category { get; private set; }
    public decimal Price { get; private set; }
    public decimal Cost { get; private set; }
    public decimal TaxRate { get; private set; }
    public decimal Stock { get; private set; }
    public decimal MinStock { get; private set; }
    public decimal MaxStock { get; private set; }
    public string? Unit { get; private set; }
    public string? Brand { get; private set; }
    public string? Supplier { get; private set; }
    public string? ImageUrl { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsSynced { get; private set; }
    public DateTime? SyncedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    protected Product()
    {
        Name = string.Empty;
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
        IsSynced = false;
    }

    public Product(Guid storeId, string name, decimal price, decimal cost, decimal taxRate, decimal stock,
        string? description = null, string? barcode = null, string? sku = null, string? category = null,
        decimal minStock = 0, decimal maxStock = 0, string? unit = null, string? brand = null,
        string? supplier = null, string? imageUrl = null)
    {
        if (storeId == Guid.Empty)
            throw new DomainException("Store ID cannot be empty");
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Name cannot be empty");
        if (price < 0)
            throw new DomainException("Price cannot be negative");
        if (cost < 0)
            throw new DomainException("Cost cannot be negative");
        if (taxRate < 0)
            throw new DomainException("Tax rate cannot be negative");
        if (stock < 0)
            throw new DomainException("Stock cannot be negative");
        if (minStock < 0)
            throw new DomainException("Minimum stock cannot be negative");
        if (maxStock < 0)
            throw new DomainException("Maximum stock cannot be negative");
        if (maxStock > 0 && minStock > maxStock)
            throw new DomainException("Minimum stock cannot be greater than maximum stock");

        StoreId = storeId;
        Name = name;
        Description = description;
        Barcode = barcode;
        SKU = sku;
        Category = category;
        Price = price;
        Cost = cost;
        TaxRate = taxRate;
        Stock = stock;
        MinStock = minStock;
        MaxStock = maxStock;
        Unit = unit;
        Brand = brand;
        Supplier = supplier;
        ImageUrl = imageUrl;
        IsActive = true;
        IsSynced = false;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(string name, decimal price, decimal cost, decimal taxRate,
        string? description = null, string? category = null, string? unit = null,
        string? brand = null, string? supplier = null, string? imageUrl = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Name cannot be empty");
        if (price < 0)
            throw new DomainException("Price cannot be negative");
        if (cost < 0)
            throw new DomainException("Cost cannot be negative");
        if (taxRate < 0)
            throw new DomainException("Tax rate cannot be negative");

        Name = name;
        Description = description;
        Category = category;
        Price = price;
        Cost = cost;
        TaxRate = taxRate;
        Unit = unit;
        Brand = brand;
        Supplier = supplier;
        ImageUrl = imageUrl;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateStock(decimal newStock)
    {
        if (newStock < 0)
            throw new DomainException("Stock cannot be negative");

        Stock = newStock;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateStockLevels(decimal minStock, decimal maxStock)
    {
        if (minStock < 0)
            throw new DomainException("Minimum stock cannot be negative");
        if (maxStock < 0)
            throw new DomainException("Maximum stock cannot be negative");
        if (maxStock > 0 && minStock > maxStock)
            throw new DomainException("Minimum stock cannot be greater than maximum stock");

        MinStock = minStock;
        MaxStock = maxStock;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        if (!IsActive)
            throw new DomainException("Product is already inactive");

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        if (IsActive)
            throw new DomainException("Product is already active");

        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsSynced()
    {
        IsSynced = true;
        SyncedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsOffline()
    {
        IsSynced = false;
        SyncedAt = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool HasLowStock()
    {
        return MinStock > 0 && Stock <= MinStock;
    }

    public bool HasExcessStock()
    {
        return MaxStock > 0 && Stock >= MaxStock;
    }

    public decimal GetProfit()
    {
        return Price - Cost;
    }

    public decimal GetProfitMargin()
    {
        if (Price == 0)
            return 0;
        return (Price - Cost) / Price;
    }
} 
