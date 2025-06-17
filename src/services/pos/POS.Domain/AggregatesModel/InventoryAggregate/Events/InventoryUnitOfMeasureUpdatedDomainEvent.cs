using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.InventoryAggregate.Events;

public class InventoryUnitOfMeasureUpdatedDomainEvent : IDomainEvent
{
    public Guid InventoryId { get; }
    public string OldUnitOfMeasure { get; }
    public string NewUnitOfMeasure { get; }
    public string UpdatedBy { get; }
    public DateTime UpdatedAt { get; }

    public InventoryUnitOfMeasureUpdatedDomainEvent(
        Guid inventoryId,
        string oldUnitOfMeasure,
        string newUnitOfMeasure,
        string updatedBy,
        DateTime updatedAt)
    {
        InventoryId = inventoryId;
        OldUnitOfMeasure = oldUnitOfMeasure;
        NewUnitOfMeasure = newUnitOfMeasure;
        UpdatedBy = updatedBy;
        UpdatedAt = updatedAt;
    }
} 
