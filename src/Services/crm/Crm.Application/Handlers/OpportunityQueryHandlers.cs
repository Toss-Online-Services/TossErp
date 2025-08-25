using MediatR;
using Crm.Application.DTOs;
using Crm.Application.Queries;
using TossErp.CRM.Domain.Repositories;

namespace Crm.Application.Handlers;

/// <summary>
/// Handler for getting opportunity by ID
/// </summary>
public class GetOpportunityByIdHandler : IRequestHandler<GetOpportunityByIdQuery, OpportunityDto?>
{
    private readonly IOpportunityRepository _opportunityRepository;

    public GetOpportunityByIdHandler(IOpportunityRepository opportunityRepository)
    {
        _opportunityRepository = opportunityRepository;
    }

    public async Task<OpportunityDto?> Handle(GetOpportunityByIdQuery request, CancellationToken cancellationToken)
    {
        var opportunity = await _opportunityRepository.GetByIdAsync(request.Id, cancellationToken);
        if (opportunity == null)
            return null;

        return new OpportunityDto
        {
            Id = opportunity.Id,
            Name = opportunity.Name,
            Description = opportunity.Description,
            CustomerId = opportunity.CustomerId,
            EstimatedValue = opportunity.Value.EstimatedValue.Amount,
            Stage = opportunity.Stage,
            Probability = opportunity.Value.Probability,
            ExpectedCloseDate = opportunity.ExpectedCloseDate,
            ActualCloseDate = opportunity.ActualCloseDate,
            AssignedTo = opportunity.AssignedTo,
            CreatedAt = opportunity.CreatedAt,
            Type = TossErp.CRM.Domain.Enums.OpportunityType.NewBusiness, // Default for now
            Priority = TossErp.CRM.Domain.Enums.OpportunityPriority.Medium, // Default for now
            Source = opportunity.Source,
            LeadId = opportunity.LeadId,
            LastActivityDate = opportunity.LastActivityDate,
            ContactAttempts = opportunity.ContactAttempts,
            NextFollowUp = opportunity.NextFollowUp,
            WinReason = opportunity.WinReason,
            LossReason = opportunity.LossReason,
            CompetitorName = opportunity.CompetitorName,
            ActualValue = opportunity.ActualValue?.Amount,
            CampaignName = opportunity.CampaignName
        };
    }
}

/// <summary>
/// Handler for getting opportunities with filtering
/// </summary>
public class GetOpportunitiesHandler : IRequestHandler<GetOpportunitiesQuery, PagedResult<OpportunitySummaryDto>>
{
    private readonly IOpportunityRepository _opportunityRepository;

    public GetOpportunitiesHandler(IOpportunityRepository opportunityRepository)
    {
        _opportunityRepository = opportunityRepository;
    }

    public async Task<PagedResult<OpportunitySummaryDto>> Handle(GetOpportunitiesQuery request, CancellationToken cancellationToken)
    {
        var allOpportunities = await _opportunityRepository.GetAllAsync(cancellationToken);

        // Apply filters
        var filteredOpportunities = allOpportunities.AsEnumerable();

        if (request.Stage.HasValue)
        {
            filteredOpportunities = filteredOpportunities.Where(o => o.Stage == request.Stage.Value);
        }

        if (!string.IsNullOrEmpty(request.AssignedTo))
        {
            filteredOpportunities = filteredOpportunities.Where(o => o.AssignedTo == request.AssignedTo);
        }

        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLowerInvariant();
            filteredOpportunities = filteredOpportunities.Where(o => 
                o.Name.ToLowerInvariant().Contains(searchTerm) ||
                (o.Description?.ToLowerInvariant().Contains(searchTerm) == true));
        }

        if (request.IsOverdue.HasValue)
        {
            var today = DateTime.UtcNow.Date;
            if (request.IsOverdue.Value)
            {
                filteredOpportunities = filteredOpportunities.Where(o => 
                    o.ExpectedCloseDate.Date < today &&
                    o.Stage != TossErp.CRM.Domain.Enums.OpportunityStage.ClosedWon &&
                    o.Stage != TossErp.CRM.Domain.Enums.OpportunityStage.ClosedLost);
            }
            else
            {
                filteredOpportunities = filteredOpportunities.Where(o => 
                    o.ExpectedCloseDate.Date >= today ||
                    o.Stage == TossErp.CRM.Domain.Enums.OpportunityStage.ClosedWon ||
                    o.Stage == TossErp.CRM.Domain.Enums.OpportunityStage.ClosedLost);
            }
        }

        var totalCount = filteredOpportunities.Count();
        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        var pagedOpportunities = filteredOpportunities
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(opportunity => new OpportunitySummaryDto
            {
                Id = opportunity.Id,
                Name = opportunity.Name,
                CustomerName = $"Customer {opportunity.CustomerId}", // TODO: Load actual customer name
                Stage = opportunity.Stage,
                EstimatedValue = opportunity.Value.EstimatedValue.Amount,
                Probability = opportunity.Value.Probability,
                ExpectedCloseDate = opportunity.ExpectedCloseDate,
                CreatedAt = opportunity.CreatedAt
            })
            .ToList();

        return new PagedResult<OpportunitySummaryDto>(
            pagedOpportunities,
            totalCount,
            request.Page,
            request.PageSize,
            totalPages
        );
    }
}

/// <summary>
/// Handler for getting opportunities by customer
/// </summary>
public class GetOpportunitiesByCustomerHandler : IRequestHandler<GetOpportunitiesByCustomerQuery, PagedResult<OpportunitySummaryDto>>
{
    private readonly IOpportunityRepository _opportunityRepository;

    public GetOpportunitiesByCustomerHandler(IOpportunityRepository opportunityRepository)
    {
        _opportunityRepository = opportunityRepository;
    }

    public async Task<PagedResult<OpportunitySummaryDto>> Handle(GetOpportunitiesByCustomerQuery request, CancellationToken cancellationToken)
    {
        var opportunities = await _opportunityRepository.GetByCustomerAsync(request.CustomerId, cancellationToken);

        var totalCount = opportunities.Count();
        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        var pagedOpportunities = opportunities
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(opportunity => new OpportunitySummaryDto
            {
                Id = opportunity.Id,
                Name = opportunity.Name,
                CustomerName = $"Customer {opportunity.CustomerId}",
                Stage = opportunity.Stage,
                EstimatedValue = opportunity.Value.EstimatedValue.Amount,
                Probability = opportunity.Value.Probability,
                ExpectedCloseDate = opportunity.ExpectedCloseDate,
                CreatedAt = opportunity.CreatedAt
            })
            .ToList();

        return new PagedResult<OpportunitySummaryDto>(
            pagedOpportunities,
            totalCount,
            request.Page,
            request.PageSize,
            totalPages
        );
    }
}

/// <summary>
/// Handler for getting opportunity pipeline analytics
/// </summary>
public class GetOpportunityPipelineHandler : IRequestHandler<GetOpportunityPipelineQuery, OpportunityPipelineDto>
{
    private readonly IOpportunityRepository _opportunityRepository;

    public GetOpportunityPipelineHandler(IOpportunityRepository opportunityRepository)
    {
        _opportunityRepository = opportunityRepository;
    }

    public async Task<OpportunityPipelineDto> Handle(GetOpportunityPipelineQuery request, CancellationToken cancellationToken)
    {
        var allOpportunities = await _opportunityRepository.GetAllAsync(cancellationToken);
        var totalValue = await _opportunityRepository.GetTotalValueAsync(cancellationToken);

        // Group by stage
        var stageGroups = allOpportunities.GroupBy(o => o.Stage).ToList();

        var stages = stageGroups.Select(group => new PipelineStageDto
        {
            Stage = group.Key,
            StageName = group.Key.ToString(),
            Count = group.Count(),
            TotalValue = group.Sum(o => o.Value.EstimatedValue.Amount),
            WeightedValue = group.Sum(o => o.Value.WeightedValue.Amount),
            AverageValue = group.Average(o => o.Value.EstimatedValue.Amount),
            Opportunities = group.Select(o => new OpportunitySummaryDto
            {
                Id = o.Id,
                Name = o.Name,
                CustomerName = $"Customer {o.CustomerId}",
                Stage = o.Stage,
                EstimatedValue = o.Value.EstimatedValue.Amount,
                Probability = o.Value.Probability,
                ExpectedCloseDate = o.ExpectedCloseDate,
                CreatedAt = o.CreatedAt
            }).ToList()
        }).ToList();

        return new OpportunityPipelineDto
        {
            Stages = stages,
            TotalPipelineValue = totalValue,
            WeightedPipelineValue = stages.Sum(s => s.WeightedValue),
            TotalOpportunities = allOpportunities.Count(),
            AverageOpportunityValue = allOpportunities.Any() ? allOpportunities.Average(o => o.Value.EstimatedValue.Amount) : 0,
            ConversionRate = 0 // TODO: Calculate based on historical data
        };
    }
}
