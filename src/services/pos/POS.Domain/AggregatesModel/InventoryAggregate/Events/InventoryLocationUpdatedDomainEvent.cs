using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.InventoryAggregate.Events;

public class InventoryLocationUpdatedDomainEvent : IDomainEvent
{
    public Guid InventoryId { get; }
    public string OldLocation { get; }
    public string NewLocation { get; }
    public string UpdatedBy { get; }
    public DateTime UpdatedAt { get; }

    public InventoryLocationUpdatedDomainEvent(
        Guid inventoryId,
        string oldLocation,
        string newLocation,
        string updatedBy,
        DateTime updatedAt)
    {
        InventoryId = inventoryId;
        OldLocation = oldLocation;
        NewLocation = newLocation;
        UpdatedBy = updatedBy;
        UpdatedAt = updatedAt;
    }
} 
