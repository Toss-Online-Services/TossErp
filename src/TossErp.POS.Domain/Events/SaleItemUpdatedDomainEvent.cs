using TossErp.Domain.SeedWork;
using TossErp.POS.Domain.AggregatesModel.SaleAggregate;

namespace TossErp.POS.Domain.Events
{
    public class SaleItemUpdatedDomainEvent : DomainEvent
    {
        public Sale Sale { get; }
        public Guid ItemId { get; }
        public decimal NewQuantity { get; }

        public SaleItemUpdatedDomainEvent(Sale sale, Guid itemId, decimal newQuantity)
        {
            Sale = sale;
            ItemId = itemId;
            NewQuantity = newQuantity;
        }
    }
} 
