using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.PaymentAggregate.Events;

public class PaymentProcessingDomainEvent : IDomainEvent
{
    public Guid PaymentId { get; }
    public Guid SaleId { get; }
    public decimal Amount { get; }
    public string Method { get; }
    public DateTime ProcessedAt { get; }

    public PaymentProcessingDomainEvent(
        Guid paymentId,
        Guid saleId,
        decimal amount,
        string method)
    {
        PaymentId = paymentId;
        SaleId = saleId;
        Amount = amount;
        Method = method;
        ProcessedAt = DateTime.UtcNow;
    }
} 
