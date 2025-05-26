using MediatR;

namespace Catalog.Domain.Events;

public class ProductTagCreatedEvent : INotification
{
    public int ProductTagId { get; }
    public string Name { get; }

    public ProductTagCreatedEvent(int productTagId, string name)
    {
        ProductTagId = productTagId;
        Name = name;
    }
} 
