using MediatR;

namespace Catalog.Domain.Events;

public class ProductTagUpdatedEvent : INotification
{
    public int ProductTagId { get; }
    public string OldName { get; }
    public string NewName { get; }

    public ProductTagUpdatedEvent(int productTagId, string oldName, string newName)
    {
        ProductTagId = productTagId;
        OldName = oldName;
        NewName = newName;
    }
} 
