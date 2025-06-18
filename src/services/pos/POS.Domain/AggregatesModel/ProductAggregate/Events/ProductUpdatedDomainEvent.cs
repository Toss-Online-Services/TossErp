using POS.Domain.SeedWork;
using POS.Domain.AggregatesModel.ProductAggregate;

namespace POS.Domain.AggregatesModel.ProductAggregate.Events;

/// <summary>
/// Event raised when a product is updated
/// </summary>
public class ProductUpdatedDomainEvent : DomainEvent
{
    public Guid ProductId { get; }
    public string Name { get; }
    public string Description { get; }
    public Money Price { get; }

    public ProductUpdatedDomainEvent(Product product)
    {
        ProductId = product.Id;
        Name = product.Name;
        Description = product.Description;
        Price = product.Price;
    }
} 
