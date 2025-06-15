using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.SaleAggregate
{
    public class SalePaymentRemovedEvent : DomainEvent
    {
        public Sale Sale { get; }
        public int PaymentId { get; }

        public SalePaymentRemovedEvent(Sale sale, int paymentId)
        {
            Sale = sale;
            PaymentId = paymentId;
        }
    }
} 
