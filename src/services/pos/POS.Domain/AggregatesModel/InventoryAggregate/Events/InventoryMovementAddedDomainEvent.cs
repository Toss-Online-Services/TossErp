using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.InventoryAggregate.Events;

public class InventoryMovementAddedDomainEvent : IDomainEvent
{
    public Guid InventoryId { get; }
    public Guid MovementId { get; }
    public DateTime AddedAt { get; }

    public InventoryMovementAddedDomainEvent(Guid inventoryId, Guid movementId, DateTime addedAt)
    {
        InventoryId = inventoryId;
        MovementId = movementId;
        AddedAt = addedAt;
    }
} 
