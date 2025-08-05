using TossErp.Stock.Domain.Common;

namespace TossErp.Stock.Domain.Aggregates.ItemAggregate.Entities;

/// <summary>
/// Item Supplier - Child entity of Item Aggregate
/// Represents supplier relationship for an item
/// </summary>
public class ItemSupplier : Entity
{
    public Guid SupplierId { get; private set; }
    public string SupplierItemCode { get; private set; } = string.Empty;
    public string? SupplierItemName { get; private set; }
    public decimal? LastPurchaseRate { get; private set; }
    public decimal? MinimumOrderQty { get; private set; }
    public int? LeadTimeDays { get; private set; }
    public bool IsPreferred { get; private set; }
    public bool IsActive { get; private set; }

    protected ItemSupplier() { } // For EF Core

    public ItemSupplier(
        Guid supplierId, 
        string supplierItemCode, 
        string? supplierItemName = null,
        decimal? lastPurchaseRate = null,
        decimal? minimumOrderQty = null,
        int? leadTimeDays = null,
        bool isPreferred = false)
    {
        if (supplierId == Guid.Empty)
            throw new ArgumentException("Supplier ID cannot be empty", nameof(supplierId));
        if (string.IsNullOrWhiteSpace(supplierItemCode))
            throw new ArgumentException("Supplier item code cannot be empty", nameof(supplierItemCode));

        SupplierId = supplierId;
        SupplierItemCode = supplierItemCode.Trim();
        SupplierItemName = supplierItemName?.Trim();
        LastPurchaseRate = lastPurchaseRate;
        MinimumOrderQty = minimumOrderQty;
        LeadTimeDays = leadTimeDays;
        IsPreferred = isPreferred;
        IsActive = true;
    }

    public void UpdateSupplierInfo(string supplierItemCode, string? supplierItemName)
    {
        if (string.IsNullOrWhiteSpace(supplierItemCode))
            throw new ArgumentException("Supplier item code cannot be empty", nameof(supplierItemCode));

        SupplierItemCode = supplierItemCode.Trim();
        SupplierItemName = supplierItemName?.Trim();
    }

    public void UpdatePricing(decimal? lastPurchaseRate, decimal? minimumOrderQty)
    {
        if (lastPurchaseRate.HasValue && lastPurchaseRate.Value < 0)
            throw new ArgumentException("Last purchase rate cannot be negative", nameof(lastPurchaseRate));
        if (minimumOrderQty.HasValue && minimumOrderQty.Value < 0)
            throw new ArgumentException("Minimum order quantity cannot be negative", nameof(minimumOrderQty));

        LastPurchaseRate = lastPurchaseRate;
        MinimumOrderQty = minimumOrderQty;
    }

    public void UpdateLeadTime(int? leadTimeDays)
    {
        if (leadTimeDays.HasValue && leadTimeDays.Value < 0)
            throw new ArgumentException("Lead time cannot be negative", nameof(leadTimeDays));

        LeadTimeDays = leadTimeDays;
    }

    public void SetAsPreferred()
    {
        IsPreferred = true;
    }

    public void RemovePreferredStatus()
    {
        IsPreferred = false;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public bool IsAvailable() => IsActive;

    public bool IsPreferredSupplier() => IsPreferred && IsActive;
} 
