using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.SaleAggregate
{
    public class SaleRefundedDomainEvent : DomainEvent
    {
        public Sale Sale { get; }

        public SaleRefundedDomainEvent(Sale sale)
        {
            Sale = sale;
        }
    }
} 
