using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.CustomerAggregate.Events;

public class CustomerDeactivatedDomainEvent : IDomainEvent
{
    public Guid CustomerId { get; }
    public DateTime DeactivatedAt { get; }

    public CustomerDeactivatedDomainEvent(Guid customerId, DateTime deactivatedAt)
    {
        CustomerId = customerId;
        DeactivatedAt = deactivatedAt;
    }
} 
