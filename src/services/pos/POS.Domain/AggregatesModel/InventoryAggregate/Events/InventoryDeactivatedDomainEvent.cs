using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.InventoryAggregate.Events;

public class InventoryDeactivatedDomainEvent : IDomainEvent
{
    public Guid InventoryId { get; }
    public string DeactivatedBy { get; }
    public DateTime DeactivatedAt { get; }

    public InventoryDeactivatedDomainEvent(
        Guid inventoryId,
        string deactivatedBy,
        DateTime deactivatedAt)
    {
        InventoryId = inventoryId;
        DeactivatedBy = deactivatedBy;
        DeactivatedAt = deactivatedAt;
    }
} 
