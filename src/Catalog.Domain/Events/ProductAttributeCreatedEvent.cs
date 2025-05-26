using Catalog.Domain.SeedWork;
using MediatR;

namespace Catalog.Domain.Events;

public class ProductAttributeCreatedEvent : INotification
{
    public int AttributeId { get; }
    public string Name { get; }
    public string Description { get; }

    public ProductAttributeCreatedEvent(int attributeId, string name, string description)
    {
        AttributeId = attributeId;
        Name = name;
        Description = description;
    }
} 
