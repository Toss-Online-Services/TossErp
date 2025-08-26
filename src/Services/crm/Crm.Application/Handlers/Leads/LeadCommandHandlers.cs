using MediatR;
using Crm.Application.Commands;
using Crm.Application.DTOs;
using TossErp.CRM.Domain.Aggregates;
using TossErp.CRM.Domain.Enums;
using TossErp.CRM.Domain.Repositories;
using TossErp.CRM.Domain.ValueObjects;

namespace Crm.Application.Handlers.Leads;

/// <summary>
/// Handler for creating new leads
/// </summary>
public class CreateLeadHandler : IRequestHandler<CreateLeadCommand, LeadDto>
{
    private readonly ILeadRepository _leadRepository;

    public CreateLeadHandler(ILeadRepository leadRepository)
    {
        _leadRepository = leadRepository;
    }

    public async Task<LeadDto> Handle(CreateLeadCommand request, CancellationToken cancellationToken)
    {
        // Check if lead with this email already exists
        if (await _leadRepository.ExistsByEmailAsync(request.Email, cancellationToken))
        {
            throw new InvalidOperationException($"Lead with email {request.Email} already exists");
        }

        // Create lead aggregate
        var lead = Lead.Create(
            firstName: request.FirstName,
            lastName: request.LastName,
            company: request.Company,
            email: Email.Create(request.Email),
            phone: !string.IsNullOrEmpty(request.Phone) ? PhoneNumber.Create(request.Phone) : null,
            source: request.Source,
            assignedTo: request.AssignedTo,
            campaignName: request.CampaignName
        );

        // Set initial score if provided
        if (request.InitialScore.HasValue)
        {
            lead.UpdateScore(LeadScore.Create(request.InitialScore.Value));
        }

        // Add notes if provided
        if (!string.IsNullOrEmpty(request.Notes))
        {
            lead.AddNote(request.Notes, "System");
        }

        // Save lead
        await _leadRepository.AddAsync(lead, cancellationToken);

        // Return DTO
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
/// Handler for updating leads
/// </summary>
public class UpdateLeadHandler : IRequestHandler<UpdateLeadCommand, LeadDto>
{
    private readonly ILeadRepository _leadRepository;

    public UpdateLeadHandler(ILeadRepository leadRepository)
    {
        _leadRepository = leadRepository;
    }

    public async Task<LeadDto> Handle(UpdateLeadCommand request, CancellationToken cancellationToken)
    {
        var lead = await _leadRepository.GetByIdAsync(request.Id, cancellationToken);
        if (lead == null)
        {
            throw new KeyNotFoundException($"Lead with ID {request.Id} not found");
        }

        // Update basic information
        if (!string.IsNullOrEmpty(request.FirstName))
            lead.UpdateFirstName(request.FirstName);

        if (!string.IsNullOrEmpty(request.LastName))
            lead.UpdateLastName(request.LastName);

        if (!string.IsNullOrEmpty(request.Company))
            lead.UpdateCompany(request.Company);

        if (!string.IsNullOrEmpty(request.Phone))
            lead.UpdatePhone(PhoneNumber.Create(request.Phone));

        if (!string.IsNullOrEmpty(request.AssignedTo))
            lead.AssignTo(request.AssignedTo);

        if (!string.IsNullOrEmpty(request.CampaignName))
            lead.UpdateCampaign(request.CampaignName);

        // Update score if provided
        if (request.Score.HasValue)
            lead.UpdateScore(LeadScore.Create(request.Score.Value));

        // Save changes
        await _leadRepository.UpdateAsync(lead, cancellationToken);

        // Return updated DTO
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
/// Handler for updating lead status
/// </summary>
public class UpdateLeadStatusHandler : IRequestHandler<UpdateLeadStatusCommand, LeadDto>
{
    private readonly ILeadRepository _leadRepository;

    public UpdateLeadStatusHandler(ILeadRepository leadRepository)
    {
        _leadRepository = leadRepository;
    }

    public async Task<LeadDto> Handle(UpdateLeadStatusCommand request, CancellationToken cancellationToken)
    {
        var lead = await _leadRepository.GetByIdAsync(request.Id, cancellationToken);
        if (lead == null)
        {
            throw new KeyNotFoundException($"Lead with ID {request.Id} not found");
        }

        // Update status using domain method
        switch (request.Status)
        {
            case LeadStatus.Qualified:
                lead.Qualify(!string.IsNullOrEmpty(request.Reason) ? request.Reason : "Lead qualified");
                break;
            case LeadStatus.Disqualified:
                lead.Disqualify(!string.IsNullOrEmpty(request.Reason) ? request.Reason : "Lead disqualified");
                break;
            case LeadStatus.Converted:
                // This should be handled by ConvertLeadCommand
                throw new InvalidOperationException("Use ConvertLeadCommand to convert leads");
            default:
                lead.UpdateStatus(request.Status);
                break;
        }

        await _leadRepository.UpdateAsync(lead, cancellationToken);

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
            UpdatedAt = lead.UpdatedAt
        };
    }
}

/// <summary>
/// Handler for converting leads to customers/opportunities
/// </summary>
public class ConvertLeadHandler : IRequestHandler<ConvertLeadCommand, ConvertLeadResult>
{
    private readonly ILeadRepository _leadRepository;

    public ConvertLeadHandler(ILeadRepository leadRepository)
    {
        _leadRepository = leadRepository;
    }

    public async Task<ConvertLeadResult> Handle(ConvertLeadCommand request, CancellationToken cancellationToken)
    {
        var lead = await _leadRepository.GetByIdAsync(request.LeadId, cancellationToken);
        if (lead == null)
        {
            throw new KeyNotFoundException($"Lead with ID {request.LeadId} not found");
        }

        if (lead.Status == LeadStatus.Converted)
        {
            throw new InvalidOperationException("Lead has already been converted");
        }

        // Convert the lead
        lead.Convert(request.CustomerId, request.OpportunityId);

        await _leadRepository.UpdateAsync(lead, cancellationToken);

        return new ConvertLeadResult
        {
            LeadId = lead.Id,
            CustomerId = request.CustomerId,
            OpportunityId = request.OpportunityId,
            ConvertedAt = DateTime.UtcNow
        };
    }
}

/// <summary>
/// Handler for adding activities to leads
/// </summary>
public class AddLeadActivityHandler : IRequestHandler<AddLeadActivityCommand, LeadDto>
{
    private readonly ILeadRepository _leadRepository;

    public AddLeadActivityHandler(ILeadRepository leadRepository)
    {
        _leadRepository = leadRepository;
    }

    public async Task<LeadDto> Handle(AddLeadActivityCommand request, CancellationToken cancellationToken)
    {
        var lead = await _leadRepository.GetByIdAsync(request.LeadId, cancellationToken);
        if (lead == null)
        {
            throw new KeyNotFoundException($"Lead with ID {request.LeadId} not found");
        }

        lead.AddActivity(
            type: request.Type,
            subject: request.Subject,
            description: request.Description,
            scheduledDate: request.ScheduledDate,
            createdBy: request.CreatedBy
        );

        await _leadRepository.UpdateAsync(lead, cancellationToken);

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
/// Handler for deleting leads
/// </summary>
public class DeleteLeadHandler : IRequestHandler<DeleteLeadCommand, bool>
{
    private readonly ILeadRepository _leadRepository;

    public DeleteLeadHandler(ILeadRepository leadRepository)
    {
        _leadRepository = leadRepository;
    }

    public async Task<bool> Handle(DeleteLeadCommand request, CancellationToken cancellationToken)
    {
        var lead = await _leadRepository.GetByIdAsync(request.Id, cancellationToken);
        if (lead == null)
        {
            return false;
        }

        await _leadRepository.DeleteAsync(lead, cancellationToken);
        return true;
    }
}
