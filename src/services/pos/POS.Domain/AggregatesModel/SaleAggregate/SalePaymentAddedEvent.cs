using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.SaleAggregate
{
    public class SalePaymentAddedEvent : DomainEvent
    {
        public Sale Sale { get; }
        public Payment Payment { get; }

        public SalePaymentAddedEvent(Sale sale, Payment payment)
        {
            Sale = sale;
            Payment = payment;
        }
    }
} 
