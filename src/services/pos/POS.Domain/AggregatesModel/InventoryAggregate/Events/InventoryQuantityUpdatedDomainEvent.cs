using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.InventoryAggregate.Events;

public class InventoryQuantityUpdatedDomainEvent : IDomainEvent
{
    public Guid InventoryId { get; }
    public int OldQuantity { get; }
    public int NewQuantity { get; }
    public string Reason { get; }
    public DateTime UpdatedAt { get; }

    public InventoryQuantityUpdatedDomainEvent(
        Guid inventoryId,
        int oldQuantity,
        int newQuantity,
        string reason,
        DateTime updatedAt)
    {
        InventoryId = inventoryId;
        OldQuantity = oldQuantity;
        NewQuantity = newQuantity;
        Reason = reason;
        UpdatedAt = updatedAt;
    }
} 
