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
    public SaleItemStatus Status { get; private set; }

    private SaleItem() { } // For EF

    public SaleItem(
        Guid id,
        Guid saleId,
        Guid productId,
        string productName,
        decimal unitPrice,
        int quantity,
        decimal discount = 0) : base(id)
    {
        SaleId = saleId;
        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;
        Discount = discount;
        TotalPrice = CalculateTotalPrice();
        Status = SaleItemStatus.Active;
    }

    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new DomainException("Quantity must be greater than zero");

        var oldQuantity = Quantity;
        Quantity = newQuantity;
        TotalPrice = CalculateTotalPrice();

        AddDomainEvent(new SaleItemQuantityChangedDomainEvent(
            SaleId,
            Id,
            oldQuantity,
            newQuantity,
            DateTime.UtcNow));
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

        AddDomainEvent(new SaleItemRemovedDomainEvent(
            SaleId,
            Id,
            "Item removed from sale",
            DateTime.UtcNow));
    }

    private decimal CalculateTotalPrice()
    {
        return (UnitPrice * Quantity) - Discount;
    }
} 
