using TossErp.Domain.SeedWork;
using TossErp.Inventory.Domain.AggregatesModel.ItemAggregate;

namespace TossErp.Inventory.Domain.Events
{
    public class ItemCategoryUpdatedDomainEvent : DomainEvent
    {
        public Item Item { get; }
        public Guid? NewCategoryId { get; }

        public ItemCategoryUpdatedDomainEvent(Item item, Guid? newCategoryId)
        {
            Item = item;
            NewCategoryId = newCategoryId;
        }
    }
} 
