using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.ProductAggregate.Events;

public class ProductCreatedDomainEvent : IDomainEvent
{
    public Guid ProductId { get; }
    public string Name { get; }
    public string SKU { get; }
    public DateTime CreatedAt { get; }

    public ProductCreatedDomainEvent(Guid productId, string name, string sku)
    {
        ProductId = productId;
        Name = name;
        SKU = sku;
        CreatedAt = DateTime.UtcNow;
    }
} 
