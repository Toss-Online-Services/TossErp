using MediatR;
using Crm.Application.DTOs;
using Crm.Application.Queries;
using TossErp.CRM.Domain.Repositories;

namespace Crm.Application.Handlers.Leads;

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
            Email = lead.Email.Value,
            Phone = lead.Phone?.Value,
            Source = lead.Source,
            Status = lead.Status,
            AssignedTo = lead.AssignedTo,
            Score = lead.Score.Value,
            IsHot = lead.IsHot,
            IsStale = lead.IsStale,
            CampaignName = lead.CampaignName,
            CreatedAt = lead.CreatedAt,
            UpdatedAt = lead.UpdatedAt,
            Notes = lead.Notes.Select(n => new LeadNoteDto
            {
                Content = n.Content,
                CreatedBy = n.CreatedBy,
                CreatedAt = n.CreatedAt
            }).ToList(),
            Activities = lead.Activities.Select(a => new LeadActivityDto
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
/// Handler for getting all leads
/// </summary>
public class GetAllLeadsHandler : IRequestHandler<GetAllLeadsQuery, IEnumerable<LeadDto>>
{
    private readonly ILeadRepository _leadRepository;

    public GetAllLeadsHandler(ILeadRepository leadRepository)
    {
        _leadRepository = leadRepository;
    }

    public async Task<IEnumerable<LeadDto>> Handle(GetAllLeadsQuery request, CancellationToken cancellationToken)
    {
        var leads = await _leadRepository.GetAllAsync(cancellationToken);

        return leads.Select(lead => new LeadDto
        {
            Id = lead.Id,
            FirstName = lead.FirstName,
            LastName = lead.LastName,
            Company = lead.Company,
            Email = lead.Email.Value,
            Phone = lead.Phone?.Value,
            Source = lead.Source,
            Status = lead.Status,
            AssignedTo = lead.AssignedTo,
            Score = lead.Score.Value,
            IsHot = lead.IsHot,
            IsStale = lead.IsStale,
            CampaignName = lead.CampaignName,
            CreatedAt = lead.CreatedAt,
            UpdatedAt = lead.UpdatedAt
        });
    }
}

/// <summary>
/// Handler for getting leads by status
/// </summary>
public class GetLeadsByStatusHandler : IRequestHandler<GetLeadsByStatusQuery, IEnumerable<LeadDto>>
{
    private readonly ILeadRepository _leadRepository;

    public GetLeadsByStatusHandler(ILeadRepository leadRepository)
    {
        _leadRepository = leadRepository;
    }

    public async Task<IEnumerable<LeadDto>> Handle(GetLeadsByStatusQuery request, CancellationToken cancellationToken)
    {
        var leads = await _leadRepository.GetByStatusAsync(request.Status, cancellationToken);

        return leads.Select(lead => new LeadDto
        {
            Id = lead.Id,
            FirstName = lead.FirstName,
            LastName = lead.LastName,
            Company = lead.Company,
            Email = lead.Email.Value,
            Phone = lead.Phone?.Value,
            Source = lead.Source,
            Status = lead.Status,
            AssignedTo = lead.AssignedTo,
            Score = lead.Score.Value,
            IsHot = lead.IsHot,
            IsStale = lead.IsStale,
            CampaignName = lead.CampaignName,
            CreatedAt = lead.CreatedAt,
            UpdatedAt = lead.UpdatedAt
        });
    }
}

/// <summary>
/// Handler for getting hot leads
/// </summary>
public class GetHotLeadsHandler : IRequestHandler<GetHotLeadsQuery, IEnumerable<LeadDto>>
{
    private readonly ILeadRepository _leadRepository;

    public GetHotLeadsHandler(ILeadRepository leadRepository)
    {
        _leadRepository = leadRepository;
    }

    public async Task<IEnumerable<LeadDto>> Handle(GetHotLeadsQuery request, CancellationToken cancellationToken)
    {
        var leads = await _leadRepository.GetHotLeadsAsync(cancellationToken);

        return leads.Select(lead => new LeadDto
        {
            Id = lead.Id,
            FirstName = lead.FirstName,
            LastName = lead.LastName,
            Company = lead.Company,
            Email = lead.Email.Value,
            Phone = lead.Phone?.Value,
            Source = lead.Source,
            Status = lead.Status,
            AssignedTo = lead.AssignedTo,
            Score = lead.Score.Value,
            IsHot = lead.IsHot,
            IsStale = lead.IsStale,
            CampaignName = lead.CampaignName,
            CreatedAt = lead.CreatedAt,
            UpdatedAt = lead.UpdatedAt
        });
    }
}

/// <summary>
/// Handler for getting stale leads
/// </summary>
public class GetStaleLeadsHandler : IRequestHandler<GetStaleLeadsQuery, IEnumerable<LeadDto>>
{
    private readonly ILeadRepository _leadRepository;

    public GetStaleLeadsHandler(ILeadRepository leadRepository)
    {
        _leadRepository = leadRepository;
    }

    public async Task<IEnumerable<LeadDto>> Handle(GetStaleLeadsQuery request, CancellationToken cancellationToken)
    {
        var leads = await _leadRepository.GetStaleLeadsAsync(request.DaysThreshold, cancellationToken);

        return leads.Select(lead => new LeadDto
        {
            Id = lead.Id,
            FirstName = lead.FirstName,
            LastName = lead.LastName,
            Company = lead.Company,
            Email = lead.Email.Value,
            Phone = lead.Phone?.Value,
            Source = lead.Source,
            Status = lead.Status,
            AssignedTo = lead.AssignedTo,
            Score = lead.Score.Value,
            IsHot = lead.IsHot,
            IsStale = lead.IsStale,
            CampaignName = lead.CampaignName,
            CreatedAt = lead.CreatedAt,
            UpdatedAt = lead.UpdatedAt
        });
    }
}

/// <summary>
/// Handler for getting leads by campaign
/// </summary>
public class GetLeadsByCampaignHandler : IRequestHandler<GetLeadsByCampaignQuery, IEnumerable<LeadDto>>
{
    private readonly ILeadRepository _leadRepository;

    public GetLeadsByCampaignHandler(ILeadRepository leadRepository)
    {
        _leadRepository = leadRepository;
    }

    public async Task<IEnumerable<LeadDto>> Handle(GetLeadsByCampaignQuery request, CancellationToken cancellationToken)
    {
        var leads = await _leadRepository.GetByCampaignAsync(request.CampaignName, cancellationToken);

        return leads.Select(lead => new LeadDto
        {
            Id = lead.Id,
            FirstName = lead.FirstName,
            LastName = lead.LastName,
            Company = lead.Company,
            Email = lead.Email.Value,
            Phone = lead.Phone?.Value,
            Source = lead.Source,
            Status = lead.Status,
            AssignedTo = lead.AssignedTo,
            Score = lead.Score.Value,
            IsHot = lead.IsHot,
            IsStale = lead.IsStale,
            CampaignName = lead.CampaignName,
            CreatedAt = lead.CreatedAt,
            UpdatedAt = lead.UpdatedAt
        });
    }
}

/// <summary>
/// Handler for searching leads
/// </summary>
public class SearchLeadsHandler : IRequestHandler<SearchLeadsQuery, IEnumerable<LeadDto>>
{
    private readonly ILeadRepository _leadRepository;

    public SearchLeadsHandler(ILeadRepository leadRepository)
    {
        _leadRepository = leadRepository;
    }

    public async Task<IEnumerable<LeadDto>> Handle(SearchLeadsQuery request, CancellationToken cancellationToken)
    {
        var leads = await _leadRepository.SearchAsync(request.SearchTerm, cancellationToken);

        return leads.Select(lead => new LeadDto
        {
            Id = lead.Id,
            FirstName = lead.FirstName,
            LastName = lead.LastName,
            Company = lead.Company,
            Email = lead.Email.Value,
            Phone = lead.Phone?.Value,
            Source = lead.Source,
            Status = lead.Status,
            AssignedTo = lead.AssignedTo,
            Score = lead.Score.Value,
            IsHot = lead.IsHot,
            IsStale = lead.IsStale,
            CampaignName = lead.CampaignName,
            CreatedAt = lead.CreatedAt,
            UpdatedAt = lead.UpdatedAt
        });
    }
}
