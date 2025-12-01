using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Tasks.Commands.UpdateTask;

public record UpdateTaskCommand : IRequest<bool>
{
    public int TaskId { get; init; }
    public string? Title { get; init; }
    public string? Description { get; init; }
    public DateTimeOffset? DueDate { get; init; }
    public string? AssigneeId { get; init; }
    public int? Priority { get; init; }
    public string? Tags { get; init; }
}

public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public UpdateTaskCommandHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<bool> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        var task = await _context.TaskItems
            .FirstOrDefaultAsync(t => t.Id == request.TaskId
                && t.BusinessId == _businessContext.CurrentBusinessId!.Value, cancellationToken);

        if (task == null)
        {
            return false;
        }

        // Update fields if provided
        if (request.Title != null)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                throw new ValidationException("Task title cannot be empty.");
            }
            task.Title = request.Title;
        }

        if (request.Description != null)
        {
            task.Description = request.Description;
        }

        if (request.DueDate.HasValue)
        {
            task.DueDate = request.DueDate.Value;
        }

        if (request.AssigneeId != null)
        {
            if (string.IsNullOrWhiteSpace(request.AssigneeId))
            {
                task.AssigneeId = null;
            }
            else
            {
                // Validate assignee exists
                // Note: In a full implementation, we would use UserManager or IUserManagementService
                // For MVP, we'll accept the assignee ID and validate at the API layer if required
                task.AssigneeId = request.AssigneeId;
            }
        }

        if (request.Priority.HasValue)
        {
            if (request.Priority.Value < 1 || request.Priority.Value > 3)
            {
                throw new ValidationException("Priority must be between 1 (Low) and 3 (High).");
            }
            task.Priority = request.Priority.Value;
        }

        if (request.Tags != null)
        {
            task.Tags = request.Tags;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}

