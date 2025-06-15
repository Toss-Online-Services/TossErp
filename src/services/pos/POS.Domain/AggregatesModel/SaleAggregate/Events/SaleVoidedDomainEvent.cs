#nullable enable
using TossErp.POS.Domain.SeedWork;

namespace TossErp.POS.Domain.AggregatesModel.SaleAggregate.Events;

public class SaleVoidedDomainEvent : DomainEvent
{
    public int SaleId { get; }
    public string Reason { get; }

    public SaleVoidedDomainEvent(int saleId, string reason)
    {
        SaleId = saleId;
        Reason = reason;
    }
} 
