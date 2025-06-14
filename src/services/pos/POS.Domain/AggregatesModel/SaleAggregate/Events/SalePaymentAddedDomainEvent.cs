#nullable enable
using TossErp.POS.Domain.Events;

namespace TossErp.POS.Domain.AggregatesModel.SaleAggregate.Events;

public class SalePaymentAddedDomainEvent : DomainEvent
{
    public int SaleId { get; }
    public int PaymentId { get; }
    public decimal Amount { get; }

    public SalePaymentAddedDomainEvent(int saleId, int paymentId, decimal amount)
    {
        SaleId = saleId;
        PaymentId = paymentId;
        Amount = amount;
    }
} 
