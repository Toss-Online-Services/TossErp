#nullable enable

using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.SaleAggregate.Events;

public class SaleItemAddedDomainEvent : IDomainEvent
{
    public Guid SaleId { get; }
    public Guid SaleItemId { get; }

    public SaleItemAddedDomainEvent(Guid saleId, Guid saleItemId)
    {
        SaleId = saleId;
        SaleItemId = saleItemId;
    }
} 
