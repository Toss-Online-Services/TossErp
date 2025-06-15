using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.SaleAggregate
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
