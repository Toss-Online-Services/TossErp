using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.OrderAggregate.Events;

/// <summary>
/// Event raised when an order's discount percentage is updated
/// </summary>
public class OrderDiscountPercentageUpdatedDomainEvent : DomainEvent
{
    public Guid OrderId { get; }
    public string OrderNumber { get; }
    public decimal NewDiscountPercentage { get; }
    public DateTime UpdatedAt { get; }

    public OrderDiscountPercentageUpdatedDomainEvent(Order order, decimal newDiscountPercentage)
    {
        OrderId = order.Id;
        OrderNumber = order.OrderNumber;
        NewDiscountPercentage = newDiscountPercentage;
        UpdatedAt = DateTime.UtcNow;
    }
} 
