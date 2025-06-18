using POS.Domain.Events;
using POS.Domain.AggregatesModel.OrderAggregate;
using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.OrderAggregate.Events;

/// <summary>
/// Event raised when an order is cancelled
/// </summary>
public class OrderCancelledDomainEvent : DomainEvent
{
    public Guid OrderId { get; }
    public string OrderNumber { get; }
    public string? CancellationReason { get; }

    public OrderCancelledDomainEvent(Order order, string? reason)
    {
        OrderId = order.Id;
        OrderNumber = order.OrderNumber;
        CancellationReason = reason;
    }
} 
