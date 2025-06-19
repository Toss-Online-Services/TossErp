using TossErp.Domain.SeedWork;
using TossErp.POS.Domain.AggregatesModel.SaleAggregate;

namespace TossErp.POS.Domain.Events
{
    public class SaleItemsClearedDomainEvent : DomainEvent
    {
        public Sale Sale { get; }

        public SaleItemsClearedDomainEvent(Sale sale)
        {
            Sale = sale;
        }
    }
} 
