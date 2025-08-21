using MediatR;
using Collaboration.Domain.Enums;
using Collaboration.Application.DTOs;

namespace Collaboration.Application.Commands;

/// <summary>
/// Command to create a new campaign
/// </summary>
public record CreateCampaignCommand(
    string Name,
    string Description,
    CampaignType Type,
    DateTime StartDate,
    DateTime EndDate,
    int MinParticipants,
    int MaxParticipants,
    decimal TargetAmount,
    decimal DiscountPercentage,
    Guid CreatedBy,
    Guid TenantId
) : IRequest<CampaignDto>;
