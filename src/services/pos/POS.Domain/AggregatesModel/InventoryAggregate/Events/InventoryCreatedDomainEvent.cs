using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.InventoryAggregate.Events;

public class InventoryCreatedDomainEvent : IDomainEvent
{
    public Guid InventoryId { get; }
    public Guid ProductId { get; }
    public Guid StoreId { get; }
    public DateTime CreatedAt { get; }

    public InventoryCreatedDomainEvent(Guid inventoryId, Guid productId, Guid storeId)
    {
        InventoryId = inventoryId;
        ProductId = productId;
        StoreId = storeId;
        CreatedAt = DateTime.UtcNow;
    }
} 
