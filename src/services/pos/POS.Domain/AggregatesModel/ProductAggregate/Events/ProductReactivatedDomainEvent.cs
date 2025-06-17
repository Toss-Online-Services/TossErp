using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.ProductAggregate.Events;

public class ProductReactivatedDomainEvent : IDomainEvent
{
    public Guid ProductId { get; }
    public DateTime ReactivatedAt { get; }

    public ProductReactivatedDomainEvent(Guid productId, DateTime reactivatedAt)
    {
        ProductId = productId;
        ReactivatedAt = reactivatedAt;
    }
} 
