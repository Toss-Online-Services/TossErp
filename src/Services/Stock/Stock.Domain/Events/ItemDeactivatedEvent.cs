using TossErp.Stock.Domain.SeedWork;

namespace TossErp.Stock.Domain.Events;

public class ItemDeactivatedEvent : DomainEvent
{
    public Guid ItemId { get; }

    public ItemDeactivatedEvent(Guid itemId)
    {
        ItemId = itemId;
    }
}
