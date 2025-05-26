using Catalog.Domain.SeedWork;
using MediatR;

namespace Catalog.Domain.Events;

public class ProductPriceChangedEvent : INotification
{
    public int ProductId { get; }
    public decimal OldPrice { get; }
    public decimal NewPrice { get; }

    public ProductPriceChangedEvent(int productId, decimal oldPrice, decimal newPrice)
    {
        ProductId = productId;
        OldPrice = oldPrice;
        NewPrice = newPrice;
    }
} 
