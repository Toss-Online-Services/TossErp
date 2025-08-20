using Collaboration.Domain.Common;

namespace Collaboration.Domain.Events;

/// <summary>
/// Domain event raised when a campaign is launched
/// </summary>
public class CampaignLaunchedEvent : IDomainEvent
{
    public Guid CampaignId { get; }
    public string CampaignName { get; }
    public Guid CreatedBy { get; }
    public Guid TenantId { get; }
    public DateTime OccurredOn { get; }

    public CampaignLaunchedEvent(Guid campaignId, string campaignName, Guid createdBy, Guid tenantId)
    {
        CampaignId = campaignId;
        CampaignName = campaignName;
        CreatedBy = createdBy;
        TenantId = tenantId;
        OccurredOn = DateTime.UtcNow;
    }
}
