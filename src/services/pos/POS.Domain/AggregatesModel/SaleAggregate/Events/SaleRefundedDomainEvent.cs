#nullable enable
using eShop.POS.Domain.Common;

namespace eShop.POS.Domain.AggregatesModel.SaleAggregate.Events;

public class SaleRefundedDomainEvent : DomainEvent
{
    public Sale Sale { get; }

    public SaleRefundedDomainEvent(Sale sale)
    {
        Sale = sale;
    }
} 
