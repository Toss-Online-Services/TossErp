using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.StaffAggregate.Events;

public class StaffScheduleUpdatedDomainEvent : IDomainEvent
{
    public Guid StaffId { get; }
    public DateTime UpdatedAt { get; }

    public StaffScheduleUpdatedDomainEvent(Guid staffId)
    {
        StaffId = staffId;
        UpdatedAt = DateTime.UtcNow;
    }
} 
