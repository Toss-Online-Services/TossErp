#nullable enable
using eShop.POS.Domain.Common;

namespace eShop.POS.Domain.AggregatesModel.SaleAggregate.Events;

public class SaleDiscountAddedDomainEvent : DomainEvent
{
    public Sale Sale { get; }
    public SaleDiscount Discount { get; }

    public SaleDiscountAddedDomainEvent(Sale sale, SaleDiscount discount)
    {
        Sale = sale;
        Discount = discount;
    }
} 
