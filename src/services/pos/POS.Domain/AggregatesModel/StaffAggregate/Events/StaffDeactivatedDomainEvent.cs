using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.StaffAggregate.Events;

public class StaffDeactivatedDomainEvent : IDomainEvent
{
    public Guid StaffId { get; }
    public DateTime DeactivatedAt { get; }

    public StaffDeactivatedDomainEvent(Guid staffId)
    {
        StaffId = staffId;
        DeactivatedAt = DateTime.UtcNow;
    }
} 
