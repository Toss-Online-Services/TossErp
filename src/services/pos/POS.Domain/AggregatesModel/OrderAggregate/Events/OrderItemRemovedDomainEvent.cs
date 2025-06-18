using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.OrderAggregate.Events;

public class OrderItemRemovedDomainEvent : DomainEvent
{
    public Guid OrderId { get; }
    public Guid ProductId { get; }

    public OrderItemRemovedDomainEvent(Order order, Guid productId)
    {
        OrderId = order.Id;
        ProductId = productId;
    }
} 
