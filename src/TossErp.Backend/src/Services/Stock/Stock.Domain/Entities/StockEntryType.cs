using TossErp.Stock.Domain.Events;
using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Enums;
using TossErp.Stock.Domain.Aggregates.StockEntryAggregate;

namespace TossErp.Stock.Domain.Entities;

public class StockEntryType : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public StockEntryPurpose Purpose { get; private set; }
    public bool AddToTransit { get; private set; }
    public bool AllowNegativeStock { get; private set; }
    public bool IsDisabled { get; private set; }

    // Navigation properties
    public virtual ICollection<StockEntryAggregate> StockEntries { get; private set; } = new List<StockEntryAggregate>();

    private StockEntryType() { } // For EF Core

    public StockEntryType(string name, StockEntryPurpose purpose)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Purpose = purpose;
        AddToTransit = false;
        AllowNegativeStock = false;
        IsDisabled = false;

        AddDomainEvent(new StockEntryTypeCreatedEvent(this));
    }

    public void UpdateInfo(string? description)
    {
        Description = description;
        AddDomainEvent(new StockEntryTypeUpdatedEvent(this));
    }

    public void UpdatePurpose(StockEntryPurpose purpose)
    {
        Purpose = purpose;
        AddDomainEvent(new StockEntryTypePurposeUpdatedEvent(this));
    }

    public void SetAddToTransit(bool addToTransit)
    {
        AddToTransit = addToTransit;
        AddDomainEvent(new StockEntryTypeTransitUpdatedEvent(this));
    }

    public void SetAllowNegativeStock(bool allowNegativeStock)
    {
        AllowNegativeStock = allowNegativeStock;
        AddDomainEvent(new StockEntryTypeNegativeStockUpdatedEvent(this));
    }

    public void Disable()
    {
        IsDisabled = true;
        AddDomainEvent(new StockEntryTypeDisabledEvent(this));
    }

    public void Enable()
    {
        IsDisabled = false;
        AddDomainEvent(new StockEntryTypeEnabledEvent(this));
    }

    public void AddStockEntry(StockEntryAggregate stockEntry)
    {
        if (stockEntry == null)
            throw new ArgumentNullException(nameof(stockEntry));

        StockEntries.Add(stockEntry);
        AddDomainEvent(new StockEntryTypeStockEntryAddedEvent(this, stockEntry));
    }

    public void RemoveStockEntry(StockEntryAggregate stockEntry)
    {
        if (stockEntry == null)
            throw new ArgumentNullException(nameof(stockEntry));

        if (StockEntries.Remove(stockEntry))
        {
            AddDomainEvent(new StockEntryTypeStockEntryRemovedEvent(this, stockEntry));
        }
    }

    public bool IsDisabledType()
    {
        return IsDisabled;
    }

    public bool AllowsTransit()
    {
        return AddToTransit;
    }

    public bool AllowsNegativeStock()
    {
        return AllowNegativeStock;
    }
} 
