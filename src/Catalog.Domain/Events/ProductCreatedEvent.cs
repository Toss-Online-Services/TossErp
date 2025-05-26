using Catalog.Domain.SeedWork;
using MediatR;

namespace Catalog.Domain.Events;

public class ProductCreatedEvent : INotification
{
    public int ProductId { get; }
    public string Name { get; }
    public decimal Price { get; }

    public ProductCreatedEvent(int productId, string name, decimal price)
    {
        ProductId = productId;
        Name = name;
        Price = price;
    }
} 
