using System;
using TossErp.POS.Domain.Exceptions;
using TossErp.POS.Domain.SeedWork;

namespace TossErp.POS.Domain.AggregatesModel.SaleAggregate;

public class SaleItem : Entity
{
    public int SaleId { get; private set; }
    public int ProductId { get; private set; }
    public string ProductName { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TaxRate { get; private set; }
    public decimal SubTotal { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public decimal Discount { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    protected SaleItem()
    {
        ProductName = string.Empty;
        CreatedAt = DateTime.UtcNow;
    }

    public SaleItem(int productId, string productName, decimal quantity, decimal unitPrice, decimal taxRate)
    {
        if (string.IsNullOrWhiteSpace(productName))
            throw new DomainException("Product name cannot be empty");
        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero");
        if (unitPrice < 0)
            throw new DomainException("Unit price cannot be negative");
        if (taxRate < 0)
            throw new DomainException("Tax rate cannot be negative");

        ProductId = productId;
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
        TaxRate = taxRate;
        CalculateTotals();
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateQuantity(decimal newQuantity)
    {
        if (newQuantity <= 0)
            throw new DomainException("Quantity must be greater than zero");

        Quantity = newQuantity;
        CalculateTotals();
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateUnitPrice(decimal newUnitPrice)
    {
        if (newUnitPrice < 0)
            throw new DomainException("Unit price cannot be negative");

        UnitPrice = newUnitPrice;
        CalculateTotals();
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateTaxRate(decimal newTaxRate)
    {
        if (newTaxRate < 0)
            throw new DomainException("Tax rate cannot be negative");

        TaxRate = newTaxRate;
        CalculateTotals();
        UpdatedAt = DateTime.UtcNow;
    }

    private void CalculateTotals()
    {
        SubTotal = Quantity * UnitPrice;
        TaxAmount = SubTotal * (TaxRate / 100);
        TotalAmount = SubTotal + TaxAmount;
    }

    public decimal GetTotalPrice()
    {
        return (UnitPrice * Quantity) - Discount;
    }
} 
