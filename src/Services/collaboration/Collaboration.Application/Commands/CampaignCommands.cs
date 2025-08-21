using MediatR;

namespace Collaboration.Application.Commands;

/// <summary>
/// Command to update campaign details
/// </summary>
public record UpdateCampaignCommand(
    Guid CampaignId,
    string Name,
    string Description,
    DateTime StartDate,
    DateTime EndDate
) : IRequest;

/// <summary>
/// Command to launch a campaign
/// </summary>
public record LaunchCampaignCommand(Guid CampaignId) : IRequest;

/// <summary>
/// Command to join a campaign
/// </summary>
public record JoinCampaignCommand(
    Guid CampaignId,
    Guid UserId,
    decimal CommittedAmount,
    int DesiredQuantity
) : IRequest;

/// <summary>
/// Command to leave a campaign
/// </summary>
public record LeaveCampaignCommand(
    Guid CampaignId,
    Guid UserId
) : IRequest;

/// <summary>
/// Command to complete a campaign
/// </summary>
public record CompleteCampaignCommand(Guid CampaignId) : IRequest;

/// <summary>
/// Command to cancel a campaign
/// </summary>
public record CancelCampaignCommand(
    Guid CampaignId,
    string Reason,
    Guid CancelledBy
) : IRequest;
