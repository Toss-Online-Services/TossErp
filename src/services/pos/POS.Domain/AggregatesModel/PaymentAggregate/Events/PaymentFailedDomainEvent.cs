using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.PaymentAggregate.Events;

public class PaymentFailedDomainEvent : IDomainEvent
{
    public Guid PaymentId { get; }
    public Guid SaleId { get; }
    public decimal Amount { get; }
    public string Method { get; }
    public string ErrorMessage { get; }
    public DateTime FailedAt { get; }

    public PaymentFailedDomainEvent(
        Guid paymentId,
        Guid saleId,
        decimal amount,
        string method,
        string errorMessage)
    {
        PaymentId = paymentId;
        SaleId = saleId;
        Amount = amount;
        Method = method;
        ErrorMessage = errorMessage;
        FailedAt = DateTime.UtcNow;
    }
} 
