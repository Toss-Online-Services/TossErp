using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Application.Common.Mappings;
using Toss.Application.Common.Models;
using Toss.Domain.Entities.Tasks;
using Toss.Domain.Enums;
using TaskStatus = Toss.Domain.Enums.TaskStatus;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Tasks.Queries.GetTasks;

public record GetTasksQuery : IRequest<PaginatedList<TaskItemDto>>
{
    public TaskStatus? Status { get; init; }
    public string? LinkedType { get; init; }
    public int? LinkedId { get; init; }
    public string? AssigneeId { get; init; }
    public int? Priority { get; init; }
    public DateTimeOffset? DueDateFrom { get; init; }
    public DateTimeOffset? DueDateTo { get; init; }
    public string? SearchTerm { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 50;
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

public class GetTasksQueryHandler : IRequestHandler<GetTasksQuery, PaginatedList<TaskItemDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public GetTasksQueryHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<PaginatedList<TaskItemDto>> Handle(GetTasksQuery request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        var query = _context.TaskItems
            .Where(t => t.BusinessId == _businessContext.CurrentBusinessId!.Value)
            .AsQueryable();

        if (request.Status.HasValue)
        {
            query = query.Where(t => t.Status == request.Status.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.LinkedType))
        {
            query = query.Where(t => t.LinkedType == request.LinkedType);
        }

        if (request.LinkedId.HasValue)
        {
            query = query.Where(t => t.LinkedId == request.LinkedId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.AssigneeId))
        {
            query = query.Where(t => t.AssigneeId == request.AssigneeId);
        }

        if (request.Priority.HasValue)
        {
            query = query.Where(t => t.Priority == request.Priority.Value);
        }

        if (request.DueDateFrom.HasValue)
        {
            query = query.Where(t => t.DueDate.HasValue && t.DueDate >= request.DueDateFrom.Value);
        }

        if (request.DueDateTo.HasValue)
        {
            query = query.Where(t => t.DueDate.HasValue && t.DueDate <= request.DueDateTo.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(t => 
                t.Title.ToLower().Contains(searchTerm) ||
                (t.Description != null && t.Description.ToLower().Contains(searchTerm)) ||
                (t.Tags != null && t.Tags.ToLower().Contains(searchTerm)));
        }

        return await query
            .OrderByDescending(t => t.Priority)
            .ThenBy(t => t.DueDate ?? DateTimeOffset.MaxValue)
            .ThenByDescending(t => t.Created)
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
            .PaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
    }
}

