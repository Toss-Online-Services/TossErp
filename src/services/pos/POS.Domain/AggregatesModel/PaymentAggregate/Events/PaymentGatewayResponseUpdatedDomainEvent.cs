using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.PaymentAggregate.Events;

public class PaymentGatewayResponseUpdatedDomainEvent : IDomainEvent
{
    public Guid PaymentId { get; }
    public Guid SaleId { get; }
    public string Status { get; }
    public DateTime UpdatedAt { get; }

    public PaymentGatewayResponseUpdatedDomainEvent(
        Guid paymentId,
        Guid saleId,
        string status)
    {
        PaymentId = paymentId;
        SaleId = saleId;
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }
} 
