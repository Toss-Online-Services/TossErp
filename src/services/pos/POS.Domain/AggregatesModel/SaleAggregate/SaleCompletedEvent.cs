using TossErp.POS.Domain.SeedWork;

namespace TossErp.POS.Domain.AggregatesModel.SaleAggregate
{
    public class SaleCompletedEvent : DomainEvent
    {
        public Sale Sale { get; }

        public SaleCompletedEvent(Sale sale)
        {
            Sale = sale;
        }
    }
} 
