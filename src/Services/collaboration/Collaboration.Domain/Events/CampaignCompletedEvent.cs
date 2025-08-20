using Collaboration.Domain.Common;

namespace Collaboration.Domain.Events;

/// <summary>
/// Domain event raised when a campaign is completed successfully
/// </summary>
public class CampaignCompletedEvent : IDomainEvent
{
    public Guid CampaignId { get; }
    public string CampaignName { get; }
    public int TotalParticipants { get; }
    public decimal TotalAmount { get; }
    public decimal DiscountPercentage { get; }
    public Guid TenantId { get; }
    public DateTime OccurredOn { get; }

    public CampaignCompletedEvent(
        Guid campaignId, 
        string campaignName, 
        int totalParticipants, 
        decimal totalAmount, 
        decimal discountPercentage, 
        Guid tenantId)
    {
        CampaignId = campaignId;
        CampaignName = campaignName;
        TotalParticipants = totalParticipants;
        TotalAmount = totalAmount;
        DiscountPercentage = discountPercentage;
        TenantId = tenantId;
        OccurredOn = DateTime.UtcNow;
    }
}
