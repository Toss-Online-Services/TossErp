using TossErp.Domain.SeedWork;
using TossErp.Inventory.Domain.AggregatesModel.ItemAggregate;

namespace TossErp.Inventory.Domain.Events
{
    public class ItemCreatedDomainEvent : DomainEvent
    {
        public Item Item { get; }

        public ItemCreatedDomainEvent(Item item)
        {
            Item = item;
        }
    }
} 
