using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.CustomerAggregate.Events;

public class CustomerAddressUpdatedDomainEvent : IDomainEvent
{
    public Guid CustomerId { get; }
    public Guid AddressId { get; }
    public string Street { get; }
    public string City { get; }
    public string State { get; }
    public string Country { get; }
    public string PostalCode { get; }
    public string AddressType { get; }
    public DateTime UpdatedAt { get; }

    public CustomerAddressUpdatedDomainEvent(
        Guid customerId,
        Guid addressId,
        string street,
        string city,
        string state,
        string country,
        string postalCode,
        string addressType,
        DateTime updatedAt)
    {
        CustomerId = customerId;
        AddressId = addressId;
        Street = street;
        City = city;
        State = state;
        Country = country;
        PostalCode = postalCode;
        AddressType = addressType;
        UpdatedAt = updatedAt;
    }
} 
