using MediatR;
using Crm.Application.DTOs;
using Crm.Application.Queries;
using TossErp.CRM.Domain.Repositories;

namespace Crm.Application.Handlers.Opportunities;

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
            Title = opportunity.Title,
            Description = opportunity.Description,
            CustomerId = opportunity.CustomerId,
            Value = opportunity.Value,
            Stage = opportunity.Stage,
            Probability = opportunity.Probability,
            ExpectedCloseDate = opportunity.ExpectedCloseDate,
            ActualCloseDate = opportunity.ActualCloseDate,
            AssignedTo = opportunity.AssignedTo,
            SourceLeadId = opportunity.SourceLeadId,
            CloseReason = opportunity.CloseReason,
            CreatedAt = opportunity.CreatedAt,
            UpdatedAt = opportunity.UpdatedAt,
            Activities = opportunity.Activities.Select(a => new OpportunityActivityDto
            {
                Type = a.Type,
                Subject = a.Subject,
                Description = a.Description,
                ScheduledDate = a.ScheduledDate,
                CompletedDate = a.CompletedDate,
                Status = a.Status,
                CreatedBy = a.CreatedBy,
                CreatedAt = a.CreatedAt
            }).ToList()
        };
    }
}

/// <summary>
/// Handler for getting all opportunities
/// </summary>
public class GetAllOpportunitiesHandler : IRequestHandler<GetAllOpportunitiesQuery, IEnumerable<OpportunityDto>>
{
    private readonly IOpportunityRepository _opportunityRepository;

    public GetAllOpportunitiesHandler(IOpportunityRepository opportunityRepository)
    {
        _opportunityRepository = opportunityRepository;
    }

    public async Task<IEnumerable<OpportunityDto>> Handle(GetAllOpportunitiesQuery request, CancellationToken cancellationToken)
    {
        var opportunities = await _opportunityRepository.GetAllAsync(cancellationToken);

        return opportunities.Select(opportunity => new OpportunityDto
        {
            Id = opportunity.Id,
            Title = opportunity.Title,
            Description = opportunity.Description,
            CustomerId = opportunity.CustomerId,
            Value = opportunity.Value,
            Stage = opportunity.Stage,
            Probability = opportunity.Probability,
            ExpectedCloseDate = opportunity.ExpectedCloseDate,
            ActualCloseDate = opportunity.ActualCloseDate,
            AssignedTo = opportunity.AssignedTo,
            SourceLeadId = opportunity.SourceLeadId,
            CloseReason = opportunity.CloseReason,
            CreatedAt = opportunity.CreatedAt,
            UpdatedAt = opportunity.UpdatedAt
        });
    }
}

/// <summary>
/// Handler for getting opportunities by stage
/// </summary>
public class GetOpportunitiesByStageHandler : IRequestHandler<GetOpportunitiesByStageQuery, IEnumerable<OpportunityDto>>
{
    private readonly IOpportunityRepository _opportunityRepository;

    public GetOpportunitiesByStageHandler(IOpportunityRepository opportunityRepository)
    {
        _opportunityRepository = opportunityRepository;
    }

    public async Task<IEnumerable<OpportunityDto>> Handle(GetOpportunitiesByStageQuery request, CancellationToken cancellationToken)
    {
        var opportunities = await _opportunityRepository.GetByStageAsync(request.Stage, cancellationToken);

        return opportunities.Select(opportunity => new OpportunityDto
        {
            Id = opportunity.Id,
            Title = opportunity.Title,
            Description = opportunity.Description,
            CustomerId = opportunity.CustomerId,
            Value = opportunity.Value,
            Stage = opportunity.Stage,
            Probability = opportunity.Probability,
            ExpectedCloseDate = opportunity.ExpectedCloseDate,
            ActualCloseDate = opportunity.ActualCloseDate,
            AssignedTo = opportunity.AssignedTo,
            SourceLeadId = opportunity.SourceLeadId,
            CloseReason = opportunity.CloseReason,
            CreatedAt = opportunity.CreatedAt,
            UpdatedAt = opportunity.UpdatedAt
        });
    }
}

/// <summary>
/// Handler for getting opportunities closing soon
/// </summary>
public class GetOpportunitiesClosingSoonHandler : IRequestHandler<GetOpportunitiesClosingSoonQuery, IEnumerable<OpportunityDto>>
{
    private readonly IOpportunityRepository _opportunityRepository;

    public GetOpportunitiesClosingSoonHandler(IOpportunityRepository opportunityRepository)
    {
        _opportunityRepository = opportunityRepository;
    }

    public async Task<IEnumerable<OpportunityDto>> Handle(GetOpportunitiesClosingSoonQuery request, CancellationToken cancellationToken)
    {
        var opportunities = await _opportunityRepository.GetClosingSoonAsync(request.DaysThreshold, cancellationToken);

        return opportunities.Select(opportunity => new OpportunityDto
        {
            Id = opportunity.Id,
            Title = opportunity.Title,
            Description = opportunity.Description,
            CustomerId = opportunity.CustomerId,
            Value = opportunity.Value,
            Stage = opportunity.Stage,
            Probability = opportunity.Probability,
            ExpectedCloseDate = opportunity.ExpectedCloseDate,
            ActualCloseDate = opportunity.ActualCloseDate,
            AssignedTo = opportunity.AssignedTo,
            SourceLeadId = opportunity.SourceLeadId,
            CloseReason = opportunity.CloseReason,
            CreatedAt = opportunity.CreatedAt,
            UpdatedAt = opportunity.UpdatedAt
        });
    }
}

/// <summary>
/// Handler for getting opportunities by customer
/// </summary>
public class GetOpportunitiesByCustomerHandler : IRequestHandler<GetOpportunitiesByCustomerQuery, IEnumerable<OpportunityDto>>
{
    private readonly IOpportunityRepository _opportunityRepository;

    public GetOpportunitiesByCustomerHandler(IOpportunityRepository opportunityRepository)
    {
        _opportunityRepository = opportunityRepository;
    }

    public async Task<IEnumerable<OpportunityDto>> Handle(GetOpportunitiesByCustomerQuery request, CancellationToken cancellationToken)
    {
        var opportunities = await _opportunityRepository.GetByCustomerAsync(request.CustomerId, cancellationToken);

        return opportunities.Select(opportunity => new OpportunityDto
        {
            Id = opportunity.Id,
            Title = opportunity.Title,
            Description = opportunity.Description,
            CustomerId = opportunity.CustomerId,
            Value = opportunity.Value,
            Stage = opportunity.Stage,
            Probability = opportunity.Probability,
            ExpectedCloseDate = opportunity.ExpectedCloseDate,
            ActualCloseDate = opportunity.ActualCloseDate,
            AssignedTo = opportunity.AssignedTo,
            SourceLeadId = opportunity.SourceLeadId,
            CloseReason = opportunity.CloseReason,
            CreatedAt = opportunity.CreatedAt,
            UpdatedAt = opportunity.UpdatedAt
        });
    }
}

/// <summary>
/// Handler for searching opportunities
/// </summary>
public class SearchOpportunitiesHandler : IRequestHandler<SearchOpportunitiesQuery, IEnumerable<OpportunityDto>>
{
    private readonly IOpportunityRepository _opportunityRepository;

    public SearchOpportunitiesHandler(IOpportunityRepository opportunityRepository)
    {
        _opportunityRepository = opportunityRepository;
    }

    public async Task<IEnumerable<OpportunityDto>> Handle(SearchOpportunitiesQuery request, CancellationToken cancellationToken)
    {
        var opportunities = await _opportunityRepository.SearchAsync(request.SearchTerm, cancellationToken);

        return opportunities.Select(opportunity => new OpportunityDto
        {
            Id = opportunity.Id,
            Title = opportunity.Title,
            Description = opportunity.Description,
            CustomerId = opportunity.CustomerId,
            Value = opportunity.Value,
            Stage = opportunity.Stage,
            Probability = opportunity.Probability,
            ExpectedCloseDate = opportunity.ExpectedCloseDate,
            ActualCloseDate = opportunity.ActualCloseDate,
            AssignedTo = opportunity.AssignedTo,
            SourceLeadId = opportunity.SourceLeadId,
            CloseReason = opportunity.CloseReason,
            CreatedAt = opportunity.CreatedAt,
            UpdatedAt = opportunity.UpdatedAt
        });
    }
}

/// <summary>
/// Handler for getting pipeline summary
/// </summary>
public class GetPipelineSummaryHandler : IRequestHandler<GetPipelineSummaryQuery, PipelineSummaryDto>
{
    private readonly IOpportunityRepository _opportunityRepository;

    public GetPipelineSummaryHandler(IOpportunityRepository opportunityRepository)
    {
        _opportunityRepository = opportunityRepository;
    }

    public async Task<PipelineSummaryDto> Handle(GetPipelineSummaryQuery request, CancellationToken cancellationToken)
    {
        var allOpportunities = await _opportunityRepository.GetAllAsync(cancellationToken);
        var totalValue = await _opportunityRepository.GetTotalValueAsync(cancellationToken);

        // Group by stage
        var stageGroups = allOpportunities.GroupBy(o => o.Stage).ToList();

        var stageData = stageGroups.Select(group => new PipelineStageDto
        {
            Stage = group.Key,
            Count = group.Count(),
            Value = group.Sum(o => o.Value),
            AverageProbability = group.Average(o => o.Probability)
        }).ToList();

        return new PipelineSummaryDto
        {
            TotalOpportunities = allOpportunities.Count(),
            TotalValue = totalValue,
            StageData = stageData,
            GeneratedAt = DateTime.UtcNow
        };
    }
}
