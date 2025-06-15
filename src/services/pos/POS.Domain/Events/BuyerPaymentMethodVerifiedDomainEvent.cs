using POS.Domain.SeedWork;

namespace POS.Domain.Events;

public class BuyerPaymentMethodVerifiedDomainEvent : DomainEvent
{
    public int BuyerId { get; }
    public int PaymentMethodId { get; }

    public BuyerPaymentMethodVerifiedDomainEvent(int buyerId, int paymentMethodId)
    {
        BuyerId = buyerId;
        PaymentMethodId = paymentMethodId;
    }
}
