using System;
using TossErp.POS.Domain.SeedWork;

namespace TossErp.POS.Domain.AggregatesModel.SaleAggregate;

public class SaleItem : Entity
{
    public int SaleId { get; private set; }
    public int ProductId { get; private set; }
    public string ProductName { get; private set; }
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public decimal Discount { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public decimal TotalPrice => (UnitPrice * Quantity) - Discount;

    protected SaleItem()
    {
        ProductName = string.Empty;
        CreatedAt = DateTime.UtcNow;
    }

    public SaleItem(int saleId, int productId, string productName, decimal unitPrice, int quantity, decimal discount = 0)
    {
        if (saleId <= 0)
            throw new DomainException("Sale ID must be greater than zero");
        if (productId <= 0)
            throw new DomainException("Product ID must be greater than zero");
        if (string.IsNullOrWhiteSpace(productName))
            throw new DomainException("Product name cannot be empty");
        if (unitPrice < 0)
            throw new DomainException("Unit price cannot be negative");
        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero");
        if (discount < 0)
            throw new DomainException("Discount cannot be negative");

        SaleId = saleId;
        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;
        Discount = discount;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(int quantity, decimal discount)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero");
        if (discount < 0)
            throw new DomainException("Discount cannot be negative");

        Quantity = quantity;
        Discount = discount;
        UpdatedAt = DateTime.UtcNow;
    }

    public decimal GetTotalPrice()
    {
        return (UnitPrice * Quantity) - Discount;
    }
} 
