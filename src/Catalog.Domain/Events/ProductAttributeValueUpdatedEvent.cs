using MediatR;

namespace Catalog.Domain.Events;

public class ProductAttributeValueUpdatedEvent : INotification
{
    public int ProductAttributeValueId { get; }
    public int ProductAttributeMappingId { get; }
    public int ProductId { get; }
    public string OldName { get; }
    public string NewName { get; }
    public decimal OldPriceAdjustment { get; }
    public decimal NewPriceAdjustment { get; }

    public ProductAttributeValueUpdatedEvent(int productAttributeValueId, int productAttributeMappingId, 
        int productId, string oldName, string newName, decimal oldPriceAdjustment, decimal newPriceAdjustment)
    {
        ProductAttributeValueId = productAttributeValueId;
        ProductAttributeMappingId = productAttributeMappingId;
        ProductId = productId;
        OldName = oldName;
        NewName = newName;
        OldPriceAdjustment = oldPriceAdjustment;
        NewPriceAdjustment = newPriceAdjustment;
    }
} 
