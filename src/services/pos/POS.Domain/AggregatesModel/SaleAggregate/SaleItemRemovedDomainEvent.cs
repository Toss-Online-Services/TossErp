using TossErp.POS.Domain.AggregatesModel.SaleAggregate;
using TossErp.POS.Domain.Common;

namespace TossErp.POS.Domain.AggregatesModel.SaleAggregate
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
