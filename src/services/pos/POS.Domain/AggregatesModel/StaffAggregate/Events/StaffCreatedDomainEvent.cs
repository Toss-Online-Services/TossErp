using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.StaffAggregate.Events;

public class StaffCreatedDomainEvent : IDomainEvent
{
    public Guid StaffId { get; }
    public string Name { get; }
    public string Email { get; }
    public string Role { get; }
    public DateTime CreatedAt { get; }

    public StaffCreatedDomainEvent(Guid staffId, string name, string email, string role)
    {
        StaffId = staffId;
        Name = name;
        Email = email;
        Role = role;
        CreatedAt = DateTime.UtcNow;
    }
} 
