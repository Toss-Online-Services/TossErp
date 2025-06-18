using POS.Domain.SeedWork;
using POS.Domain.AggregatesModel.ProductAggregate;

namespace POS.Domain.AggregatesModel.ProductAggregate.Events;

/// <summary>
/// Event raised when a new product is created
/// </summary>
public class ProductCreatedDomainEvent : DomainEvent
{
    public Guid ProductId { get; }
    public string Name { get; }
    public string Sku { get; }
    public decimal Price { get; }

    public ProductCreatedDomainEvent(Product product)
    {
        ProductId = product.Id;
        Name = product.Name;
        Sku = product.Sku;
        Price = product.Price;
    }
} 
