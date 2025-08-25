using MediatR;
using TossErp.CRM.Domain.Enums;

namespace Crm.Application.Commands;

/// <summary>
/// Command to create a new opportunity
/// </summary>
public record CreateOpportunityCommand(
    string Name,
    Guid CustomerId,
    decimal EstimatedValue,
    DateTime ExpectedCloseDate,
    string? Description = null,
    OpportunityType Type = OpportunityType.NewBusiness,
    OpportunityPriority Priority = OpportunityPriority.Medium,
    LeadSource? Source = null,
    Guid? LeadId = null,
    string? CampaignName = null,
    decimal? Probability = null
) : IRequest<Guid>;
