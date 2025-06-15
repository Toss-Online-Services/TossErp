using TossErp.POS.Domain.SeedWork;

namespace TossErp.POS.Domain.AggregatesModel.SaleAggregate.Events
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
