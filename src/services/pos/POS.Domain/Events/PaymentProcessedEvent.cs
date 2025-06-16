using POS.Domain.SeedWork;

namespace POS.Domain.Events;

public class PaymentProcessedEvent : DomainEvent
{
    public Guid PaymentId { get; }
    public Guid SaleId { get; }
    public decimal Amount { get; }
    public string Currency { get; }
    public string PaymentMethod { get; }
    public DateTime ProcessedAt { get; }

    public PaymentProcessedEvent(Guid paymentId, Guid saleId, decimal amount, string currency, string paymentMethod)
    {
        PaymentId = paymentId;
        SaleId = saleId;
        Amount = amount;
        Currency = currency;
        PaymentMethod = paymentMethod;
        ProcessedAt = DateTime.UtcNow;
    }
} 
