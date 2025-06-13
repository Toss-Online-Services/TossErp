using eShop.POS.Domain.AggregatesModel.BuyerAggregate;
using eShop.POS.Domain.Common;

namespace eShop.POS.Domain.Events;

public class BuyerPaymentMethodVerifiedDomainEvent : DomainEvent
{
    public Buyer Buyer { get; }
    public PaymentMethod PaymentMethod { get; }

    public BuyerPaymentMethodVerifiedDomainEvent(Buyer buyer, PaymentMethod paymentMethod)
    {
        Buyer = buyer;
        PaymentMethod = paymentMethod;
    }
}
