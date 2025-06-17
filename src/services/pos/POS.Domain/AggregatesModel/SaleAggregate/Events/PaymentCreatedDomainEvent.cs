using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.SaleAggregate.Events;

public class PaymentCreatedDomainEvent : DomainEvent, IDomainEvent
{
    public Guid PaymentId { get; }
    public Guid SaleId { get; }
    public decimal Amount { get; }
    public string PaymentMethod { get; }
    public DateTime CreatedAt { get; }

    public PaymentCreatedDomainEvent(Guid paymentId, Guid saleId, decimal amount, string paymentMethod)
    {
        PaymentId = paymentId;
        SaleId = saleId;
        Amount = amount;
        PaymentMethod = paymentMethod;
        CreatedAt = DateTime.UtcNow;
    }
} 
