using TossErp.Domain.SeedWork;
using TossErp.Inventory.Domain.AggregatesModel.ItemAggregate;

namespace TossErp.Inventory.Domain.Events
{
    public class ItemSupplierUpdatedDomainEvent : DomainEvent
    {
        public Item Item { get; }
        public Guid? NewSupplierId { get; }

        public ItemSupplierUpdatedDomainEvent(Item item, Guid? newSupplierId)
        {
            Item = item;
            NewSupplierId = newSupplierId;
        }
    }
} 
