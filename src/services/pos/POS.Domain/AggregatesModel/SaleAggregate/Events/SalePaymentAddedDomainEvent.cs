using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.SaleAggregate.Events;

public class SalePaymentAddedDomainEvent : IDomainEvent
{
    public Guid SaleId { get; }
    public Guid PaymentId { get; }

    public SalePaymentAddedDomainEvent(Guid saleId, Guid paymentId)
    {
        SaleId = saleId;
        PaymentId = paymentId;
    }
} 
