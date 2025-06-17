using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.StaffAggregate.Events;

public class StaffReactivatedDomainEvent : IDomainEvent
{
    public Guid StaffId { get; }
    public DateTime ReactivatedAt { get; }

    public StaffReactivatedDomainEvent(Guid staffId)
    {
        StaffId = staffId;
        ReactivatedAt = DateTime.UtcNow;
    }
} 
