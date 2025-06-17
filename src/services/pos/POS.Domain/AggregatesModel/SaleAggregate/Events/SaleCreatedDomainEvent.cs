using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.SaleAggregate.Events;

public class SaleCreatedDomainEvent : IDomainEvent
{
    public Guid SaleId { get; }
    public Guid StoreId { get; }
    public DateTime CreatedAt { get; }

    public SaleCreatedDomainEvent(Guid saleId, Guid storeId, DateTime createdAt)
    {
        SaleId = saleId;
        StoreId = storeId;
        CreatedAt = createdAt;
    }
} 
