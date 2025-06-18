using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.OrderAggregate.Events;

public class OrderItemQuantityUpdatedDomainEvent : DomainEvent
{
    public Guid OrderId { get; }
    public Guid ProductId { get; }
    public int NewQuantity { get; }

    public OrderItemQuantityUpdatedDomainEvent(Order order, Guid productId, int newQuantity)
    {
        OrderId = order.Id;
        ProductId = productId;
        NewQuantity = newQuantity;
    }
} 
