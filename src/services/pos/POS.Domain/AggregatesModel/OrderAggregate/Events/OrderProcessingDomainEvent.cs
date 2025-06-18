using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.OrderAggregate.Events;

public class OrderProcessingDomainEvent : DomainEvent
{
    public Guid OrderId { get; }
    public string OrderNumber { get; }
    public DateTime ProcessingStartedAt { get; }

    public OrderProcessingDomainEvent(Order order)
    {
        OrderId = order.Id;
        OrderNumber = order.OrderNumber;
        ProcessingStartedAt = DateTime.UtcNow;
    }
} 
