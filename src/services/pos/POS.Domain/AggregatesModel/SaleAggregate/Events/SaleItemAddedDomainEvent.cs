#nullable enable
using eShop.POS.Domain.Common;

namespace eShop.POS.Domain.AggregatesModel.SaleAggregate.Events;

public class SaleItemAddedDomainEvent : DomainEvent
{
    public Sale Sale { get; }
    public SaleItem Item { get; }

    public SaleItemAddedDomainEvent(Sale sale, SaleItem item)
    {
        Sale = sale;
        Item = item;
    }
} 
