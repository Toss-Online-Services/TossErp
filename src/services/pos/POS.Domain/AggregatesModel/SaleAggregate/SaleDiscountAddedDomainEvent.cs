using TossErp.POS.Domain.AggregatesModel.SaleAggregate;
using TossErp.POS.Domain.SeedWork;

namespace TossErp.POS.Domain.AggregatesModel.SaleAggregate
{
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
} 
