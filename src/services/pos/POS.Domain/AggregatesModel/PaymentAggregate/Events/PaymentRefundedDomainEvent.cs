using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.PaymentAggregate.Events;

public class PaymentRefundedDomainEvent : IDomainEvent
{
    public Guid PaymentId { get; }
    public decimal RefundedAmount { get; }
    public string RefundedBy { get; }
    public DateTime RefundedAt { get; }
    public string Reason { get; }

    public PaymentRefundedDomainEvent(
        Guid paymentId,
        decimal refundedAmount,
        string refundedBy,
        DateTime refundedAt,
        string reason)
    {
        PaymentId = paymentId;
        RefundedAmount = refundedAmount;
        RefundedBy = refundedBy;
        RefundedAt = refundedAt;
        Reason = reason;
    }
} 
