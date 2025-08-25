using MediatR;
using TossErp.CRM.Domain.Enums;

namespace Crm.Application.Commands;

/// <summary>
/// Command to create a new lead in the CRM system
/// </summary>
public record CreateLeadCommand(
    string FirstName,
    string LastName,
    string Company,
    string Email,
    string? Phone = null,
    string? JobTitle = null,
    string? Industry = null,
    int? CompanySize = null,
    LeadSource Source = LeadSource.Website,
    string? Address = null,
    decimal? EstimatedValue = null,
    string? CampaignName = null,
    string? Notes = null
) : IRequest<Guid>;
