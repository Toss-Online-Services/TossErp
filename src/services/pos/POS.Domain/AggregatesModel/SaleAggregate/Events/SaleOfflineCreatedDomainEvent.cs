using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.SaleAggregate.Events
{
    public class SaleOfflineCreatedDomainEvent : IDomainEvent
    {
        public Guid SaleId { get; }
        public string SaleNumber { get; }
        public Guid StoreId { get; }
        public DateTime CreatedAt { get; }
        public bool RequiresSync { get; }

        public SaleOfflineCreatedDomainEvent(
            Guid saleId,
            string saleNumber,
            Guid storeId,
            DateTime createdAt,
            bool requiresSync)
        {
            SaleId = saleId;
            SaleNumber = saleNumber;
            StoreId = storeId;
            CreatedAt = createdAt;
            RequiresSync = requiresSync;
        }
    }
} 
