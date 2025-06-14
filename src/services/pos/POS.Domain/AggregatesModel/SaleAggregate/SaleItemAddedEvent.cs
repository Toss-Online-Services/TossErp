using TossErp.POS.Domain.SeedWork;

namespace TossErp.POS.Domain.AggregatesModel.SaleAggregate
{
    public class SaleItemAddedEvent : DomainEvent
    {
        public Sale Sale { get; }
        public SaleItem Item { get; }

        public SaleItemAddedEvent(Sale sale, SaleItem item)
        {
            Sale = sale;
            Item = item;
        }
    }
} 
