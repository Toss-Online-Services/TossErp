using TossErp.Stock.Domain.SeedWork;

namespace TossErp.Stock.Domain.Events;

public class ItemUpdatedEvent : DomainEvent
{
    public Guid ItemId { get; }
    public string[] Changes { get; }

    public ItemUpdatedEvent(Guid itemId, string[] changes)
    {
        ItemId = itemId;
        Changes = changes;
    }
}
