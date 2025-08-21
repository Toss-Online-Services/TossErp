using MediatR;
using Collaboration.Application.DTOs;
using Collaboration.Domain.Enums;

namespace Collaboration.Application.Queries;

/// <summary>
/// Query to get campaigns with filtering
/// </summary>
public record GetCampaignsQuery(
    CampaignStatus? Status = null,
    CampaignType? Type = null,
    int Page = 1,
    int PageSize = 10
) : IRequest<IEnumerable<CampaignDto>>;

/// <summary>
/// Query to get a specific campaign by ID
/// </summary>
public record GetCampaignByIdQuery(Guid CampaignId) : IRequest<CampaignDetailsDto?>;

/// <summary>
/// Query to get campaign participants
/// </summary>
public record GetCampaignParticipantsQuery(Guid CampaignId) : IRequest<IEnumerable<CampaignParticipantDto>>;

/// <summary>
/// Query to get campaign analytics
/// </summary>
public record GetCampaignAnalyticsQuery(Guid CampaignId) : IRequest<CampaignAnalyticsDto>;
