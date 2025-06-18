using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.OrderAggregate.Events;

/// <summary>
/// Event raised when an order's tax rate is updated
/// </summary>
public class OrderTaxRateUpdatedDomainEvent : DomainEvent
{
    public Guid OrderId { get; }
    public string OrderNumber { get; }
    public decimal NewTaxRate { get; }
    public DateTime UpdatedAt { get; }

    public OrderTaxRateUpdatedDomainEvent(Order order, decimal newTaxRate)
    {
        OrderId = order.Id;
        OrderNumber = order.OrderNumber;
        NewTaxRate = newTaxRate;
        UpdatedAt = DateTime.UtcNow;
    }
} 
