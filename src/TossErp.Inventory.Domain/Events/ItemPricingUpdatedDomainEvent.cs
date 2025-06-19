using TossErp.Domain.SeedWork;
using TossErp.Inventory.Domain.AggregatesModel.ItemAggregate;

namespace TossErp.Inventory.Domain.Events
{
    public class ItemPricingUpdatedDomainEvent : DomainEvent
    {
        public Item Item { get; }
        public decimal OldStandardCost { get; }
        public decimal NewStandardCost { get; }
        public decimal OldSellingPrice { get; }
        public decimal NewSellingPrice { get; }

        public ItemPricingUpdatedDomainEvent(Item item, decimal newStandardCost, decimal newSellingPrice)
        {
            Item = item;
            OldStandardCost = item.StandardCost;
            NewStandardCost = newStandardCost;
            OldSellingPrice = item.SellingPrice;
            NewSellingPrice = newSellingPrice;
        }
    }
} 
