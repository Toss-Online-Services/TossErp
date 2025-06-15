using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.SaleAggregate
{
    public class SaleItemRemovedEvent : DomainEvent
    {
        public Sale Sale { get; }
        public int ProductId { get; }

        public SaleItemRemovedEvent(Sale sale, int productId)
        {
            Sale = sale;
            ProductId = productId;
        }
    }
} 
