using TossErp.Domain.SeedWork;
using TossErp.Inventory.Domain.AggregatesModel.ItemAggregate;

namespace TossErp.Inventory.Domain.Events
{
    public class ItemVariantRemovedDomainEvent : DomainEvent
    {
        public Item Item { get; }
        public ItemVariant Variant { get; }

        public ItemVariantRemovedDomainEvent(Item item, ItemVariant variant)
        {
            Item = item;
            Variant = variant;
        }
    }
} 
