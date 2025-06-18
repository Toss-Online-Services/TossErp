using POS.Domain.Events;
using POS.Domain.AggregatesModel.OrderAggregate;
using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.OrderAggregate.Events;

/// <summary>
/// Event raised when an order is refunded
/// </summary>
public class OrderRefundedDomainEvent : DomainEvent
{
    public Guid OrderId { get; }
    public string OrderNumber { get; }
    public Money TotalAmount { get; }
    public string? RefundReason { get; }

    public OrderRefundedDomainEvent(Order order, string? reason)
    {
        OrderId = order.Id;
        OrderNumber = order.OrderNumber;
        TotalAmount = order.TotalAmount;
        RefundReason = reason;
    }
} 
