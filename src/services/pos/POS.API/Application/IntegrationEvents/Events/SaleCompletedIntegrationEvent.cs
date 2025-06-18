using eShop.EventBus.Events;

namespace POS.API.Application.IntegrationEvents.Events;

public record SaleCompletedIntegrationEvent : IntegrationEvent
{
    public Guid SaleId { get; init; }
    public Guid OrderId { get; init; }
    public Guid CustomerId { get; init; }
    public Guid StoreId { get; init; }
    public decimal TotalAmount { get; init; }
    public decimal TaxAmount { get; init; }
    public decimal DiscountAmount { get; init; }
    public string PaymentMethod { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime CompletedAt { get; init; }
    public List<SaleItemDto> Items { get; init; } = new();

    public SaleCompletedIntegrationEvent(
        Guid saleId,
        Guid orderId,
        Guid customerId,
        Guid storeId,
        decimal totalAmount,
        decimal taxAmount,
        decimal discountAmount,
        string paymentMethod,
        string status,
        DateTime completedAt,
        List<SaleItemDto> items)
    {
        SaleId = saleId;
        OrderId = orderId;
        CustomerId = customerId;
        StoreId = storeId;
        TotalAmount = totalAmount;
        TaxAmount = taxAmount;
        DiscountAmount = discountAmount;
        PaymentMethod = paymentMethod;
        Status = status;
        CompletedAt = completedAt;
        Items = items;
    }
}

public record SaleItemDto
{
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal TotalPrice { get; init; }
} 
