using TossErp.Domain.Common;

namespace TossErp.Domain.Entities.Collaboration;

/// <summary>
/// Community events for networking and collaboration
/// </summary>
public class CommunityEvent : BaseEntity
{
    public string EventNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public EventType Type { get; set; }
    public EventStatus Status { get; set; } = EventStatus.Draft;
    
    // Organizer
    public int OrganizerId { get; set; }
    public string OrganizerName { get; set; } = string.Empty;
    public string? OrganizerContact { get; set; }
    
    // Timing
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public int DurationMinutes { get; set; }
    
    // Location
    public bool IsOnline { get; set; }
    public string? VenueAddress { get; set; }
    public string? VenueName { get; set; }
    public string? OnlineMeetingUrl { get; set; }
    public string? OnlineMeetingPlatform { get; set; } // Zoom, Teams, etc.
    
    // Capacity
    public int? MaximumAttendees { get; set; }
    public int RegisteredCount { get; set; }
    public int AttendedCount { get; set; }
    
    // Registration
    public bool RequiresRegistration { get; set; } = true;
    public DateTime? RegistrationDeadline { get; set; }
    public decimal? RegistrationFee { get; set; }
    public bool IsFree { get; set; } = true;
    
    // Content
    public string? Agenda { get; set; }
    public string? Speakers { get; set; } // JSON array
    public string? Topics { get; set; } // JSON array
    
    // Media
    public string? BannerImageUrl { get; set; }
    public string? AttachmentUrls { get; set; } // JSON array
    
    // Engagement
    public int LikeCount { get; set; }
    public int ShareCount { get; set; }
    public int CommentCount { get; set; }
    
    // Metadata
    public string? Tags { get; set; } // JSON array
    public string? Notes { get; set; }
    
    // Navigation Properties
    public ICollection<EventRegistration> Registrations { get; set; } = new List<EventRegistration>();
}

public enum EventType
{
    Workshop,
    Seminar,
    Networking,
    Training,
    Conference,
    Webinar,
    MeetAndGreet,
    Other
}

public enum EventStatus
{
    Draft,
    Published,
    RegistrationOpen,
    RegistrationClosed,
    InProgress,
    Completed,
    Cancelled
}

