using POS.Domain.SeedWork;
using POS.Domain.AggregatesModel.ProductAggregate;

namespace POS.Domain.AggregatesModel.ProductAggregate.Events;

/// <summary>
/// Event raised when a product's stock is updated
/// </summary>
public class ProductStockUpdatedDomainEvent : DomainEvent
{
    public Guid ProductId { get; }
    public int NewStockQuantity { get; }

    public ProductStockUpdatedDomainEvent(Product product)
    {
        ProductId = product.Id;
        NewStockQuantity = product.StockQuantity;
    }
} 
