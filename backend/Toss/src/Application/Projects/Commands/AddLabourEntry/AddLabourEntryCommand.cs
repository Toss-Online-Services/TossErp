using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.Projects;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Projects.Commands.AddLabourEntry;

public record AddLabourEntryCommand : IRequest<int>
{
    public int ProjectId { get; init; }
    public string? UserId { get; init; }
    public DateTimeOffset WorkDate { get; init; }
    public decimal Hours { get; init; }
    public decimal Rate { get; init; }
    public string? Description { get; init; }
    public int? ProjectTaskId { get; init; }
}

public class AddLabourEntryCommandHandler : IRequestHandler<AddLabourEntryCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public AddLabourEntryCommandHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<int> Handle(AddLabourEntryCommand request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        if (request.Hours <= 0)
        {
            throw new ValidationException("Hours must be greater than zero.");
        }

        if (request.Rate < 0)
        {
            throw new ValidationException("Rate cannot be negative.");
        }

        // Validate project exists
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == request.ProjectId
                && p.BusinessId == _businessContext.CurrentBusinessId!.Value, cancellationToken);

        if (project == null)
        {
            throw new NotFoundException("Project", request.ProjectId.ToString());
        }

        // Validate project task exists if provided
        if (request.ProjectTaskId.HasValue)
        {
            var taskExists = await _context.ProjectTasks
                .AnyAsync(t => t.Id == request.ProjectTaskId.Value
                    && t.ProjectId == request.ProjectId
                    && t.BusinessId == _businessContext.CurrentBusinessId!.Value, cancellationToken);

            if (!taskExists)
            {
                throw new NotFoundException("ProjectTask", request.ProjectTaskId.Value.ToString());
            }
        }

        var totalCost = request.Hours * request.Rate;

        var labourEntry = new LabourEntry
        {
            BusinessId = _businessContext.CurrentBusinessId!.Value,
            ProjectId = request.ProjectId,
            UserId = request.UserId,
            WorkDate = request.WorkDate,
            Hours = request.Hours,
            Rate = request.Rate,
            TotalCost = totalCost,
            Description = request.Description,
            ProjectTaskId = request.ProjectTaskId
        };

        _context.LabourEntries.Add(labourEntry);
        await _context.SaveChangesAsync(cancellationToken);

        return labourEntry.Id;
    }
}

