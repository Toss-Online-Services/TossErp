using POS.Domain.Common.Events;
using POS.Domain.Enums;

namespace POS.Domain.AggregatesModel.PaymentAggregate.Events;

public class PaymentStatusChangedDomainEvent : IDomainEvent
{
    public Guid PaymentId { get; }
    public PaymentStatus Status { get; }
    public DateTime ChangedAt { get; }

    public PaymentStatusChangedDomainEvent(Guid paymentId, PaymentStatus status, DateTime changedAt)
    {
        PaymentId = paymentId;
        Status = status;
        ChangedAt = changedAt;
    }
} 
