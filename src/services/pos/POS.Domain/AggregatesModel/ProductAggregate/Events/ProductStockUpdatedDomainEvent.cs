using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.ProductAggregate.Events;

public class ProductStockUpdatedDomainEvent : IDomainEvent
{
    public Guid ProductId { get; }
    public int Quantity { get; }
    public DateTime UpdatedAt { get; }

    public ProductStockUpdatedDomainEvent(Guid productId, int quantity, DateTime updatedAt)
    {
        ProductId = productId;
        Quantity = quantity;
        UpdatedAt = updatedAt;
    }
} 
