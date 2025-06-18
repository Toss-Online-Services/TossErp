using POS.Domain.Events;
using POS.Domain.AggregatesModel.ProductAggregate;

namespace POS.Domain.AggregatesModel.ProductAggregate.Events;

/// <summary>
/// Event raised when a product is activated
/// </summary>
public class ProductActivatedDomainEvent : DomainEvent
{
    public Guid ProductId { get; }

    public ProductActivatedDomainEvent(Product product)
    {
        ProductId = product.Id;
    }
} 
