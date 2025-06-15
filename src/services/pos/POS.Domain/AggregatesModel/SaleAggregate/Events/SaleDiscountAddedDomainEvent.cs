#nullable enable
using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.SaleAggregate.Events;

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
