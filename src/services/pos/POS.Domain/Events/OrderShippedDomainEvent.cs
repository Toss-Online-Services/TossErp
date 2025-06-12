namespace eShop.POS.Domain.Events;

public class OrderShippedDomainEvent : INotification
{
    public Order Order { get; }

    public OrderShippedDomainEvent(Order order)
    {
        Order = order;
    }
}
