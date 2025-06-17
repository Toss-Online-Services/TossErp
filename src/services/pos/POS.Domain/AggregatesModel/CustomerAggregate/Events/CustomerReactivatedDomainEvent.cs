using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.CustomerAggregate.Events;

public class CustomerReactivatedDomainEvent : IDomainEvent
{
    public Guid CustomerId { get; }
    public DateTime ReactivatedAt { get; }

    public CustomerReactivatedDomainEvent(Guid customerId, DateTime reactivatedAt)
    {
        CustomerId = customerId;
        ReactivatedAt = reactivatedAt;
    }
} 
