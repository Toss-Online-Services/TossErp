using MediatR;
using TossErp.CRM.Domain.Enums;

namespace Crm.Application.Commands;

/// <summary>
/// Command to change lead status (qualify, disqualify, etc.)
/// </summary>
public record ChangeLeadStatusCommand(
    Guid Id,
    LeadStatus Status,
    string? Reason = null
) : IRequest;
