using TossErp.Domain.SeedWork;
using TossErp.POS.Domain.AggregatesModel.SaleAggregate;

namespace TossErp.POS.Domain.Events
{
    public class SaleUpdatedDomainEvent : DomainEvent
    {
        public Sale Sale { get; }

        public SaleUpdatedDomainEvent(Sale sale)
        {
            Sale = sale;
        }
    }
} 
