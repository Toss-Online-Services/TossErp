using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.Projects;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Projects.Commands.UpdateProjectStatus;

public record UpdateProjectStatusCommand : IRequest<bool>
{
    public int ProjectId { get; init; }
    public ProjectStatus Status { get; init; }
}

public class UpdateProjectStatusCommandHandler : IRequestHandler<UpdateProjectStatusCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public UpdateProjectStatusCommandHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<bool> Handle(UpdateProjectStatusCommand request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == request.ProjectId
                && p.BusinessId == _businessContext.CurrentBusinessId!.Value, cancellationToken);

        if (project == null)
        {
            return false;
        }

        // Validate status transition
        if (project.Status == ProjectStatus.Cancelled && request.Status != ProjectStatus.Cancelled)
        {
            throw new ValidationException("Cannot change status of a cancelled project.");
        }

        if (project.Status == ProjectStatus.Closed && request.Status != ProjectStatus.Closed)
        {
            throw new ValidationException("Cannot change status of a closed project.");
        }

        project.Status = request.Status;

        // Set completion date when status changes to Completed
        if (request.Status == ProjectStatus.Completed && project.CompletedDate == null)
        {
            project.CompletedDate = DateTimeOffset.UtcNow;
        }
        else if (request.Status != ProjectStatus.Completed)
        {
            project.CompletedDate = null;
        }

        // Set start date when status changes to InProgress
        if (request.Status == ProjectStatus.InProgress && project.StartDate == null)
        {
            project.StartDate = DateTimeOffset.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}

