using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.InventoryAggregate.Events;

public class InventoryReactivatedDomainEvent : IDomainEvent
{
    public Guid InventoryId { get; }
    public string ReactivatedBy { get; }
    public DateTime ReactivatedAt { get; }

    public InventoryReactivatedDomainEvent(
        Guid inventoryId,
        string reactivatedBy,
        DateTime reactivatedAt)
    {
        InventoryId = inventoryId;
        ReactivatedBy = reactivatedBy;
        ReactivatedAt = reactivatedAt;
    }
} 
