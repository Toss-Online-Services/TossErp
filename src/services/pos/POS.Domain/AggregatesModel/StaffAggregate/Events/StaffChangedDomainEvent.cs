using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.StaffAggregate.Events;

public class StaffChangedDomainEvent : IDomainEvent
{
    public Guid StaffId { get; }
    public DateTime ChangedAt { get; }

    public StaffChangedDomainEvent(Guid staffId, DateTime changedAt)
    {
        StaffId = staffId;
        ChangedAt = changedAt;
    }
} 
