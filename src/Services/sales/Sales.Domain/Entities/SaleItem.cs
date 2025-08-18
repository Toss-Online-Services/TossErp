using TossErp.Sales.Domain.Common;
using TossErp.Sales.Domain.ValueObjects;

namespace TossErp.Sales.Domain.Entities;

/// <summary>
/// Sale item entity representing a line item in a sale
/// </summary>
public class SaleItem : Entity<Guid>
{
    public Guid ItemId { get; private set; }
    public string ItemName { get; private set; } = string.Empty;
    public string ItemSku { get; private set; } = string.Empty;
    public decimal Quantity { get; private set; }
    public Money UnitPrice { get; private set; } = Money.Zero();
    public decimal TaxRate { get; private set; }
    public Money LineTotal { get; private set; } = Money.Zero();
    public Money TaxAmount { get; private set; } = Money.Zero();
    public Money LineTotalIncludingTax { get; private set; } = Money.Zero();

    protected SaleItem() : base() { } // For EF Core

    public SaleItem(Guid id, Guid itemId, string itemName, string itemSku, decimal quantity, Money unitPrice, decimal taxRate = 0.15m) : base(id)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        if (unitPrice.Amount < 0)
            throw new ArgumentException("Unit price cannot be negative", nameof(unitPrice));

        if (taxRate < 0 || taxRate > 1)
            throw new ArgumentException("Tax rate must be between 0 and 1", nameof(taxRate));

        ItemId = itemId;
        ItemName = itemName ?? throw new ArgumentNullException(nameof(itemName));
        ItemSku = itemSku ?? throw new ArgumentNullException(nameof(itemSku));
        Quantity = quantity;
        UnitPrice = unitPrice ?? throw new ArgumentNullException(nameof(unitPrice));
        TaxRate = taxRate;
        TenantId = "default-tenant"; // Will be set by parent Sale

        CalculateAmounts();
    }

    /// <summary>
    /// Update the quantity for this line item
    /// </summary>
    public void UpdateQuantity(decimal newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(newQuantity));

        Quantity = newQuantity;
        CalculateAmounts();
    }

    /// <summary>
    /// Update the unit price for this line item
    /// </summary>
    public void UpdateUnitPrice(Money newUnitPrice)
    {
        if (newUnitPrice.Amount < 0)
            throw new ArgumentException("Unit price cannot be negative", nameof(newUnitPrice));

        UnitPrice = newUnitPrice ?? throw new ArgumentNullException(nameof(newUnitPrice));
        CalculateAmounts();
    }

    /// <summary>
    /// Update the tax rate for this line item
    /// </summary>
    public void UpdateTaxRate(decimal newTaxRate)
    {
        if (newTaxRate < 0 || newTaxRate > 1)
            throw new ArgumentException("Tax rate must be between 0 and 1", nameof(newTaxRate));

        TaxRate = newTaxRate;
        CalculateAmounts();
    }

    /// <summary>
    /// Set tenant ID for this entity
    /// </summary>
    internal void SetTenantId(string tenantId)
    {
        TenantId = tenantId;
    }

    /// <summary>
    /// Calculate line totals and tax amounts
    /// </summary>
    private void CalculateAmounts()
    {
        LineTotal = UnitPrice * (decimal)Quantity;
        TaxAmount = LineTotal * (decimal)TaxRate;
        LineTotalIncludingTax = LineTotal + TaxAmount;
    }
}
