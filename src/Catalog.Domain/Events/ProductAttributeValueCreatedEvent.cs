using MediatR;

namespace Catalog.Domain.Events;

public class ProductAttributeValueCreatedEvent : INotification
{
    public int ProductAttributeValueId { get; }
    public int ProductAttributeMappingId { get; }
    public int ProductId { get; }
    public string Name { get; }
    public decimal PriceAdjustment { get; }

    public ProductAttributeValueCreatedEvent(int productAttributeValueId, int productAttributeMappingId, 
        int productId, string name, decimal priceAdjustment)
    {
        ProductAttributeValueId = productAttributeValueId;
        ProductAttributeMappingId = productAttributeMappingId;
        ProductId = productId;
        Name = name;
        PriceAdjustment = priceAdjustment;
    }
} 
