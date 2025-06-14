using System;
using TossErp.POS.Domain.Common;

namespace TossErp.POS.Domain.AggregatesModel.SaleAggregate;

public class SaleItem : Entity
{
    public int SaleId { get; private set; }
    public int ProductId { get; private set; }
    public string ProductName { get; private set; }
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public decimal TotalAmount { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    protected SaleItem() { }

    public SaleItem(int productId, string productName, decimal unitPrice, int quantity)
    {
        if (productId <= 0)
            throw new DomainException("Product ID must be greater than zero");

        if (string.IsNullOrWhiteSpace(productName))
            throw new DomainException("Product name cannot be empty");

        if (unitPrice < 0)
            throw new DomainException("Unit price cannot be negative");

        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero");

        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;
        TotalAmount = unitPrice * quantity;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero");

        Quantity = quantity;
        TotalAmount = UnitPrice * quantity;
        UpdatedAt = DateTime.UtcNow;
    }
} 
