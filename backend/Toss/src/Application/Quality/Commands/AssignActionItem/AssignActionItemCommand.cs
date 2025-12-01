using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.Quality;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Quality.Commands.AssignActionItem;

public record AssignActionItemCommand : IRequest<int>
{
    public int? IncidentId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? AssignedToId { get; init; }
    public DateTimeOffset? DueDate { get; init; }
    public string? Notes { get; init; }
}

public class AssignActionItemCommandHandler : IRequestHandler<AssignActionItemCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public AssignActionItemCommandHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<int> Handle(AssignActionItemCommand request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ValidationException("Action item title is required.");
        }

        // Validate incident exists if provided
        if (request.IncidentId.HasValue)
        {
            var incidentExists = await _context.Incidents
                .AnyAsync(i => i.Id == request.IncidentId.Value
                    && i.BusinessId == _businessContext.CurrentBusinessId!.Value, cancellationToken);

            if (!incidentExists)
            {
                throw new NotFoundException("Incident", request.IncidentId.Value.ToString());
            }
        }

        // Validate due date is in the future if provided
        if (request.DueDate.HasValue && request.DueDate.Value < DateTimeOffset.UtcNow)
        {
            throw new ValidationException("Due date cannot be in the past.");
        }

        var actionItem = new ActionItem
        {
            BusinessId = _businessContext.CurrentBusinessId!.Value,
            IncidentId = request.IncidentId,
            Title = request.Title,
            Description = request.Description,
            AssignedToId = request.AssignedToId,
            DueDate = request.DueDate,
            Status = ActionItemStatus.Open,
            Notes = request.Notes
        };

        _context.ActionItems.Add(actionItem);
        await _context.SaveChangesAsync(cancellationToken);

        return actionItem.Id;
    }
}

