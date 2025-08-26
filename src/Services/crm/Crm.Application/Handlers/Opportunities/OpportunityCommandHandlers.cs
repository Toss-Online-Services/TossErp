using MediatR;
using Crm.Application.Commands;
using Crm.Application.DTOs;
using TossErp.CRM.Domain.Aggregates;
using TossErp.CRM.Domain.Enums;
using TossErp.CRM.Domain.Repositories;

namespace Crm.Application.Handlers.Opportunities;

/// <summary>
/// Handler for creating new opportunities
/// </summary>
public class CreateOpportunityHandler : IRequestHandler<CreateOpportunityCommand, OpportunityDto>
{
    private readonly IOpportunityRepository _opportunityRepository;

    public CreateOpportunityHandler(IOpportunityRepository opportunityRepository)
    {
        _opportunityRepository = opportunityRepository;
    }

    public async Task<OpportunityDto> Handle(CreateOpportunityCommand request, CancellationToken cancellationToken)
    {
        // Create opportunity aggregate
        var opportunity = Opportunity.Create(
            title: request.Title,
            customerId: request.CustomerId,
            value: request.Value,
            expectedCloseDate: request.ExpectedCloseDate,
            assignedTo: request.AssignedTo,
            sourceLeadId: request.SourceLeadId,
            description: request.Description
        );

        // Save opportunity
        await _opportunityRepository.AddAsync(opportunity, cancellationToken);

        // Return DTO
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
/// Handler for updating opportunities
/// </summary>
public class UpdateOpportunityHandler : IRequestHandler<UpdateOpportunityCommand, OpportunityDto>
{
    private readonly IOpportunityRepository _opportunityRepository;

    public UpdateOpportunityHandler(IOpportunityRepository opportunityRepository)
    {
        _opportunityRepository = opportunityRepository;
    }

    public async Task<OpportunityDto> Handle(UpdateOpportunityCommand request, CancellationToken cancellationToken)
    {
        var opportunity = await _opportunityRepository.GetByIdAsync(request.Id, cancellationToken);
        if (opportunity == null)
        {
            throw new KeyNotFoundException($"Opportunity with ID {request.Id} not found");
        }

        // Update basic information
        if (!string.IsNullOrEmpty(request.Title))
            opportunity.UpdateTitle(request.Title);

        if (!string.IsNullOrEmpty(request.Description))
            opportunity.UpdateDescription(request.Description);

        if (request.Value.HasValue)
            opportunity.UpdateValue(request.Value.Value);

        if (request.ExpectedCloseDate.HasValue)
            opportunity.UpdateExpectedCloseDate(request.ExpectedCloseDate.Value);

        if (!string.IsNullOrEmpty(request.AssignedTo))
            opportunity.AssignTo(request.AssignedTo);

        // Save changes
        await _opportunityRepository.UpdateAsync(opportunity, cancellationToken);

        // Return updated DTO
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
/// Handler for advancing opportunity stage
/// </summary>
public class AdvanceOpportunityStageHandler : IRequestHandler<AdvanceOpportunityStageCommand, OpportunityDto>
{
    private readonly IOpportunityRepository _opportunityRepository;

    public AdvanceOpportunityStageHandler(IOpportunityRepository opportunityRepository)
    {
        _opportunityRepository = opportunityRepository;
    }

    public async Task<OpportunityDto> Handle(AdvanceOpportunityStageCommand request, CancellationToken cancellationToken)
    {
        var opportunity = await _opportunityRepository.GetByIdAsync(request.Id, cancellationToken);
        if (opportunity == null)
        {
            throw new KeyNotFoundException($"Opportunity with ID {request.Id} not found");
        }

        // Advance to next stage
        opportunity.AdvanceStage();

        await _opportunityRepository.UpdateAsync(opportunity, cancellationToken);

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
            UpdatedAt = opportunity.UpdatedAt
        };
    }
}

/// <summary>
/// Handler for closing opportunities as won
/// </summary>
public class CloseOpportunityAsWonHandler : IRequestHandler<CloseOpportunityAsWonCommand, OpportunityDto>
{
    private readonly IOpportunityRepository _opportunityRepository;

    public CloseOpportunityAsWonHandler(IOpportunityRepository opportunityRepository)
    {
        _opportunityRepository = opportunityRepository;
    }

    public async Task<OpportunityDto> Handle(CloseOpportunityAsWonCommand request, CancellationToken cancellationToken)
    {
        var opportunity = await _opportunityRepository.GetByIdAsync(request.Id, cancellationToken);
        if (opportunity == null)
        {
            throw new KeyNotFoundException($"Opportunity with ID {request.Id} not found");
        }

        opportunity.CloseAsWon(request.Reason);

        await _opportunityRepository.UpdateAsync(opportunity, cancellationToken);

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
            UpdatedAt = opportunity.UpdatedAt
        };
    }
}

/// <summary>
/// Handler for closing opportunities as lost
/// </summary>
public class CloseOpportunityAsLostHandler : IRequestHandler<CloseOpportunityAsLostCommand, OpportunityDto>
{
    private readonly IOpportunityRepository _opportunityRepository;

    public CloseOpportunityAsLostHandler(IOpportunityRepository opportunityRepository)
    {
        _opportunityRepository = opportunityRepository;
    }

    public async Task<OpportunityDto> Handle(CloseOpportunityAsLostCommand request, CancellationToken cancellationToken)
    {
        var opportunity = await _opportunityRepository.GetByIdAsync(request.Id, cancellationToken);
        if (opportunity == null)
        {
            throw new KeyNotFoundException($"Opportunity with ID {request.Id} not found");
        }

        opportunity.CloseAsLost(request.Reason);

        await _opportunityRepository.UpdateAsync(opportunity, cancellationToken);

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
            UpdatedAt = opportunity.UpdatedAt
        };
    }
}

/// <summary>
/// Handler for adding activities to opportunities
/// </summary>
public class AddOpportunityActivityHandler : IRequestHandler<AddOpportunityActivityCommand, OpportunityDto>
{
    private readonly IOpportunityRepository _opportunityRepository;

    public AddOpportunityActivityHandler(IOpportunityRepository opportunityRepository)
    {
        _opportunityRepository = opportunityRepository;
    }

    public async Task<OpportunityDto> Handle(AddOpportunityActivityCommand request, CancellationToken cancellationToken)
    {
        var opportunity = await _opportunityRepository.GetByIdAsync(request.OpportunityId, cancellationToken);
        if (opportunity == null)
        {
            throw new KeyNotFoundException($"Opportunity with ID {request.OpportunityId} not found");
        }

        opportunity.AddActivity(
            type: request.Type,
            subject: request.Subject,
            description: request.Description,
            scheduledDate: request.ScheduledDate,
            createdBy: request.CreatedBy
        );

        await _opportunityRepository.UpdateAsync(opportunity, cancellationToken);

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
/// Handler for deleting opportunities
/// </summary>
public class DeleteOpportunityHandler : IRequestHandler<DeleteOpportunityCommand, bool>
{
    private readonly IOpportunityRepository _opportunityRepository;

    public DeleteOpportunityHandler(IOpportunityRepository opportunityRepository)
    {
        _opportunityRepository = opportunityRepository;
    }

    public async Task<bool> Handle(DeleteOpportunityCommand request, CancellationToken cancellationToken)
    {
        var opportunity = await _opportunityRepository.GetByIdAsync(request.Id, cancellationToken);
        if (opportunity == null)
        {
            return false;
        }

        await _opportunityRepository.DeleteAsync(opportunity, cancellationToken);
        return true;
    }
}
