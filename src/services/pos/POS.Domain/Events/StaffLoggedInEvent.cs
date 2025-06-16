using POS.Domain.SeedWork;

namespace POS.Domain.Events;

public class StaffLoggedInEvent : DomainEvent
{
    public Guid StaffId { get; }
    public string StaffName { get; }
    public string Role { get; }
    public DateTime LoggedInAt { get; }

    public StaffLoggedInEvent(Guid staffId, string staffName, string role)
    {
        StaffId = staffId;
        StaffName = staffName;
        Role = role;
        LoggedInAt = DateTime.UtcNow;
    }
} 
