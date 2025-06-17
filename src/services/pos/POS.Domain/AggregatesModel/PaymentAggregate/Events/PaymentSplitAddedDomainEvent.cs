using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.PaymentAggregate.Events;

public class PaymentSplitAddedDomainEvent : IDomainEvent
{
    public Guid PaymentId { get; }
    public Guid SaleId { get; }
    public decimal SplitAmount { get; }
    public string SplitMethod { get; }
    public DateTime AddedAt { get; }

    public PaymentSplitAddedDomainEvent(
        Guid paymentId,
        Guid saleId,
        decimal splitAmount,
        string splitMethod)
    {
        PaymentId = paymentId;
        SaleId = saleId;
        SplitAmount = splitAmount;
        SplitMethod = splitMethod;
        AddedAt = DateTime.UtcNow;
    }
} 
