using TossErp.Accounting.Domain.Common;
using TossErp.Accounting.Domain.Enums;
using TossErp.Accounting.Domain.Events;

namespace TossErp.Accounting.Domain.Entities;

/// <summary>
/// Stock valuation snapshot for P&L reporting
/// </summary>
public class StockValuationSnapshot : Entity<Guid>
{
    public DateTime SnapshotDate { get; private set; }
    public string WarehouseCode { get; private set; } = string.Empty;
    public string ItemCode { get; private set; } = string.Empty;
    public string ItemName { get; private set; } = string.Empty;
    public decimal Quantity { get; private set; }
    public Money UnitCost { get; private set; } = Money.Zero();
    public Money TotalValue { get; private set; } = Money.Zero();
    public ValuationMethod Method { get; private set; }
    public string? Notes { get; private set; }

    // Navigation properties
    public Guid AccountId { get; private set; }
    public virtual Account Account { get; private set; } = default!;

    // Domain events
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private StockValuationSnapshot() { } // Required for EF Core

    public StockValuationSnapshot(Guid id, DateTime snapshotDate, string warehouseCode, string itemCode, 
        string itemName, decimal quantity, Money unitCost, Money totalValue, ValuationMethod method, 
        Guid accountId, string tenantId, string? notes = null) : base(id, tenantId)
    {
        SnapshotDate = snapshotDate;
        WarehouseCode = warehouseCode ?? throw new ArgumentNullException(nameof(warehouseCode));
        ItemCode = itemCode ?? throw new ArgumentNullException(nameof(itemCode));
        ItemName = itemName ?? throw new ArgumentNullException(nameof(itemName));
        Quantity = quantity;
        UnitCost = unitCost ?? throw new ArgumentNullException(nameof(unitCost));
        TotalValue = totalValue ?? throw new ArgumentNullException(nameof(totalValue));
        Method = method;
        AccountId = accountId;
        Notes = notes;
        
        AddDomainEvent(new StockValuationSnapshotCreatedEvent(Id, SnapshotDate, WarehouseCode, ItemCode, TotalValue, TenantId));
    }

    /// <summary>
    /// Create a new stock valuation snapshot
    /// </summary>
    public static StockValuationSnapshot Create(DateTime snapshotDate, string warehouseCode, string itemCode, 
        string itemName, decimal quantity, Money unitCost, ValuationMethod method, Guid accountId, 
        string tenantId, string? notes = null)
    {
        if (quantity < 0) throw new ArgumentException("Quantity cannot be negative.", nameof(quantity));
        if (unitCost.IsNegative()) throw new ArgumentException("Unit cost cannot be negative.", nameof(unitCost));
        
        var totalValue = unitCost * quantity;
        var snapshot = new StockValuationSnapshot(Guid.NewGuid(), snapshotDate, warehouseCode, itemCode, 
            itemName, quantity, unitCost, totalValue, method, accountId, tenantId, notes);
        
        return snapshot;
    }

    /// <summary>
    /// Update snapshot details
    /// </summary>
    public void UpdateDetails(decimal quantity, Money unitCost, string? notes, string updatedBy)
    {
        if (quantity < 0) throw new ArgumentException("Quantity cannot be negative.", nameof(quantity));
        if (unitCost.IsNegative()) throw new ArgumentException("Unit cost cannot be negative.", nameof(unitCost));
        
        Quantity = quantity;
        UnitCost = unitCost;
        TotalValue = unitCost * quantity;
        Notes = notes;
        MarkAsUpdated(updatedBy);
        
        AddDomainEvent(new StockValuationSnapshotUpdatedEvent(Id, SnapshotDate, WarehouseCode, 
            ItemCode, TotalValue));
    }

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
