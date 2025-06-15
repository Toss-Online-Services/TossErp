using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.SaleAggregate
{
    public class SaleItemRemovedDomainEvent : DomainEvent
    {
        public Sale Sale { get; }
        public SaleItem SaleItem { get; }

        public SaleItemRemovedDomainEvent(Sale sale, SaleItem saleItem)
        {
            Sale = sale;
            SaleItem = saleItem;
        }
    }
} 
