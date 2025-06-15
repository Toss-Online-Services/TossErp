namespace POS.Domain.Events;

/// <summary>
/// Event used when the order stock items are confirmed
/// </summary>
public class OrderStatusChangedToStockConfirmedDomainEvent
{
    public int OrderId { get; }

    public OrderStatusChangedToStockConfirmedDomainEvent(int orderId)
        => OrderId = orderId;
}
