using TossErp.Procurement.Domain.Common;
using TossErp.Procurement.Domain.ValueObjects;

namespace TossErp.Procurement.Domain.Entities;

/// <summary>
/// Purchase order item entity representing a line item in a purchase order
/// </summary>
public class PurchaseOrderItem : Entity<Guid>
{
    public Guid PurchaseOrderId { get; private set; }
    public Guid ItemId { get; private set; }
    public string ItemName { get; private set; } = string.Empty;
    public string ItemSku { get; private set; } = string.Empty;
    public decimal Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TaxRate { get; private set; }
    public decimal ReceivedQuantity { get; private set; }
    public decimal? DiscountPercentage { get; private set; }
    public string? Notes { get; private set; }
    public DateTime? ExpectedDeliveryDate { get; private set; }

    protected PurchaseOrderItem() : base() { } // For EF Core

    public PurchaseOrderItem(Guid id, Guid purchaseOrderId, Guid itemId, string itemName, string itemSku, 
        decimal quantity, decimal unitPrice, decimal taxRate = 0.15m) : base(id)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

        if (unitPrice < 0)
            throw new ArgumentException("Unit price cannot be negative", nameof(unitPrice));

        if (taxRate < 0 || taxRate > 1)
            throw new ArgumentException("Tax rate must be between 0 and 1", nameof(taxRate));

        PurchaseOrderId = purchaseOrderId;
        ItemId = itemId;
        ItemName = itemName ?? throw new ArgumentNullException(nameof(itemName));
        ItemSku = itemSku ?? throw new ArgumentNullException(nameof(itemSku));
        Quantity = quantity;
        UnitPrice = unitPrice;
        TaxRate = taxRate;
        ReceivedQuantity = 0;
        TenantId = "default-tenant"; // Will be set by parent PurchaseOrder
    }

    /// <summary>
    /// Create a new purchase order item
    /// </summary>
    public static PurchaseOrderItem Create(Guid purchaseOrderId, Guid itemId, string itemName, string itemSku, 
        decimal quantity, decimal unitPrice, decimal taxRate = 0.15m)
    {
        return new PurchaseOrderItem(Guid.NewGuid(), purchaseOrderId, itemId, itemName, itemSku, quantity, unitPrice, taxRate);
    }

    /// <summary>
    /// Update item quantity
    /// </summary>
    public void UpdateQuantity(decimal newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(newQuantity));

        if (newQuantity < ReceivedQuantity)
            throw new InvalidOperationException("Cannot reduce quantity below received quantity");

        Quantity = newQuantity;
    }

    /// <summary>
    /// Update unit price
    /// </summary>
    public void UpdateUnitPrice(decimal newUnitPrice)
    {
        if (newUnitPrice < 0)
            throw new ArgumentException("Unit price cannot be negative", nameof(newUnitPrice));

        UnitPrice = newUnitPrice;
    }

    /// <summary>
    /// Update tax rate
    /// </summary>
    public void UpdateTaxRate(decimal newTaxRate)
    {
        if (newTaxRate < 0 || newTaxRate > 1)
            throw new ArgumentException("Tax rate must be between 0 and 1", nameof(newTaxRate));

        TaxRate = newTaxRate;
    }

    /// <summary>
    /// Set discount percentage
    /// </summary>
    public void SetDiscountPercentage(decimal? discountPercentage)
    {
        if (discountPercentage.HasValue && (discountPercentage.Value < 0 || discountPercentage.Value > 100))
            throw new ArgumentException("Discount percentage must be between 0 and 100", nameof(discountPercentage));

        DiscountPercentage = discountPercentage;
    }

    /// <summary>
    /// Set expected delivery date
    /// </summary>
    public void SetExpectedDeliveryDate(DateTime? expectedDeliveryDate)
    {
        ExpectedDeliveryDate = expectedDeliveryDate;
    }

    /// <summary>
    /// Receive items
    /// </summary>
    public void ReceiveItems(decimal receivedQty)
    {
        if (receivedQty <= 0)
            throw new ArgumentException("Received quantity must be greater than zero", nameof(receivedQty));

        if (ReceivedQuantity + receivedQty > Quantity)
            throw new InvalidOperationException("Cannot receive more items than ordered");

        ReceivedQuantity += receivedQty;
    }

    /// <summary>
    /// Add notes
    /// </summary>
    public void AddNotes(string notes)
    {
        Notes = notes;
    }

    /// <summary>
    /// Set tenant ID for this entity
    /// </summary>
    internal void SetTenantId(string tenantId)
    {
        TenantId = tenantId;
    }

    /// <summary>
    /// Calculate line total (quantity * unit price)
    /// </summary>
    public decimal LineTotal => Quantity * UnitPrice;

    /// <summary>
    /// Calculate discount amount
    /// </summary>
    public decimal DiscountAmount => DiscountPercentage.HasValue ? LineTotal * (DiscountPercentage.Value / 100) : 0;

    /// <summary>
    /// Calculate subtotal after discount
    /// </summary>
    public decimal SubtotalAfterDiscount => LineTotal - DiscountAmount;

    /// <summary>
    /// Calculate tax amount
    /// </summary>
    public decimal TaxAmount => SubtotalAfterDiscount * TaxRate;

    /// <summary>
    /// Calculate total amount for this line
    /// </summary>
    public decimal TotalAmount => SubtotalAfterDiscount + TaxAmount;

    /// <summary>
    /// Check if item is fully received
    /// </summary>
    public bool IsFullyReceived => ReceivedQuantity >= Quantity;

    /// <summary>
    /// Get remaining quantity to receive
    /// </summary>
    public decimal RemainingQuantity => Quantity - ReceivedQuantity;
}
