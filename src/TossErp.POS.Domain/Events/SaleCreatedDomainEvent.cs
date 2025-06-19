using TossErp.Domain.SeedWork;
using TossErp.POS.Domain.AggregatesModel.SaleAggregate;

namespace TossErp.POS.Domain.Events
{
    public class SaleCreatedDomainEvent : DomainEvent
    {
        public Sale Sale { get; }

        public SaleCreatedDomainEvent(Sale sale)
        {
            Sale = sale;
        }
    }
} 
