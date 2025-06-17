using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.StoreAggregate.Events;

public class StoreOperatingHoursUpdatedDomainEvent : IDomainEvent
{
    public Guid StoreId { get; }
    public DateTime UpdatedAt { get; }

    public StoreOperatingHoursUpdatedDomainEvent(Guid storeId, DateTime updatedAt)
    {
        StoreId = storeId;
        UpdatedAt = updatedAt;
    }
} 
