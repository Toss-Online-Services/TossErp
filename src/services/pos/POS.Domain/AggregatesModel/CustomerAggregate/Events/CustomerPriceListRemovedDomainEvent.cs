using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.CustomerAggregate.Events;

public class CustomerPriceListRemovedDomainEvent : IDomainEvent
{
    public Guid CustomerId { get; }
    public Guid PriceListId { get; }
    public string PriceListName { get; }
    public string RemovedBy { get; }
    public DateTime RemovedAt { get; }

    public CustomerPriceListRemovedDomainEvent(
        Guid customerId,
        Guid priceListId,
        string priceListName,
        string removedBy,
        DateTime removedAt)
    {
        CustomerId = customerId;
        PriceListId = priceListId;
        PriceListName = priceListName;
        RemovedBy = removedBy;
        RemovedAt = removedAt;
    }
} 
