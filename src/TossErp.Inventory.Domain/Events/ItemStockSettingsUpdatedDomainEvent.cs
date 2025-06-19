using TossErp.Domain.SeedWork;
using TossErp.Inventory.Domain.AggregatesModel.ItemAggregate;

namespace TossErp.Inventory.Domain.Events
{
    public class ItemStockSettingsUpdatedDomainEvent : DomainEvent
    {
        public Item Item { get; }

        public ItemStockSettingsUpdatedDomainEvent(Item item)
        {
            Item = item;
        }
    }
} 
