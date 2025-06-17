using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.CustomerAggregate.Events;

public class CustomerContactInfoUpdatedDomainEvent : IDomainEvent
{
    public Guid CustomerId { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string Email { get; }
    public string PhoneNumber { get; }
    public DateTime UpdatedAt { get; }

    public CustomerContactInfoUpdatedDomainEvent(
        Guid customerId,
        string firstName,
        string lastName,
        string email,
        string phoneNumber,
        DateTime updatedAt)
    {
        CustomerId = customerId;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        UpdatedAt = updatedAt;
    }
} 
