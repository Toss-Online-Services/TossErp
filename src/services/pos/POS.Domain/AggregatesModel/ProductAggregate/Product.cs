#nullable enable
using System;
using TossErp.POS.Domain.SeedWork;

namespace TossErp.POS.Domain.AggregatesModel.ProductAggregate;

public class Product : AggregateRoot
{
    public string StoreId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public string? Sku { get; set; }
    public decimal Price { get; set; }
    public decimal Cost { get; set; }
    public int StockQuantity { get; set; }
    public int LowStockThreshold { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public string Code { get; set; } = string.Empty;
    public int StockLevel { get; set; }

    protected Product()
    {
        Name = string.Empty;
        Description = string.Empty;
        CreatedAt = DateTime.UtcNow;
    }

    public Product(string name, string description, decimal price, int stockQuantity)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Name cannot be empty");
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Description cannot be empty");
        if (price <= 0)
            throw new DomainException("Price must be greater than zero");
        if (stockQuantity < 0)
            throw new DomainException("Stock quantity cannot be negative");

        Name = name;
        Description = description;
        Price = price;
        StockQuantity = stockQuantity;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(string name, string description, decimal price, int stockQuantity)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Name cannot be empty");
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Description cannot be empty");
        if (price <= 0)
            throw new DomainException("Price must be greater than zero");
        if (stockQuantity < 0)
            throw new DomainException("Stock quantity cannot be negative");

        Name = name;
        Description = description;
        Price = price;
        StockQuantity = stockQuantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateStock(int quantity)
    {
        StockQuantity = quantity;
        StockLevel = StockQuantity;
        UpdatedAt = DateTime.UtcNow;
    }
} 
