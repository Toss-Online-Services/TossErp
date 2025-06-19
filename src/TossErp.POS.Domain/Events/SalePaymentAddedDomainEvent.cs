using TossErp.Domain.SeedWork;
using TossErp.POS.Domain.AggregatesModel.SaleAggregate;

namespace TossErp.POS.Domain.Events
{
    public class SalePaymentAddedDomainEvent : DomainEvent
    {
        public Sale Sale { get; }
        public SalePayment Payment { get; }

        public SalePaymentAddedDomainEvent(Sale sale, SalePayment payment)
        {
            Sale = sale;
            Payment = payment;
        }
    }
} 
