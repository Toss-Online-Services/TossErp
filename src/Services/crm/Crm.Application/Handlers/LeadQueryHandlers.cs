using MediatR;
using Crm.Application.DTOs;
using Crm.Application.Queries;
using TossErp.CRM.Domain.Repositories;

namespace Crm.Application.Handlers;

/// <summary>
/// Handler for getting lead by ID
/// </summary>
public class GetLeadByIdHandler : IRequestHandler<GetLeadByIdQuery, LeadDto?>
{
    private readonly ILeadRepository _leadRepository;

    public GetLeadByIdHandler(ILeadRepository leadRepository)
    {
        _leadRepository = leadRepository;
    }

    public async Task<LeadDto?> Handle(GetLeadByIdQuery request, CancellationToken cancellationToken)
    {
        var lead = await _leadRepository.GetByIdAsync(request.Id, cancellationToken);
        if (lead == null)
            return null;

        return new LeadDto
        {
            Id = lead.Id,
            FirstName = lead.FirstName,
            LastName = lead.LastName,
            Company = lead.Company,
            JobTitle = lead.JobTitle,
            Email = lead.Email.Value,
            Phone = lead.Phone?.Value,
            Source = lead.Source,
            Status = lead.Status,
            Score = lead.Score.Value,
            AssignedTo = lead.AssignedTo,
            CampaignName = lead.CampaignName,
            CreatedAt = lead.CreatedAt,
            LastContactedAt = lead.LastContactedAt,
            QualifiedAt = lead.QualifiedAt,
            IsQualified = lead.Status == TossErp.CRM.Domain.Enums.LeadStatus.Qualified,
            IsConverted = lead.Status == TossErp.CRM.Domain.Enums.LeadStatus.Converted,
            ConvertedAt = lead.ConvertedAt,
            ConvertedCustomerId = lead.ConvertedCustomerId,
            ConvertedOpportunityId = lead.ConvertedOpportunityId,
            Industry = lead.Industry,
            CompanySize = lead.CompanySize,
            EstimatedValue = lead.EstimatedValue?.Amount ?? 0
        };
    }
}

/// <summary>
/// Handler for getting leads with filtering
/// </summary>
public class GetLeadsHandler : IRequestHandler<GetLeadsQuery, PagedResult<LeadSummaryDto>>
{
    private readonly ILeadRepository _leadRepository;

    public GetLeadsHandler(ILeadRepository leadRepository)
    {
        _leadRepository = leadRepository;
    }

    public async Task<PagedResult<LeadSummaryDto>> Handle(GetLeadsQuery request, CancellationToken cancellationToken)
    {
        var allLeads = await _leadRepository.GetAllAsync(cancellationToken);

        // Apply filters
        var filteredLeads = allLeads.AsEnumerable();

        if (request.Status.HasValue)
        {
            filteredLeads = filteredLeads.Where(l => l.Status == request.Status.Value);
        }

        if (request.Source.HasValue)
        {
            filteredLeads = filteredLeads.Where(l => l.Source == request.Source.Value);
        }

        if (!string.IsNullOrEmpty(request.AssignedTo))
        {
            filteredLeads = filteredLeads.Where(l => l.AssignedTo == request.AssignedTo);
        }

        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLowerInvariant();
            filteredLeads = filteredLeads.Where(l => 
                l.FirstName.ToLowerInvariant().Contains(searchTerm) ||
                l.LastName.ToLowerInvariant().Contains(searchTerm) ||
                l.Company.ToLowerInvariant().Contains(searchTerm) ||
                l.Email.Value.ToLowerInvariant().Contains(searchTerm));
        }

        var totalCount = filteredLeads.Count();
        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        var pagedLeads = filteredLeads
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(lead => new LeadSummaryDto
            {
                Id = lead.Id,
                FullName = $"{lead.FirstName} {lead.LastName}",
                Company = lead.Company,
                Email = lead.Email.Value,
                Phone = lead.Phone?.Value,
                Status = lead.Status,
                Source = lead.Source,
                Score = lead.Score.Value,
                CreatedAt = lead.CreatedAt,
                LastContactedAt = lead.LastContactedAt
            })
            .ToList();

        return new PagedResult<LeadSummaryDto>(
            pagedLeads,
            totalCount,
            request.Page,
            request.PageSize,
            totalPages
        );
    }
}

/// <summary>
/// Handler for getting hot leads
/// </summary>
public class GetHotLeadsHandler : IRequestHandler<GetHotLeadsQuery, PagedResult<LeadSummaryDto>>
{
    private readonly ILeadRepository _leadRepository;

    public GetHotLeadsHandler(ILeadRepository leadRepository)
    {
        _leadRepository = leadRepository;
    }

    public async Task<PagedResult<LeadSummaryDto>> Handle(GetHotLeadsQuery request, CancellationToken cancellationToken)
    {
        var hotLeads = await _leadRepository.GetHotLeadsAsync(cancellationToken);

        var totalCount = hotLeads.Count();
        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        var pagedLeads = hotLeads
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(lead => new LeadSummaryDto
            {
                Id = lead.Id,
                FullName = $"{lead.FirstName} {lead.LastName}",
                Company = lead.Company,
                Email = lead.Email.Value,
                Phone = lead.Phone?.Value,
                Status = lead.Status,
                Source = lead.Source,
                Score = lead.Score.Value,
                CreatedAt = lead.CreatedAt,
                LastContactedAt = lead.LastContactedAt
            })
            .ToList();

        return new PagedResult<LeadSummaryDto>(
            pagedLeads,
            totalCount,
            request.Page,
            request.PageSize,
            totalPages
        );
    }
}
