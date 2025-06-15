using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.SaleAggregate
{
    public class SaleItemUnitPriceChangedEvent : DomainEvent
    {
        public Sale Sale { get; }
        public SaleItem Item { get; }
        public decimal OldUnitPrice { get; }
        public decimal NewUnitPrice { get; }

        public SaleItemUnitPriceChangedEvent(Sale sale, SaleItem item, decimal oldUnitPrice, decimal newUnitPrice)
        {
            Sale = sale;
            Item = item;
            OldUnitPrice = oldUnitPrice;
            NewUnitPrice = newUnitPrice;
        }
    }
} 
