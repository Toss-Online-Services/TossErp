using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.CustomerAggregate.Events;

public class CustomerCreatedDomainEvent : IDomainEvent
{
    public Guid CustomerId { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string Email { get; }

    public CustomerCreatedDomainEvent(Guid customerId, string firstName, string lastName, string email)
    {
        CustomerId = customerId;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }
} 
