using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.SaleAggregate
{
    public class SaleItemDiscountChangedEvent : DomainEvent
    {
        public Sale Sale { get; }
        public SaleItem Item { get; }
        public decimal OldDiscount { get; }
        public decimal NewDiscount { get; }

        public SaleItemDiscountChangedEvent(Sale sale, SaleItem item, decimal oldDiscount, decimal newDiscount)
        {
            Sale = sale;
            Item = item;
            OldDiscount = oldDiscount;
            NewDiscount = newDiscount;
        }
    }
} 
