using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.StaffAggregate.Events;

public class StaffUpdatedDomainEvent : IDomainEvent
{
    public Guid StaffId { get; }
    public string Name { get; }
    public string Email { get; }
    public string Role { get; }
    public DateTime UpdatedAt { get; }

    public StaffUpdatedDomainEvent(Guid staffId, string name, string email, string role)
    {
        StaffId = staffId;
        Name = name;
        Email = email;
        Role = role;
        UpdatedAt = DateTime.UtcNow;
    }
} 
