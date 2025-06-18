using eShop.EventBus.Events;

namespace POS.API.Application.IntegrationEvents.Events;

public record ProductCreatedIntegrationEvent : IntegrationEvent
{
    public Guid ProductId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string SKU { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public Guid StoreId { get; init; }
    public Guid CategoryId { get; init; }
    public DateTime CreatedAt { get; init; }

    public ProductCreatedIntegrationEvent(
        Guid productId,
        string name,
        string sku,
        decimal price,
        Guid storeId,
        Guid categoryId,
        DateTime createdAt)
    {
        ProductId = productId;
        Name = name;
        SKU = sku;
        Price = price;
        StoreId = storeId;
        CategoryId = categoryId;
        CreatedAt = createdAt;
    }
} 
