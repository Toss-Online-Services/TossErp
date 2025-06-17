using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.InventoryAggregate.Events;

public class InventoryLotNumberUpdatedDomainEvent : IDomainEvent
{
    public Guid InventoryId { get; }
    public string OldLotNumber { get; }
    public string NewLotNumber { get; }
    public DateTime UpdatedAt { get; }

    public InventoryLotNumberUpdatedDomainEvent(
        Guid inventoryId,
        string oldLotNumber,
        string newLotNumber,
        DateTime updatedAt)
    {
        InventoryId = inventoryId;
        OldLotNumber = oldLotNumber;
        NewLotNumber = newLotNumber;
        UpdatedAt = updatedAt;
    }
} 
