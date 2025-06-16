using POS.Domain.Common.ValueObjects;
using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.SaleAggregate;

public class SaleItem : Entity
{
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; }
    public Money UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public decimal TaxRate { get; private set; }
    public Money Subtotal { get; private set; }
    public Money TaxAmount { get; private set; }
    public Money Total { get; private set; }

    private SaleItem() { } // For EF Core

    public SaleItem(Guid productId, string productName, decimal unitPrice, int quantity, decimal taxRate, string currency)
    {
        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("Product name cannot be empty", nameof(productName));
        if (unitPrice < 0)
            throw new ArgumentException("Unit price cannot be negative", nameof(unitPrice));
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));
        if (taxRate < 0 || taxRate > 100)
            throw new ArgumentException("Tax rate must be between 0 and 100", nameof(taxRate));

        ProductId = productId;
        ProductName = productName;
        UnitPrice = new Money(unitPrice, currency);
        Quantity = quantity;
        TaxRate = taxRate;

        CalculateTotals();
    }

    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(newQuantity));

        Quantity = newQuantity;
        CalculateTotals();
    }

    public void UpdateUnitPrice(decimal newUnitPrice)
    {
        if (newUnitPrice < 0)
            throw new ArgumentException("Unit price cannot be negative", nameof(newUnitPrice));

        UnitPrice = new Money(newUnitPrice, UnitPrice.Currency);
        CalculateTotals();
    }

    private void CalculateTotals()
    {
        Subtotal = UnitPrice * Quantity;
        TaxAmount = new Money(Subtotal.Amount * (TaxRate / 100), Subtotal.Currency);
        Total = Subtotal + TaxAmount;
    }
} 
