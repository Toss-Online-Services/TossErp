using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.SaleAggregate.Events;

public class SaleItemQuantityChangedDomainEvent : IDomainEvent
{
    public Guid SaleId { get; }
    public Guid SaleItemId { get; }
    public int OldQuantity { get; }
    public int NewQuantity { get; }
    public DateTime ChangedAt { get; }

    public SaleItemQuantityChangedDomainEvent(Guid saleId, Guid saleItemId, int oldQuantity, int newQuantity, DateTime changedAt)
    {
        SaleId = saleId;
        SaleItemId = saleItemId;
        OldQuantity = oldQuantity;
        NewQuantity = newQuantity;
        ChangedAt = changedAt;
    }
} 
