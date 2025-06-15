#nullable enable
using TossErp.POS.Domain.Events;
using TossErp.POS.Domain.SeedWork;

namespace TossErp.POS.Domain.AggregatesModel.SaleAggregate.Events;

public class SaleDiscountAddedDomainEvent : DomainEvent
{
    public Sale Sale { get; }
    public SaleDiscount SaleDiscount { get; }

    public SaleDiscountAddedDomainEvent(Sale sale, SaleDiscount saleDiscount)
    {
        Sale = sale;
        SaleDiscount = saleDiscount;
    }
} 
