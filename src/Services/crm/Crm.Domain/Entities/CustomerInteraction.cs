using Crm.Domain.Common;

namespace Crm.Domain.Entities;

public class CustomerInteraction : Entity
{
    public Guid CustomerId { get; private set; }
    public InteractionType Type { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public string Notes { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; } = string.Empty;
    public InteractionStatus Status { get; private set; }
    public DateTime? FollowUpDate { get; private set; }

    public CustomerInteraction(Guid customerId, InteractionType type, string description, string notes, string createdBy, DateTime? followUpDate = null)
    {
        Id = Guid.NewGuid();
        CustomerId = customerId;
        Type = type;
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Notes = notes ?? string.Empty;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy ?? throw new ArgumentNullException(nameof(createdBy));
        Status = InteractionStatus.Open;
        FollowUpDate = followUpDate;
    }

    private CustomerInteraction() { }

    public void UpdateStatus(InteractionStatus status)
    {
        Status = status;
    }

    public void SetFollowUpDate(DateTime followUpDate)
    {
        FollowUpDate = followUpDate;
    }

    public void UpdateNotes(string notes)
    {
        Notes = notes ?? string.Empty;
    }
}

public enum InteractionType
{
    PhoneCall,
    Email,
    InPerson,
    SocialMedia,
    SupportTicket,
    SalesInquiry,
    Complaint,
    Feedback,
    FollowUp
}

public enum InteractionStatus
{
    Open,
    InProgress,
    Resolved,
    Closed,
    Cancelled
}
