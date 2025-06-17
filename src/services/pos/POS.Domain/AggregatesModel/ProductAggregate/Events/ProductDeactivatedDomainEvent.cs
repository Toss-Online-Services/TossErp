using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.ProductAggregate.Events;

public class ProductDeactivatedDomainEvent : IDomainEvent
{
    public Guid ProductId { get; }
    public DateTime DeactivatedAt { get; }

    public ProductDeactivatedDomainEvent(Guid productId, DateTime deactivatedAt)
    {
        ProductId = productId;
        DeactivatedAt = deactivatedAt;
    }
} 
