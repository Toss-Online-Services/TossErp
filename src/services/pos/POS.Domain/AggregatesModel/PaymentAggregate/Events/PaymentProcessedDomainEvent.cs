using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.PaymentAggregate.Events;

public class PaymentProcessedDomainEvent : IDomainEvent
{
    public Guid PaymentId { get; }
    public DateTime ProcessedAt { get; }

    public PaymentProcessedDomainEvent(Guid paymentId, DateTime processedAt)
    {
        PaymentId = paymentId;
        ProcessedAt = processedAt;
    }
} 
