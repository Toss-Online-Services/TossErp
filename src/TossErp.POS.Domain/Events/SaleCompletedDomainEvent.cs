using TossErp.Domain.SeedWork;
using TossErp.POS.Domain.AggregatesModel.SaleAggregate;

namespace TossErp.POS.Domain.Events
{
    public class SaleCompletedDomainEvent : DomainEvent
    {
        public Sale Sale { get; }

        public SaleCompletedDomainEvent(Sale sale)
        {
            Sale = sale;
        }
    }
} 
