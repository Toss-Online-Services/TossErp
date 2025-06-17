using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.InventoryAggregate.Events;

public class InventorySerialNumberUpdatedDomainEvent : IDomainEvent
{
    public Guid InventoryId { get; }
    public string OldSerialNumber { get; }
    public string NewSerialNumber { get; }
    public string UpdatedBy { get; }
    public DateTime UpdatedAt { get; }

    public InventorySerialNumberUpdatedDomainEvent(
        Guid inventoryId,
        string oldSerialNumber,
        string newSerialNumber,
        string updatedBy,
        DateTime updatedAt)
    {
        InventoryId = inventoryId;
        OldSerialNumber = oldSerialNumber;
        NewSerialNumber = newSerialNumber;
        UpdatedBy = updatedBy;
        UpdatedAt = updatedAt;
    }
} 
