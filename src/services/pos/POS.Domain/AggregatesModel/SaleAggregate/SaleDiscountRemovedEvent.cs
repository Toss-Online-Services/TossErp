using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.SaleAggregate
{
    public class SaleDiscountRemovedEvent : DomainEvent
    {
        public Sale Sale { get; }
        public int DiscountId { get; }

        public SaleDiscountRemovedEvent(Sale sale, int discountId)
        {
            Sale = sale;
            DiscountId = discountId;
        }
    }
} 
