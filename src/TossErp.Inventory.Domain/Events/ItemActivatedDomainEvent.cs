using TossErp.Domain.SeedWork;
using TossErp.Inventory.Domain.AggregatesModel.ItemAggregate;

namespace TossErp.Inventory.Domain.Events
{
    public class ItemActivatedDomainEvent : DomainEvent
    {
        public Item Item { get; }

        public ItemActivatedDomainEvent(Item item)
        {
            Item = item;
        }
    }
} 
