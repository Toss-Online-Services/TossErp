using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.InventoryAggregate.Events;

public class InventoryReservationReleasedDomainEvent : IDomainEvent
{
    public Guid InventoryId { get; }
    public int ReleasedQuantity { get; }
    public string ReleasedBy { get; }
    public DateTime ReleasedAt { get; }

    public InventoryReservationReleasedDomainEvent(
        Guid inventoryId,
        int releasedQuantity,
        string releasedBy,
        DateTime releasedAt)
    {
        InventoryId = inventoryId;
        ReleasedQuantity = releasedQuantity;
        ReleasedBy = releasedBy;
        ReleasedAt = releasedAt;
    }
} 
