using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.PaymentAggregate.Events;

public class PaymentEventAddedDomainEvent : IDomainEvent
{
    public Guid PaymentId { get; }
    public Guid EventId { get; }
    public DateTime AddedAt { get; }

    public PaymentEventAddedDomainEvent(Guid paymentId, Guid eventId)
    {
        PaymentId = paymentId;
        EventId = eventId;
        AddedAt = DateTime.UtcNow;
    }
} 
