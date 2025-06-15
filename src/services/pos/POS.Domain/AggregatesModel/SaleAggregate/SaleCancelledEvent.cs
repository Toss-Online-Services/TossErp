using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.SaleAggregate
{
    public class SaleCancelledEvent : DomainEvent
    {
        public Sale Sale { get; }

        public SaleCancelledEvent(Sale sale)
        {
            Sale = sale;
        }
    }
} 
