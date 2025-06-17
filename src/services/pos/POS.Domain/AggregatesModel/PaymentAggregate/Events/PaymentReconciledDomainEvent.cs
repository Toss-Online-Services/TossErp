using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.PaymentAggregate.Events;

public class PaymentReconciledDomainEvent : IDomainEvent
{
    public Guid PaymentId { get; }
    public Guid SaleId { get; }
    public string Reference { get; }
    public DateTime ReconciledAt { get; }

    public PaymentReconciledDomainEvent(
        Guid paymentId,
        Guid saleId,
        string reference)
    {
        PaymentId = paymentId;
        SaleId = saleId;
        Reference = reference;
        ReconciledAt = DateTime.UtcNow;
    }
} 
