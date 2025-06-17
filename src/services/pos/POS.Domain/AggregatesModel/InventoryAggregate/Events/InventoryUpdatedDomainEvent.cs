using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.InventoryAggregate.Events;

public class InventoryUpdatedDomainEvent : IDomainEvent
{
    public Guid InventoryId { get; }
    public DateTime UpdatedAt { get; }

    public InventoryUpdatedDomainEvent(Guid inventoryId, DateTime updatedAt)
    {
        InventoryId = inventoryId;
        UpdatedAt = updatedAt;
    }
} 
