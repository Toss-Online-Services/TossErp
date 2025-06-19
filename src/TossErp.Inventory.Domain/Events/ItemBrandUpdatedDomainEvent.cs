using TossErp.Domain.SeedWork;
using TossErp.Inventory.Domain.AggregatesModel.ItemAggregate;

namespace TossErp.Inventory.Domain.Events
{
    public class ItemBrandUpdatedDomainEvent : DomainEvent
    {
        public Item Item { get; }
        public Guid? NewBrandId { get; }

        public ItemBrandUpdatedDomainEvent(Item item, Guid? newBrandId)
        {
            Item = item;
            NewBrandId = newBrandId;
        }
    }
} 
