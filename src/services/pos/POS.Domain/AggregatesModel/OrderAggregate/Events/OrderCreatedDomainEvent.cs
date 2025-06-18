using POS.Domain.SeedWork;
using POS.Domain.AggregatesModel.OrderAggregate;

namespace POS.Domain.AggregatesModel.OrderAggregate.Events;

/// <summary>
/// Event raised when a new order is created
/// </summary>
public class OrderCreatedDomainEvent : DomainEvent
{
    public Guid OrderId { get; }
    public string OrderNumber { get; }
    public Guid CustomerId { get; }
    public DateTime CreatedAt { get; }

    public OrderCreatedDomainEvent(Order order)
    {
        OrderId = order.Id;
        OrderNumber = order.OrderNumber;
        CustomerId = order.CustomerId;
        CreatedAt = order.CreatedAt;
    }
} 
