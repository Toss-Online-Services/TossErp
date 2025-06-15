using TossErp.POS.Domain.AggregatesModel.SaleAggregate;
using TossErp.POS.Domain.SeedWork;

namespace TossErp.POS.Domain.AggregatesModel.SaleAggregate
{
    public class SaleVoidedDomainEvent : DomainEvent
    {
        public Sale Sale { get; }

        public SaleVoidedDomainEvent(Sale sale)
        {
            Sale = sale;
        }
    }
} 
