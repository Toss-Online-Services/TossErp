using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.PaymentAggregate.Events;

public class PaymentCompletedDomainEvent : IDomainEvent
{
    public Guid PaymentId { get; }
    public Guid SaleId { get; }
    public decimal Amount { get; }
    public string Method { get; }
    public string TransactionId { get; }
    public DateTime CompletedAt { get; }

    public PaymentCompletedDomainEvent(
        Guid paymentId,
        Guid saleId,
        decimal amount,
        string method,
        string transactionId)
    {
        PaymentId = paymentId;
        SaleId = saleId;
        Amount = amount;
        Method = method;
        TransactionId = transactionId;
        CompletedAt = DateTime.UtcNow;
    }
} 
