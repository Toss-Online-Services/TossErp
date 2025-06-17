using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.CustomerAggregate.Events;

public class CustomerPriceListAddedDomainEvent : IDomainEvent
{
    public Guid CustomerId { get; }
    public Guid PriceListId { get; }
    public string PriceListName { get; }
    public bool IsDefault { get; }
    public string AddedBy { get; }
    public DateTime AddedAt { get; }

    public CustomerPriceListAddedDomainEvent(
        Guid customerId,
        Guid priceListId,
        string priceListName,
        bool isDefault,
        string addedBy,
        DateTime addedAt)
    {
        CustomerId = customerId;
        PriceListId = priceListId;
        PriceListName = priceListName;
        IsDefault = isDefault;
        AddedBy = addedBy;
        AddedAt = addedAt;
    }
} 
