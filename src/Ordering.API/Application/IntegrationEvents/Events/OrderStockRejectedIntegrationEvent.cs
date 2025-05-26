namespace Ordering.API.Application.IntegrationEvents.Events;

public record OrderStockRejectedIntegrationEvent : IntegrationEvent
{
    public int OrderId { get; }
    public IEnumerable<ConfirmedOrderStockItem> OrderStockItems { get; }

    public OrderStockRejectedIntegrationEvent(int orderId,
        IEnumerable<ConfirmedOrderStockItem> orderStockItems)
    {
        OrderId = orderId;
        OrderStockItems = orderStockItems;
    }
}

public record ConfirmedOrderStockItem
{
    public int ProductId { get; }
    public bool HasStock { get; }

    public ConfirmedOrderStockItem(int productId, bool hasStock)
    {
        ProductId = productId;
        HasStock = hasStock;
    }
}
