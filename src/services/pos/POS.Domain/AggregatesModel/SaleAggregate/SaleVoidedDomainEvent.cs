using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.SaleAggregate
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
