using POS.Domain.SeedWork;

namespace POS.Domain.Events;

public class InventoryAdjustedEvent : DomainEvent
{
    public Guid ProductId { get; }
    public int QuantityChange { get; }
    public string Reason { get; }
    public DateTime AdjustedAt { get; }

    public InventoryAdjustedEvent(Guid productId, int quantityChange, string reason)
    {
        ProductId = productId;
        QuantityChange = quantityChange;
        Reason = reason;
        AdjustedAt = DateTime.UtcNow;
    }
} 
