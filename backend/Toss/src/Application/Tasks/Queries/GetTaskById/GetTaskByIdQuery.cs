using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.Tasks;
using Toss.Domain.Enums;
using TaskStatus = Toss.Domain.Enums.TaskStatus;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Tasks.Queries.GetTaskById;

public record GetTaskByIdQuery : IRequest<TaskItemDto?>
{
    public int Id { get; init; }
}

public record TaskItemDto
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public TaskStatus Status { get; init; }
    public DateTimeOffset? DueDate { get; init; }
    public string? LinkedType { get; init; }
    public int? LinkedId { get; init; }
    public string? AssigneeId { get; init; }
    public string? AssigneeName { get; init; }
    public int Priority { get; init; }
    public string? Tags { get; init; }
    public DateTimeOffset? CompletedDate { get; init; }
    public DateTimeOffset Created { get; init; }
    public DateTimeOffset LastModified { get; init; }
}

public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, TaskItemDto?>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public GetTaskByIdQueryHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<TaskItemDto?> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        var task = await _context.TaskItems
            .Where(t => t.Id == request.Id
                && t.BusinessId == _businessContext.CurrentBusinessId!.Value)
            .Select(t => new TaskItemDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status,
                DueDate = t.DueDate,
                LinkedType = t.LinkedType,
                LinkedId = t.LinkedId,
                AssigneeId = t.AssigneeId,
                AssigneeName = null, // Would need to join with Users table if needed
                Priority = t.Priority,
                Tags = t.Tags,
                CompletedDate = t.CompletedDate,
                Created = t.Created,
                LastModified = t.LastModified
            })
            .FirstOrDefaultAsync(cancellationToken);

        return task;
    }
}

