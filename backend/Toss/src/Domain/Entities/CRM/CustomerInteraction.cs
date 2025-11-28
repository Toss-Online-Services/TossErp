using Toss.Domain.Common;
using Toss.Domain.Entities.Businesses;

namespace Toss.Domain.Entities.CRM;

public class CustomerInteraction : BaseAuditableEntity, IBusinessScopedEntity
{
    public int BusinessId { get; set; }
    public Business Business { get; set; } = null!;
    
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

