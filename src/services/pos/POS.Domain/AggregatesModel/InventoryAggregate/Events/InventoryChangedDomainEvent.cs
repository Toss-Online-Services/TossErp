using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.InventoryAggregate.Events;

public class InventoryChangedDomainEvent : IDomainEvent
{
    public Guid InventoryId { get; }
    public DateTime ChangedAt { get; }

    public InventoryChangedDomainEvent(Guid inventoryId, DateTime changedAt)
    {
        InventoryId = inventoryId;
        ChangedAt = changedAt;
    }
} 
