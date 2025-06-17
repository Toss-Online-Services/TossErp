using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.InventoryAggregate.Events;

public class InventoryReservedDomainEvent : IDomainEvent
{
    public Guid InventoryId { get; }
    public int ReservedQuantity { get; }
    public string ReservedBy { get; }
    public DateTime ReservedAt { get; }

    public InventoryReservedDomainEvent(
        Guid inventoryId,
        int reservedQuantity,
        string reservedBy,
        DateTime reservedAt)
    {
        InventoryId = inventoryId;
        ReservedQuantity = reservedQuantity;
        ReservedBy = reservedBy;
        ReservedAt = reservedAt;
    }
} 
