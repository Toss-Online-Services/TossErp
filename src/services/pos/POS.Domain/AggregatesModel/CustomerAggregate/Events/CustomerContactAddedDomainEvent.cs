using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.CustomerAggregate.Events;

public class CustomerContactAddedDomainEvent : IDomainEvent
{
    public Guid CustomerId { get; }
    public Guid ContactId { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string Email { get; }
    public string PhoneNumber { get; }
    public string ContactType { get; }
    public string AddedBy { get; }
    public DateTime AddedAt { get; }

    public CustomerContactAddedDomainEvent(
        Guid customerId,
        Guid contactId,
        string firstName,
        string lastName,
        string email,
        string phoneNumber,
        string contactType,
        string addedBy,
        DateTime addedAt)
    {
        CustomerId = customerId;
        ContactId = contactId;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        ContactType = contactType;
        AddedBy = addedBy;
        AddedAt = addedAt;
    }
} 
