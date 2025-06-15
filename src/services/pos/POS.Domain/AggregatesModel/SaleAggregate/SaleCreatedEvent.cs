using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.SaleAggregate
{
    public class SaleCreatedEvent : DomainEvent
    {
        public Sale Sale { get; }

        public SaleCreatedEvent(Sale sale)
        {
            Sale = sale;
        }
    }
} 
