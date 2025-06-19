using TossErp.Domain.SeedWork;
using TossErp.POS.Domain.AggregatesModel.SaleAggregate;

namespace TossErp.POS.Domain.Events
{
    public class SaleItemRemovedDomainEvent : DomainEvent
    {
        public Sale Sale { get; }
        public Guid ItemId { get; }

        public SaleItemRemovedDomainEvent(Sale sale, Guid itemId)
        {
            Sale = sale;
            ItemId = itemId;
        }
    }
} 
