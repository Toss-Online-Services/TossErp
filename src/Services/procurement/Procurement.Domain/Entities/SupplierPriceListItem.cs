using TossErp.Procurement.Domain.Common;

namespace TossErp.Procurement.Domain.Entities;

/// <summary>
/// Supplier price list item entity representing pricing information for a specific item from a supplier
/// </summary>
public class SupplierPriceListItem : Entity<Guid>
{
    public Guid SupplierPriceListId { get; private set; }
    public Guid ItemId { get; private set; }
    public string ItemName { get; private set; } = string.Empty;
    public string ItemSku { get; private set; } = string.Empty;
    public decimal UnitPrice { get; private set; }
    public string Currency { get; private set; } = "ZAR";
    public decimal? MinimumOrderQuantity { get; private set; }
    public int? LeadTimeDays { get; private set; }
    public string? Notes { get; private set; }
    public DateTime LastUpdated { get; private set; }

    protected SupplierPriceListItem() : base() { } // For EF Core

    public SupplierPriceListItem(Guid id, Guid supplierPriceListId, Guid itemId, string itemName, 
        string itemSku, decimal unitPrice, string currency = "ZAR", decimal? minimumOrderQuantity = null, 
        int? leadTimeDays = null, string? notes = null) : base(id)
    {
        if (unitPrice < 0)
            throw new ArgumentException("Unit price cannot be negative", nameof(unitPrice));

        if (minimumOrderQuantity.HasValue && minimumOrderQuantity.Value < 0)
            throw new ArgumentException("Minimum order quantity cannot be negative", nameof(minimumOrderQuantity));

        if (leadTimeDays.HasValue && leadTimeDays.Value < 0)
            throw new ArgumentException("Lead time days cannot be negative", nameof(leadTimeDays));

        SupplierPriceListId = supplierPriceListId;
        ItemId = itemId;
        ItemName = itemName ?? throw new ArgumentNullException(nameof(itemName));
        ItemSku = itemSku ?? throw new ArgumentNullException(nameof(itemSku));
        UnitPrice = unitPrice;
        Currency = currency ?? throw new ArgumentNullException(nameof(currency));
        MinimumOrderQuantity = minimumOrderQuantity;
        LeadTimeDays = leadTimeDays;
        Notes = notes;
        LastUpdated = DateTime.UtcNow;
        TenantId = "default-tenant"; // Will be set by parent SupplierPriceList
    }

    /// <summary>
    /// Create a new supplier price list item
    /// </summary>
    public static SupplierPriceListItem Create(Guid supplierPriceListId, Guid itemId, string itemName, 
        string itemSku, decimal unitPrice, string currency = "ZAR", decimal? minimumOrderQuantity = null, 
        int? leadTimeDays = null, string? notes = null)
    {
        return new SupplierPriceListItem(Guid.NewGuid(), supplierPriceListId, itemId, itemName, itemSku, 
            unitPrice, currency, minimumOrderQuantity, leadTimeDays, notes);
    }

    /// <summary>
    /// Update the price and related information
    /// </summary>
    public void UpdatePrice(decimal newUnitPrice, decimal? minimumOrderQuantity = null, 
        int? leadTimeDays = null, string? notes = null)
    {
        if (newUnitPrice < 0)
            throw new ArgumentException("Unit price cannot be negative", nameof(newUnitPrice));

        if (minimumOrderQuantity.HasValue && minimumOrderQuantity.Value < 0)
            throw new ArgumentException("Minimum order quantity cannot be negative", nameof(minimumOrderQuantity));

        if (leadTimeDays.HasValue && leadTimeDays.Value < 0)
            throw new ArgumentException("Lead time days cannot be negative", nameof(leadTimeDays));

        UnitPrice = newUnitPrice;
        MinimumOrderQuantity = minimumOrderQuantity;
        LeadTimeDays = leadTimeDays;
        Notes = notes;
        LastUpdated = DateTime.UtcNow;
    }

    /// <summary>
    /// Set tenant ID for this entity
    /// </summary>
    internal void SetTenantId(string tenantId)
    {
        TenantId = tenantId;
    }

    /// <summary>
    /// Check if the requested quantity meets the minimum order requirement
    /// </summary>
    public bool MeetsMinimumOrderQuantity(decimal requestedQuantity)
    {
        return !MinimumOrderQuantity.HasValue || requestedQuantity >= MinimumOrderQuantity.Value;
    }

    /// <summary>
    /// Calculate expected delivery date based on lead time
    /// </summary>
    public DateTime? CalculateExpectedDeliveryDate(DateTime? orderDate = null)
    {
        if (!LeadTimeDays.HasValue) return null;

        var baseDate = orderDate ?? DateTime.UtcNow;
        return baseDate.AddDays(LeadTimeDays.Value);
    }

    /// <summary>
    /// Calculate line total for a given quantity
    /// </summary>
    public decimal CalculateLineTotal(decimal quantity)
    {
        if (quantity < 0)
            throw new ArgumentException("Quantity cannot be negative", nameof(quantity));

        return quantity * UnitPrice;
    }
}
