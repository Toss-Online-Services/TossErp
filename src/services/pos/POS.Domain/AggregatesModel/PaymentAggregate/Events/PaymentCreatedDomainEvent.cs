using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.PaymentAggregate.Events;

public class PaymentCreatedDomainEvent : IDomainEvent
{
    public Guid PaymentId { get; }
    public Guid SaleId { get; }
    public decimal Amount { get; }
    public string Method { get; }
    public DateTime CreatedAt { get; }

    public PaymentCreatedDomainEvent(Guid paymentId, Guid saleId, decimal amount, string method)
    {
        PaymentId = paymentId;
        SaleId = saleId;
        Amount = amount;
        Method = method;
        CreatedAt = DateTime.UtcNow;
    }
} 
