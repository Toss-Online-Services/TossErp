#nullable enable
using TossErp.POS.Domain.SeedWork;

namespace TossErp.POS.Domain.AggregatesModel.SaleAggregate.Events;

public class SaleItemAddedDomainEvent : DomainEvent
{
    public Sale Sale { get; }
    public SaleItem SaleItem { get; }

    public SaleItemAddedDomainEvent(Sale sale, SaleItem saleItem)
    {
        Sale = sale;
        SaleItem = saleItem;
    }
} 
