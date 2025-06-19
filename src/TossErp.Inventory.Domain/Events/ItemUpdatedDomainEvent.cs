using TossErp.Domain.SeedWork;
using TossErp.Inventory.Domain.AggregatesModel.ItemAggregate;

namespace TossErp.Inventory.Domain.Events
{
    public class ItemUpdatedDomainEvent : DomainEvent
    {
        public Item Item { get; }

        public ItemUpdatedDomainEvent(Item item)
        {
            Item = item;
        }
    }
} 
