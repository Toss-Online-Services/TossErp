using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.SaleAggregate.Events;

public class SaleVoidedDomainEvent : IDomainEvent
{
    public Guid SaleId { get; }
    public string Reason { get; }
    public DateTime VoidedAt { get; }
    public Guid? ApprovedBy { get; }

    public SaleVoidedDomainEvent(Guid saleId, string reason, DateTime voidedAt, Guid? approvedBy = null)
    {
        SaleId = saleId;
        Reason = reason;
        VoidedAt = voidedAt;
        ApprovedBy = approvedBy;
    }
}
