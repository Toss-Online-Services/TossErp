using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.PaymentAggregate.Events;

public class PaymentRetryDomainEvent : IDomainEvent
{
    public Guid PaymentId { get; }
    public int RetryCount { get; }
    public string RetriedBy { get; }
    public DateTime RetriedAt { get; }

    public PaymentRetryDomainEvent(
        Guid paymentId,
        int retryCount,
        string retriedBy,
        DateTime retriedAt)
    {
        PaymentId = paymentId;
        RetryCount = retryCount;
        RetriedBy = retriedBy;
        RetriedAt = retriedAt;
    }
} 
