namespace Toss.Domain.Events;

public class PaymentReceivedEvent : BaseEvent
{
    public PaymentReceivedEvent(Payment payment)
    {
        Payment = payment;
    }

    public Payment Payment { get; }
}

