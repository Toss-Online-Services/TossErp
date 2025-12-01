using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.Tasks;
using Toss.Domain.Enums;
using TaskStatus = Toss.Domain.Enums.TaskStatus;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Tasks.Commands.CreateTask;

public record CreateTaskCommand : IRequest<int>
{
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public TaskStatus Status { get; init; } = TaskStatus.ToDo;
    public DateTimeOffset? DueDate { get; init; }
    public string? LinkedType { get; init; }
    public int? LinkedId { get; init; }
    public string? AssigneeId { get; init; }
    public int Priority { get; init; } = 2; // Default to Medium
    public string? Tags { get; init; }
}

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public CreateTaskCommandHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<int> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ValidationException("Task title is required.");
        }

        if (request.Priority < 1 || request.Priority > 3)
        {
            throw new ValidationException("Priority must be between 1 (Low) and 3 (High).");
        }

        // Validate assignee exists if provided
        // Note: In a full implementation, we would use UserManager or IUserManagementService
        // For MVP, we'll skip this validation or validate via a service if needed
        // The assignee will be validated at the API layer if required

        var task = new TaskItem
        {
            BusinessId = _businessContext.CurrentBusinessId!.Value,
            Title = request.Title,
            Description = request.Description,
            Status = request.Status,
            DueDate = request.DueDate,
            LinkedType = request.LinkedType,
            LinkedId = request.LinkedId,
            AssigneeId = request.AssigneeId,
            Priority = request.Priority,
            Tags = request.Tags
        };

        _context.TaskItems.Add(task);
        await _context.SaveChangesAsync(cancellationToken);

        return task.Id;
    }
}

