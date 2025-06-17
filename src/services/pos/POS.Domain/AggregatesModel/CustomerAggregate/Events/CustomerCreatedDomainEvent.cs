using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.CustomerAggregate.Events;

public class CustomerCreatedDomainEvent : IDomainEvent
{
    public Guid CustomerId { get; }
    public string Name { get; }
    public string Email { get; }

    public CustomerCreatedDomainEvent(Guid customerId, string name, string email)
    {
        CustomerId = customerId;
        Name = name;
        Email = email;
    }
} 
