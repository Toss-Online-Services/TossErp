using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.StaffAggregate.Events;

public class StaffPermissionsUpdatedDomainEvent : IDomainEvent
{
    public Guid StaffId { get; }
    public DateTime UpdatedAt { get; }

    public StaffPermissionsUpdatedDomainEvent(Guid staffId)
    {
        StaffId = staffId;
        UpdatedAt = DateTime.UtcNow;
    }
} 
