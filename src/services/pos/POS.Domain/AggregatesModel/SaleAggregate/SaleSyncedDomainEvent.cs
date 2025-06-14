using TossErp.POS.Domain.AggregatesModel.SaleAggregate;
using TossErp.POS.Domain.Common;

namespace TossErp.POS.Domain.AggregatesModel.SaleAggregate
{
    public class SaleSyncedDomainEvent : DomainEvent
    {
        public Sale Sale { get; }

        public SaleSyncedDomainEvent(Sale sale)
        {
            Sale = sale;
        }
    }
} 
