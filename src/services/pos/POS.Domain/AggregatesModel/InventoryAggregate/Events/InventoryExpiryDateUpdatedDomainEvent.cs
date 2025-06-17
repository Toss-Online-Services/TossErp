using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.InventoryAggregate.Events;

public class InventoryExpiryDateUpdatedDomainEvent : IDomainEvent
{
    public Guid InventoryId { get; }
    public DateTime? OldExpiryDate { get; }
    public DateTime? NewExpiryDate { get; }
    public string UpdatedBy { get; }
    public DateTime UpdatedAt { get; }

    public InventoryExpiryDateUpdatedDomainEvent(
        Guid inventoryId,
        DateTime? oldExpiryDate,
        DateTime? newExpiryDate,
        string updatedBy,
        DateTime updatedAt)
    {
        InventoryId = inventoryId;
        OldExpiryDate = oldExpiryDate;
        NewExpiryDate = newExpiryDate;
        UpdatedBy = updatedBy;
        UpdatedAt = updatedAt;
    }
} 
