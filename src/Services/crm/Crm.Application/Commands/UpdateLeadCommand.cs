using MediatR;
using TossErp.CRM.Domain.Enums;

namespace Crm.Application.Commands;

/// <summary>
/// Command to update an existing lead
/// </summary>
public record UpdateLeadCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string Company,
    string Email,
    string? Phone = null,
    string? JobTitle = null,
    string? Industry = null,
    int? CompanySize = null,
    LeadSource? Source = null,
    string? Address = null,
    decimal? EstimatedValue = null
) : IRequest;
