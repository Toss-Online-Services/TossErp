using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Aggregates.ItemAggregate.Events;
using TossErp.Stock.Domain.Events;

namespace TossErp.Stock.Domain.Aggregates.ItemAggregate.Entities;

public class ItemReorder : BaseAuditableEntity
{
    public Guid ItemId { get; private set; }
    public decimal ReorderLevel { get; private set; }
    public decimal ReorderQuantity { get; private set; }
    public decimal? MaximumStock { get; private set; }
    public int? LeadTimeDays { get; private set; }
    public bool IsActive { get; private set; } = true;

    private ItemReorder() { } // For EF Core

    public ItemReorder(Guid itemId, decimal reorderLevel, decimal reorderQuantity, 
        decimal? maximumStock = null, int? leadTimeDays = null)
    {
        if (itemId == Guid.Empty)
            throw new ArgumentException("Item ID cannot be empty.", nameof(itemId));

        if (reorderLevel < 0)
            throw new ArgumentException("Reorder level cannot be negative.", nameof(reorderLevel));

        if (reorderQuantity <= 0)
            throw new ArgumentException("Reorder quantity must be positive.", nameof(reorderQuantity));

        ItemId = itemId;
        ReorderLevel = reorderLevel;
        ReorderQuantity = reorderQuantity;
        MaximumStock = maximumStock;
        LeadTimeDays = leadTimeDays;

        AddDomainEvent(new ItemReorderCreatedEvent(this));
    }

    public void UpdateReorderSettings(decimal reorderLevel, decimal reorderQuantity, 
        decimal? maximumStock = null, int? leadTimeDays = null)
    {
        if (reorderLevel < 0)
            throw new ArgumentException("Reorder level cannot be negative.", nameof(reorderLevel));

        if (reorderQuantity <= 0)
            throw new ArgumentException("Reorder quantity must be positive.", nameof(reorderQuantity));

        ReorderLevel = reorderLevel;
        ReorderQuantity = reorderQuantity;
        MaximumStock = maximumStock;
        LeadTimeDays = leadTimeDays;

        AddDomainEvent(new ItemReorderUpdatedEvent(this));
    }

    public void Deactivate()
    {
        if (!IsActive) return;

        IsActive = false;
        AddDomainEvent(new ItemReorderDeactivatedEvent(this));
    }

    public void Activate()
    {
        if (IsActive) return;

        IsActive = true;
        AddDomainEvent(new ItemReorderActivatedEvent(this));
    }

    public void Delete()
    {
        AddDomainEvent(new ItemReorderDeletedEvent(this));
    }
} 
