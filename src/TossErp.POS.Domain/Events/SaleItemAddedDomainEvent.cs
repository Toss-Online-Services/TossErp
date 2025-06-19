using TossErp.Domain.SeedWork;
using TossErp.POS.Domain.AggregatesModel.SaleAggregate;

namespace TossErp.POS.Domain.Events
{
    public class SaleItemAddedDomainEvent : DomainEvent
    {
        public Sale Sale { get; }
        public Guid ItemId { get; }
        public decimal Quantity { get; }

        public SaleItemAddedDomainEvent(Sale sale, Guid itemId, decimal quantity)
        {
            Sale = sale;
            ItemId = itemId;
            Quantity = quantity;
        }
    }
} 
