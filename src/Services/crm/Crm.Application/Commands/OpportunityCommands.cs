using MediatR;
using TossErp.CRM.Domain.Enums;

namespace Crm.Application.Commands;

/// <summary>
/// Command to update an existing opportunity
/// </summary>
public record UpdateOpportunityCommand(
    Guid Id,
    string Name,
    decimal EstimatedValue,
    DateTime ExpectedCloseDate,
    string? Description = null,
    OpportunityType? Type = null,
    OpportunityPriority? Priority = null,
    decimal? Probability = null
) : IRequest;

/// <summary>
/// Command to advance opportunity stage
/// </summary>
public record AdvanceOpportunityStageCommand(
    Guid Id,
    OpportunityStage Stage,
    string? Reason = null
) : IRequest;

/// <summary>
/// Command to win an opportunity
/// </summary>
public record WinOpportunityCommand(
    Guid Id,
    decimal ActualValue,
    string? WinReason = null
) : IRequest;

/// <summary>
/// Command to lose an opportunity
/// </summary>
public record LoseOpportunityCommand(
    Guid Id,
    string LossReason,
    string? CompetitorName = null
) : IRequest;
