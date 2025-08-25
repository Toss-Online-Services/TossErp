using MediatR;
using Crm.Application.DTOs;
using TossErp.CRM.Domain.Enums;

namespace Crm.Application.Queries;

/// <summary>
/// Query to get all leads with optional filtering
/// </summary>
public record GetLeadsQuery(
    LeadStatus? Status = null,
    LeadSource? Source = null,
    string? AssignedTo = null,
    string? SearchTerm = null,
    int Page = 1,
    int PageSize = 50
) : IRequest<PagedResult<LeadSummaryDto>>;

/// <summary>
/// Query to get a specific lead by ID
/// </summary>
public record GetLeadByIdQuery(Guid Id) : IRequest<LeadDto?>;

/// <summary>
/// Query to get leads by score range
/// </summary>
public record GetLeadsByScoreQuery(
    int MinScore = 0,
    int MaxScore = 100,
    int Page = 1,
    int PageSize = 20
) : IRequest<PagedResult<LeadSummaryDto>>;

/// <summary>
/// Query to get hot leads (high score)
/// </summary>
public record GetHotLeadsQuery(
    int Page = 1,
    int PageSize = 20
) : IRequest<PagedResult<LeadSummaryDto>>;

/// <summary>
/// Query to get stale leads (not contacted recently)
/// </summary>
public record GetStaleLeadsQuery(
    int DaysThreshold = 30,
    int Page = 1,
    int PageSize = 20
) : IRequest<PagedResult<LeadSummaryDto>>;

/// <summary>
/// Query to get leads by campaign
/// </summary>
public record GetLeadsByCampaignQuery(
    string CampaignName,
    int Page = 1,
    int PageSize = 50
) : IRequest<PagedResult<LeadSummaryDto>>;

/// <summary>
/// Generic paged result wrapper
/// </summary>
public record PagedResult<T>(
    IEnumerable<T> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
);
