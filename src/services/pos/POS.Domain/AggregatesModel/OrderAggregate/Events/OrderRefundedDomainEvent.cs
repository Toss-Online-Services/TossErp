using POS.Domain.Events;
using POS.Domain.AggregatesModel.OrderAggregate;

namespace POS.Domain.AggregatesModel.OrderAggregate.Events;

/// <summary>
/// Event raised when an order is refunded
/// </summary>
public class OrderRefundedDomainEvent : DomainEvent
{
    public Guid OrderId { get; }
    public string OrderNumber { get; }
    public Money RefundAmount { get; }
    public string? RefundReason { get; }

    public OrderRefundedDomainEvent(Order order)
    {
        OrderId = order.Id;
        OrderNumber = order.OrderNumber;
        RefundAmount = order.TotalAmount;
        RefundReason = order.Notes;
    }
} 
