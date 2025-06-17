using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.ProductAggregate.Events;

public class ProductUpdatedDomainEvent : IDomainEvent
{
    public Guid ProductId { get; }
    public string Name { get; }
    public string Description { get; }
    public string SKU { get; }
    public string Barcode { get; }
    public DateTime UpdatedAt { get; }

    public ProductUpdatedDomainEvent(Guid productId, string name, string description, string sku, string barcode)
    {
        ProductId = productId;
        Name = name;
        Description = description;
        SKU = sku;
        Barcode = barcode;
        UpdatedAt = DateTime.UtcNow;
    }
} 
