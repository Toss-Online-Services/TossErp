using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.CustomerAggregate.Events;

public class CustomerContactRemovedDomainEvent : IDomainEvent
{
    public Guid CustomerId { get; }
    public Guid ContactId { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string RemovedBy { get; }
    public DateTime RemovedAt { get; }

    public CustomerContactRemovedDomainEvent(
        Guid customerId,
        Guid contactId,
        string firstName,
        string lastName,
        string removedBy,
        DateTime removedAt)
    {
        CustomerId = customerId;
        ContactId = contactId;
        FirstName = firstName;
        LastName = lastName;
        RemovedBy = removedBy;
        RemovedAt = removedAt;
    }
} 
