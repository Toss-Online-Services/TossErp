using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.Quality;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Quality.Commands.LogIncident;

public record LogIncidentCommand : IRequest<int>
{
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public IncidentType Type { get; init; }
    public IncidentSeverity Severity { get; init; }
    public DateTimeOffset OccurredAt { get; init; }
    public int? QualityChecklistId { get; init; }
    public int? ChecklistItemId { get; init; }
    public string? Notes { get; init; }
}

public class LogIncidentCommandHandler : IRequestHandler<LogIncidentCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public LogIncidentCommandHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<int> Handle(LogIncidentCommand request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ValidationException("Incident title is required.");
        }

        // Validate checklist exists if provided
        if (request.QualityChecklistId.HasValue)
        {
            var checklistExists = await _context.QualityChecklists
                .AnyAsync(c => c.Id == request.QualityChecklistId.Value
                    && c.BusinessId == _businessContext.CurrentBusinessId!.Value, cancellationToken);

            if (!checklistExists)
            {
                throw new NotFoundException("QualityChecklist", request.QualityChecklistId.Value.ToString());
            }
        }

        // Validate checklist item exists if provided
        if (request.ChecklistItemId.HasValue)
        {
            var itemExists = await _context.ChecklistItems
                .AnyAsync(i => i.Id == request.ChecklistItemId.Value
                    && i.BusinessId == _businessContext.CurrentBusinessId!.Value, cancellationToken);

            if (!itemExists)
            {
                throw new NotFoundException("ChecklistItem", request.ChecklistItemId.Value.ToString());
            }
        }

        var incident = new Incident
        {
            BusinessId = _businessContext.CurrentBusinessId!.Value,
            Title = request.Title,
            Description = request.Description,
            Type = request.Type,
            Severity = request.Severity,
            OccurredAt = request.OccurredAt,
            QualityChecklistId = request.QualityChecklistId,
            ChecklistItemId = request.ChecklistItemId,
            Notes = request.Notes
        };

        _context.Incidents.Add(incident);
        await _context.SaveChangesAsync(cancellationToken);

        return incident.Id;
    }
}

