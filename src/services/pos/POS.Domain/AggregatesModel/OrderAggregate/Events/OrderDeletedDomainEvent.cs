using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.OrderAggregate.Events;

/// <summary>
/// Event raised when an order is deleted
/// </summary>
public class OrderDeletedDomainEvent : DomainEvent
{
    public Guid OrderId { get; }
    public string OrderNumber { get; }
    public DateTime DeletedAt { get; }
    public string? DeletedBy { get; }

    public OrderDeletedDomainEvent(Order order)
    {
        OrderId = order.Id;
        OrderNumber = order.OrderNumber;
        DeletedAt = order.DeletedAt!.Value;
        DeletedBy = order.DeletedBy;
    }
} 
