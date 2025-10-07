using TossErp.Domain.Common;

namespace TossErp.Domain.Entities.Collaboration;

public class EventRegistration : BaseEntity
{
    public int EventId { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    
    // Registration
    public DateTime RegistrationDate { get; set; }
    public RegistrationStatus Status { get; set; } = RegistrationStatus.Registered;
    
    // Payment
    public decimal? FeeAmount { get; set; }
    public bool IsPaid { get; set; }
    public DateTime? PaidDate { get; set; }
    
    // Attendance
    public bool HasAttended { get; set; }
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    
    // Feedback
    public int? Rating { get; set; } // 1-5
    public string? Feedback { get; set; }
    
    // Metadata
    public string? DietaryRequirements { get; set; }
    public string? SpecialNeeds { get; set; }
    public string? Notes { get; set; }
    
    // Navigation
    public CommunityEvent Event { get; set; } = null!;
}

public enum RegistrationStatus
{
    Registered,
    Confirmed,
    Attended,
    NoShow,
    Cancelled
}

