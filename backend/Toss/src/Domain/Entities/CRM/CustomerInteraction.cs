namespace Toss.Domain.Entities.CRM;

public class CustomerInteraction : BaseAuditableEntity
{
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    
    public string InteractionType { get; set; } = string.Empty; // "Call", "WhatsApp", "Visit", "Complaint", "Inquiry"
    public string? Subject { get; set; }
    public string? Description { get; set; }
    
    public DateTimeOffset InteractionDate { get; set; }
    public string? InteractionBy { get; set; }
    
    public bool RequiresFollowUp { get; set; }
    public DateTimeOffset? FollowUpDate { get; set; }
    public bool IsFollowedUp { get; set; }
}

