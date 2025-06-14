using TossErp.POS.Domain.AggregatesModel.SaleAggregate;
using TossErp.POS.Domain.Common;

namespace TossErp.POS.Domain.AggregatesModel.SaleAggregate
{
    public class SaleRefundedDomainEvent : DomainEvent
    {
        public Sale Sale { get; }

        public SaleRefundedDomainEvent(Sale sale)
        {
            Sale = sale;
        }
    }
} 
