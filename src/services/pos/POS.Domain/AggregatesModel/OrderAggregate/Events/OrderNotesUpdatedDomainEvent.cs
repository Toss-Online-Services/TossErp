using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.OrderAggregate.Events;

public class OrderNotesUpdatedDomainEvent : DomainEvent
{
    public Guid OrderId { get; }
    public string OrderNumber { get; }
    public string? Notes { get; }
    public DateTime UpdatedAt { get; }

    public OrderNotesUpdatedDomainEvent(Order order)
    {
        OrderId = order.Id;
        OrderNumber = order.OrderNumber;
        Notes = order.Notes;
        UpdatedAt = DateTime.UtcNow;
    }
} 
