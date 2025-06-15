using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.SaleAggregate
{
    public class SaleDiscountAddedEvent : DomainEvent
    {
        public Sale Sale { get; }
        public SaleDiscount Discount { get; }

        public SaleDiscountAddedEvent(Sale sale, SaleDiscount discount)
        {
            Sale = sale;
            Discount = discount;
        }
    }
} 
