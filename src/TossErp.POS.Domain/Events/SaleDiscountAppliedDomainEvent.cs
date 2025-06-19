using TossErp.Domain.SeedWork;
using TossErp.POS.Domain.AggregatesModel.SaleAggregate;

namespace TossErp.POS.Domain.Events
{
    public class SaleDiscountAppliedDomainEvent : DomainEvent
    {
        public Sale Sale { get; }
        public decimal DiscountAmount { get; }

        public SaleDiscountAppliedDomainEvent(Sale sale, decimal discountAmount)
        {
            Sale = sale;
            DiscountAmount = discountAmount;
        }
    }
} 
