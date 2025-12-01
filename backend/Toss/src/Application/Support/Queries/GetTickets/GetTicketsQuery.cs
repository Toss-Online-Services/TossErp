using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Application.Common.Models;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Support.Queries.GetTickets;

public record GetTicketsQuery : IRequest<PaginatedList<TicketDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public TicketStatus? Status { get; init; }
    public TicketType? Type { get; init; }
    public string? LinkedEntityType { get; init; }
    public int? LinkedEntityId { get; init; }
    public string? AssignedToId { get; init; }
    public string? SearchTerm { get; init; }
}

public class GetTicketsQueryHandler : IRequestHandler<GetTicketsQuery, PaginatedList<TicketDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public GetTicketsQueryHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<PaginatedList<TicketDto>> Handle(GetTicketsQuery request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        var query = _context.Tickets
            .Where(t => t.BusinessId == _businessContext.CurrentBusinessId!.Value)
            .AsQueryable();

        if (request.Status.HasValue)
        {
            query = query.Where(t => t.Status == request.Status.Value);
        }

        if (request.Type.HasValue)
        {
            query = query.Where(t => t.Type == request.Type.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.LinkedEntityType))
        {
            query = query.Where(t => t.LinkedEntityType == request.LinkedEntityType);
        }

        if (request.LinkedEntityId.HasValue)
        {
            query = query.Where(t => t.LinkedEntityId == request.LinkedEntityId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.AssignedToId))
        {
            query = query.Where(t => t.AssignedToId == request.AssignedToId);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(t =>
                t.Title.ToLower().Contains(searchTerm) ||
                (t.Description != null && t.Description.ToLower().Contains(searchTerm)));
        }

        query = query.OrderByDescending(t => t.Created);

        var ticketQuery = query
            .Select(t => new TicketDto
            {
                Id = t.Id,
                Type = t.Type,
                Status = t.Status,
                Title = t.Title,
                Description = t.Description,
                LinkedEntityType = t.LinkedEntityType,
                LinkedEntityId = t.LinkedEntityId,
                CreatedById = t.CreatedById,
                AssignedToId = t.AssignedToId,
                ClosedAt = t.ClosedAt,
                Priority = t.Priority,
                NoteCount = t.Notes.Count,
                Created = t.Created
            });

        return await PaginatedList<TicketDto>.CreateAsync(ticketQuery, request.PageNumber, request.PageSize, cancellationToken);
    }
}

public record TicketDto
{
    public int Id { get; init; }
    public TicketType Type { get; init; }
    public TicketStatus Status { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? LinkedEntityType { get; init; }
    public int? LinkedEntityId { get; init; }
    public string CreatedById { get; init; } = string.Empty;
    public string? AssignedToId { get; init; }
    public DateTimeOffset? ClosedAt { get; init; }
    public int Priority { get; init; }
    public int NoteCount { get; init; }
    public DateTimeOffset Created { get; init; }
}

