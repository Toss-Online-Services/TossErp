using POS.Domain.Common;
using POS.Domain.Common.ValueObjects;
using POS.Domain.Models;
using POS.Domain.AggregatesModel.SaleAggregate.Events;

namespace POS.Domain.AggregatesModel.SaleAggregate;

public class SaleItem : Entity
{
    public Guid SaleId { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; }
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public decimal Discount { get; private set; }
    public decimal TotalPrice { get; private set; }
    public decimal TaxRate { get; private set; }
    public SaleItemStatus Status { get; private set; }
    public decimal Total => TotalPrice;

    private SaleItem()
    {
        ProductName = string.Empty;
        UnitPrice = 0;
        Quantity = 0;
        Discount = 0;
        TotalPrice = 0;
        TaxRate = 0;
        Status = SaleItemStatus.Active;
    }

    public SaleItem(Guid id, Guid saleId, Guid productId, string productName, decimal unitPrice, int quantity, decimal taxRate)
    {
        if (id == Guid.Empty)
            throw new DomainException("Item ID cannot be empty");
        if (saleId == Guid.Empty)
            throw new DomainException("Sale ID cannot be empty");
        if (productId == Guid.Empty)
            throw new DomainException("Product ID cannot be empty");
        if (string.IsNullOrWhiteSpace(productName))
            throw new DomainException("Product name cannot be empty");
        if (unitPrice < 0)
            throw new DomainException("Unit price cannot be negative");
        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero");
        if (taxRate < 0 || taxRate > 100)
            throw new DomainException("Tax rate must be between 0 and 100");

        Id = id;
        SaleId = saleId;
        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;
        TaxRate = taxRate;
        Status = SaleItemStatus.Active;
        TotalPrice = CalculateTotalPrice();
    }

    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new DomainException("Quantity must be greater than zero");

        var oldQuantity = Quantity;
        Quantity = newQuantity;
        TotalPrice = CalculateTotalPrice();
    }

    public void ApplyDiscount(decimal discountAmount)
    {
        if (discountAmount < 0)
            throw new DomainException("Discount cannot be negative");

        if (discountAmount > TotalPrice)
            throw new DomainException("Discount cannot be greater than total price");

        Discount = discountAmount;
        TotalPrice = CalculateTotalPrice();
    }

    public void Remove()
    {
        if (Status == SaleItemStatus.Removed)
            throw new DomainException("Item is already removed");

        Status = SaleItemStatus.Removed;
    }

    private decimal CalculateTotalPrice()
    {
        return (UnitPrice * Quantity) - Discount;
    }
} 
