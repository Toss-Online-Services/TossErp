using POS.Domain.Events;
using POS.Domain.AggregatesModel.OrderAggregate;

namespace POS.Domain.AggregatesModel.OrderAggregate.Events;

/// <summary>
/// Event raised when an item is added to an order
/// </summary>
public class OrderItemAddedDomainEvent : DomainEvent
{
    public Guid OrderId { get; }
    public Guid ProductId { get; }
    public int Quantity { get; }
    public Money UnitPrice { get; }
    public Money TotalPrice { get; }

    public OrderItemAddedDomainEvent(Order order, OrderItem item)
    {
        OrderId = order.Id;
        ProductId = item.ProductId;
        Quantity = item.Quantity;
        UnitPrice = item.UnitPrice;
        TotalPrice = item.TotalPrice;
    }
} 
