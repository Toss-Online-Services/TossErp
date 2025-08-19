using TossErp.Procurement.Domain.Common;
using TossErp.Procurement.Domain.Events;

namespace TossErp.Procurement.Domain.Entities;

/// <summary>
/// Supplier price list entity representing pricing information for items from a specific supplier
/// </summary>
public class SupplierPriceList : Entity<Guid>
{
    private readonly List<SupplierPriceListItem> _items = new();

    public Guid SupplierId { get; private set; }
    public string SupplierName { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public DateTime EffectiveFrom { get; private set; }
    public DateTime? EffectiveTo { get; private set; }
    public bool IsActive { get; private set; }
    public string Currency { get; private set; } = "ZAR";

    // Navigation properties
    public IReadOnlyList<SupplierPriceListItem> Items => _items.AsReadOnly();

    // Domain events
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected SupplierPriceList() : base() { } // For EF Core

    public SupplierPriceList(Guid id, Guid supplierId, string supplierName, string name, 
        string tenantId, DateTime effectiveFrom, string currency = "ZAR") : base(id, tenantId)
    {
        SupplierId = supplierId;
        SupplierName = supplierName ?? throw new ArgumentNullException(nameof(supplierName));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        EffectiveFrom = effectiveFrom;
        Currency = currency ?? throw new ArgumentNullException(nameof(currency));
        IsActive = true;

        AddDomainEvent(new SupplierPriceListCreatedEvent(Id, SupplierId, Name, tenantId));
    }

    /// <summary>
    /// Create a new supplier price list
    /// </summary>
    public static SupplierPriceList Create(Guid supplierId, string supplierName, string name, 
        string tenantId, DateTime? effectiveFrom = null, string currency = "ZAR")
    {
        if (string.IsNullOrWhiteSpace(name)) 
            throw new ArgumentException("Name cannot be empty", nameof(name));
        if (string.IsNullOrWhiteSpace(supplierName)) 
            throw new ArgumentException("Supplier name cannot be empty", nameof(supplierName));

        return new SupplierPriceList(Guid.NewGuid(), supplierId, supplierName, name, tenantId, 
            effectiveFrom ?? DateTime.UtcNow, currency);
    }

    /// <summary>
    /// Add or update an item price in the price list
    /// </summary>
    public void AddOrUpdateItem(Guid itemId, string itemName, string itemSku, decimal unitPrice, 
        decimal? minimumOrderQuantity = null, int? leadTimeDays = null, string? notes = null)
    {
        if (!IsActive)
            throw new InvalidOperationException("Cannot modify inactive price list");

        if (unitPrice < 0)
            throw new ArgumentException("Unit price cannot be negative", nameof(unitPrice));

        var existingItem = _items.FirstOrDefault(i => i.ItemId == itemId);
        if (existingItem != null)
        {
            // Update existing item
            existingItem.UpdatePrice(unitPrice, minimumOrderQuantity, leadTimeDays, notes);
            AddDomainEvent(new SupplierPriceListItemUpdatedEvent(Id, itemId, unitPrice));
        }
        else
        {
            // Add new item
            var priceListItem = SupplierPriceListItem.Create(Id, itemId, itemName, itemSku, 
                unitPrice, Currency, minimumOrderQuantity, leadTimeDays, notes);
            priceListItem.SetTenantId(TenantId);
            _items.Add(priceListItem);

            AddDomainEvent(new SupplierPriceListItemAddedEvent(Id, itemId, unitPrice));
        }
    }

    /// <summary>
    /// Remove an item from the price list
    /// </summary>
    public void RemoveItem(Guid itemId)
    {
        if (!IsActive)
            throw new InvalidOperationException("Cannot modify inactive price list");

        var item = _items.FirstOrDefault(i => i.ItemId == itemId);
        if (item != null)
        {
            _items.Remove(item);
            AddDomainEvent(new SupplierPriceListItemRemovedEvent(Id, itemId));
        }
    }

    /// <summary>
    /// Get price for a specific item
    /// </summary>
    public SupplierPriceListItem? GetItemPrice(Guid itemId)
    {
        return _items.FirstOrDefault(i => i.ItemId == itemId);
    }

    /// <summary>
    /// Update price list information
    /// </summary>
    public void UpdateInfo(string name, string? description, DateTime? effectiveTo, string updatedBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        Name = name;
        Description = description;
        EffectiveTo = effectiveTo;
        MarkAsUpdated(updatedBy);

        AddDomainEvent(new SupplierPriceListUpdatedEvent(Id, Name, Description));
    }

    /// <summary>
    /// Activate the price list
    /// </summary>
    public void Activate(string activatedBy)
    {
        if (IsActive)
            throw new InvalidOperationException("Price list is already active");

        IsActive = true;
        MarkAsUpdated(activatedBy);

        AddDomainEvent(new SupplierPriceListActivatedEvent(Id, activatedBy));
    }

    /// <summary>
    /// Deactivate the price list
    /// </summary>
    public void Deactivate(string deactivatedBy, string? reason = null)
    {
        if (!IsActive)
            throw new InvalidOperationException("Price list is already inactive");

        IsActive = false;
        MarkAsUpdated(deactivatedBy);

        AddDomainEvent(new SupplierPriceListDeactivatedEvent(Id, deactivatedBy, reason));
    }

    /// <summary>
    /// Check if price list is currently effective
    /// </summary>
    public bool IsCurrentlyEffective => IsActive && 
        DateTime.UtcNow >= EffectiveFrom && 
        (EffectiveTo == null || DateTime.UtcNow <= EffectiveTo);

    /// <summary>
    /// Add domain event
    /// </summary>
    private void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Clear domain events (typically called after publishing)
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
