using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.OrderAggregate.Events;

public class OrderConfirmedDomainEvent : DomainEvent
{
    public Guid OrderId { get; }
    public string OrderNumber { get; }
    public DateTime ConfirmedAt { get; }

    public OrderConfirmedDomainEvent(Order order)
    {
        OrderId = order.Id;
        OrderNumber = order.OrderNumber;
        ConfirmedAt = DateTime.UtcNow;
    }
} 
