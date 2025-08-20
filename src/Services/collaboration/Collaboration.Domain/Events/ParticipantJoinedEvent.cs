using Collaboration.Domain.Common;

namespace Collaboration.Domain.Events;

/// <summary>
/// Domain event raised when a participant joins a campaign
/// </summary>
public class ParticipantJoinedEvent : IDomainEvent
{
    public Guid CampaignId { get; }
    public Guid ParticipantId { get; }
    public Guid UserId { get; }
    public string UserName { get; }
    public decimal CommittedAmount { get; }
    public Guid TenantId { get; }
    public DateTime OccurredOn { get; }

    public ParticipantJoinedEvent(
        Guid campaignId, 
        Guid participantId, 
        Guid userId, 
        string userName, 
        decimal committedAmount, 
        Guid tenantId)
    {
        CampaignId = campaignId;
        ParticipantId = participantId;
        UserId = userId;
        UserName = userName;
        CommittedAmount = committedAmount;
        TenantId = tenantId;
        OccurredOn = DateTime.UtcNow;
    }
}
