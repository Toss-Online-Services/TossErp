using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.InventoryAggregate.Events;

public class InventoryUnitCostUpdatedDomainEvent : IDomainEvent
{
    public Guid InventoryId { get; }
    public decimal OldUnitCost { get; }
    public decimal NewUnitCost { get; }
    public string Currency { get; }
    public string UpdatedBy { get; }
    public DateTime UpdatedAt { get; }

    public InventoryUnitCostUpdatedDomainEvent(
        Guid inventoryId,
        decimal oldUnitCost,
        decimal newUnitCost,
        string currency,
        string updatedBy,
        DateTime updatedAt)
    {
        InventoryId = inventoryId;
        OldUnitCost = oldUnitCost;
        NewUnitCost = newUnitCost;
        Currency = currency;
        UpdatedBy = updatedBy;
        UpdatedAt = updatedAt;
    }
} 
