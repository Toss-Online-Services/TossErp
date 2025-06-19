using TossErp.Domain.SeedWork;
using TossErp.Inventory.Domain.AggregatesModel.ItemAggregate;

namespace TossErp.Inventory.Domain.Events
{
    public class ItemDeactivatedDomainEvent : DomainEvent
    {
        public Item Item { get; }

        public ItemDeactivatedDomainEvent(Item item)
        {
            Item = item;
        }
    }
} 
