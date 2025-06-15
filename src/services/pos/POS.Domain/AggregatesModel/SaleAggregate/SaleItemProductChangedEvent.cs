using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.SaleAggregate
{
    public class SaleItemProductChangedEvent : DomainEvent
    {
        public Sale Sale { get; }
        public SaleItem Item { get; }
        public int OldProductId { get; }
        public int NewProductId { get; }
        public string OldProductName { get; }
        public string NewProductName { get; }

        public SaleItemProductChangedEvent(Sale sale, SaleItem item, int oldProductId, int newProductId, string oldProductName, string newProductName)
        {
            Sale = sale;
            Item = item;
            OldProductId = oldProductId;
            NewProductId = newProductId;
            OldProductName = oldProductName;
            NewProductName = newProductName;
        }
    }
} 
