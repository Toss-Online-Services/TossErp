using Catalog.Domain.SeedWork;
using MediatR;

namespace Catalog.Domain.Events;

public class ProductStockChangedEvent : INotification
{
    public int ProductId { get; }
    public int OldStock { get; }
    public int NewStock { get; }

    public ProductStockChangedEvent(int productId, int oldStock, int newStock)
    {
        ProductId = productId;
        OldStock = oldStock;
        NewStock = newStock;
    }
} 
