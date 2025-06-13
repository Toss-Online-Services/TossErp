#nullable enable
using eShop.POS.Domain.Common;

namespace eShop.POS.Domain.AggregatesModel.SaleAggregate.Events;

public class SaleVoidedDomainEvent : DomainEvent
{
    public Sale Sale { get; }

    public SaleVoidedDomainEvent(Sale sale)
    {
        Sale = sale;
    }
} 
