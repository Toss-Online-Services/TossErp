using Catalog.Domain.SeedWork;
using MediatR;

namespace Catalog.Domain.Events;

public class ProductAttributeUpdatedEvent : INotification
{
    public int AttributeId { get; }
    public string OldName { get; }
    public string NewName { get; }
    public string OldDescription { get; }
    public string NewDescription { get; }

    public ProductAttributeUpdatedEvent(int attributeId, string oldName, string newName, string oldDescription, string newDescription)
    {
        AttributeId = attributeId;
        OldName = oldName;
        NewName = newName;
        OldDescription = oldDescription;
        NewDescription = newDescription;
    }
} 
