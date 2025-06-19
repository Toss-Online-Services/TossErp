using TossErp.Domain.SeedWork;
using TossErp.POS.Domain.AggregatesModel.SaleAggregate;

namespace TossErp.POS.Domain.Events
{
    public class SaleCancelledDomainEvent : DomainEvent
    {
        public Sale Sale { get; }
        public string Reason { get; }

        public SaleCancelledDomainEvent(Sale sale, string reason)
        {
            Sale = sale;
            Reason = reason;
        }
    }
} 
