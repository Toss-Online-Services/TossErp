using MediatR;

namespace Crm.Application.Commands;

/// <summary>
/// Command to convert a qualified lead to a customer
/// </summary>
public record ConvertLeadCommand(
    Guid LeadId,
    bool CreateOpportunity = true,
    string? OpportunityName = null,
    decimal? OpportunityValue = null,
    DateTime? ExpectedCloseDate = null
) : IRequest<ConvertLeadResult>;

/// <summary>
/// Result of lead conversion
/// </summary>
public record ConvertLeadResult(
    Guid CustomerId,
    Guid? OpportunityId = null
);
