using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.CustomerAggregate.Events;

public class CustomerUpdatedDomainEvent : IDomainEvent
{
    public Guid CustomerId { get; }
    public string Name { get; }
    public string Email { get; }
    public string Phone { get; }
    public string? Address { get; }
    public DateTime UpdatedAt { get; }

    public CustomerUpdatedDomainEvent(Guid customerId, string name, string email, string phone, string? address)
    {
        CustomerId = customerId;
        Name = name;
        Email = email;
        Phone = phone;
        Address = address;
        UpdatedAt = DateTime.UtcNow;
    }
} 
