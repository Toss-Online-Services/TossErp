using eShop.EventBus.Events;

namespace POS.API.Application.IntegrationEvents.Events;

public record OrderCreatedIntegrationEvent : IntegrationEvent
{
    public Guid OrderId { get; init; }
    public Guid CustomerId { get; init; }
    public Guid StoreId { get; init; }
    public decimal TotalAmount { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public List<OrderItemDto> Items { get; init; } = new();

    public OrderCreatedIntegrationEvent(
        Guid orderId,
        Guid customerId,
        Guid storeId,
        decimal totalAmount,
        string status,
        DateTime createdAt,
        List<OrderItemDto> items)
    {
        OrderId = orderId;
        CustomerId = customerId;
        StoreId = storeId;
        TotalAmount = totalAmount;
        Status = status;
        CreatedAt = createdAt;
        Items = items;
    }
}

public record OrderItemDto
{
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal TotalPrice { get; init; }
} 
