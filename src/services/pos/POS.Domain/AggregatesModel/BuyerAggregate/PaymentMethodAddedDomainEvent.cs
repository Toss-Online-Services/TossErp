using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.BuyerAggregate
{
    public class PaymentMethodAddedDomainEvent : DomainEvent
    {
        public Buyer Buyer { get; }
        public PaymentMethod PaymentMethod { get; }

        public PaymentMethodAddedDomainEvent(Buyer buyer, PaymentMethod paymentMethod)
        {
            Buyer = buyer;
            PaymentMethod = paymentMethod;
        }
    }
} 
