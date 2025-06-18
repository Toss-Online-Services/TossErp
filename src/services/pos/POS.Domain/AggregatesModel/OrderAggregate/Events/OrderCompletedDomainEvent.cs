using POS.Domain.SeedWork;
using POS.Domain.AggregatesModel.OrderAggregate;

namespace POS.Domain.AggregatesModel.OrderAggregate.Events;

/// <summary>
/// Event raised when an order is completed
/// </summary>
public class OrderCompletedDomainEvent : DomainEvent
{
    public Guid OrderId { get; }
    public string OrderNumber { get; }
    public Money TotalAmount { get; }
    public DateTime CompletedAt { get; }

    public OrderCompletedDomainEvent(Order order)
    {
        OrderId = order.Id;
        OrderNumber = order.OrderNumber;
        TotalAmount = order.TotalAmount;
        CompletedAt = order.CompletedAt!.Value;
    }
} 
