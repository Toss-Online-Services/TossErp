using TossErp.Domain.SeedWork;
using TossErp.Inventory.Domain.AggregatesModel.ItemAggregate;

namespace TossErp.Inventory.Domain.Events
{
    public class ItemVariantAddedDomainEvent : DomainEvent
    {
        public Item Item { get; }
        public ItemVariant Variant { get; }

        public ItemVariantAddedDomainEvent(Item item, ItemVariant variant)
        {
            Item = item;
            Variant = variant;
        }
    }
} 
