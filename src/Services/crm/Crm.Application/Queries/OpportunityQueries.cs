using MediatR;
using Crm.Application.DTOs;
using TossErp.CRM.Domain.Enums;

namespace Crm.Application.Queries;

/// <summary>
/// Query to get all opportunities with optional filtering
/// </summary>
public record GetOpportunitiesQuery(
    OpportunityStage? Stage = null,
    OpportunityType? Type = null,
    OpportunityPriority? Priority = null,
    string? AssignedTo = null,
    string? SearchTerm = null,
    bool? IsOverdue = null,
    int Page = 1,
    int PageSize = 50
) : IRequest<PagedResult<OpportunitySummaryDto>>;

/// <summary>
/// Query to get a specific opportunity by ID
/// </summary>
public record GetOpportunityByIdQuery(Guid Id) : IRequest<OpportunityDto?>;

/// <summary>
/// Query to get opportunities by customer
/// </summary>
public record GetOpportunitiesByCustomerQuery(
    Guid CustomerId,
    int Page = 1,
    int PageSize = 20
) : IRequest<PagedResult<OpportunitySummaryDto>>;

/// <summary>
/// Query to get opportunities closing soon
/// </summary>
public record GetOpportunitiesClosingSoonQuery(
    int DaysThreshold = 7,
    int Page = 1,
    int PageSize = 20
) : IRequest<PagedResult<OpportunitySummaryDto>>;

/// <summary>
/// Query to get pipeline analytics by stage
/// </summary>
public record GetOpportunityPipelineQuery() : IRequest<OpportunityPipelineDto>;

/// <summary>
/// Pipeline analytics DTO
/// </summary>
public class OpportunityPipelineDto
{
    public List<PipelineStageDto> Stages { get; set; } = new();
    public decimal TotalPipelineValue { get; set; }
    public decimal WeightedPipelineValue { get; set; }
    public int TotalOpportunities { get; set; }
    public decimal AverageOpportunityValue { get; set; }
    public decimal ConversionRate { get; set; }
}

/// <summary>
/// Pipeline stage information
/// </summary>
public class PipelineStageDto
{
    public OpportunityStage Stage { get; set; }
    public string StageName { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal TotalValue { get; set; }
    public decimal WeightedValue { get; set; }
    public decimal AverageValue { get; set; }
    public List<OpportunitySummaryDto> Opportunities { get; set; } = new();
}
