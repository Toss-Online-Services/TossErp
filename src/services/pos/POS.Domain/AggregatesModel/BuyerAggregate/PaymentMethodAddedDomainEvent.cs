using TossErp.POS.Domain.AggregatesModel.BuyerAggregate;
using TossErp.POS.Domain.SeedWork;

namespace TossErp.POS.Domain.AggregatesModel.BuyerAggregate
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
