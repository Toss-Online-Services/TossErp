using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Support.Queries.GetTicketById;

public record GetTicketByIdQuery : IRequest<TicketDetailDto?>
{
    public int Id { get; init; }
}

public class GetTicketByIdQueryHandler : IRequestHandler<GetTicketByIdQuery, TicketDetailDto?>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public GetTicketByIdQueryHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<TicketDetailDto?> Handle(GetTicketByIdQuery request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        var ticket = await _context.Tickets
            .Include(t => t.Notes.OrderBy(n => n.Created))
            .FirstOrDefaultAsync(t => t.Id == request.Id
                && t.BusinessId == _businessContext.CurrentBusinessId!.Value, cancellationToken);

        if (ticket == null)
        {
            return null;
        }

        return new TicketDetailDto
        {
            Id = ticket.Id,
            Type = ticket.Type,
            Status = ticket.Status,
            Title = ticket.Title,
            Description = ticket.Description,
            LinkedEntityType = ticket.LinkedEntityType,
            LinkedEntityId = ticket.LinkedEntityId,
            CreatedById = ticket.CreatedById,
            AssignedToId = ticket.AssignedToId,
            ClosedAt = ticket.ClosedAt,
            Priority = ticket.Priority,
            Notes = ticket.Notes.Select(n => new TicketNoteDto
            {
                Id = n.Id,
                Note = n.Note,
                CreatedById = n.CreatedById,
                Created = n.Created
            }).ToList(),
            Created = ticket.Created
        };
    }
}

public record TicketDetailDto
{
    public int Id { get; init; }
    public Toss.Domain.Enums.TicketType Type { get; init; }
    public Toss.Domain.Enums.TicketStatus Status { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? LinkedEntityType { get; init; }
    public int? LinkedEntityId { get; init; }
    public string CreatedById { get; init; } = string.Empty;
    public string? AssignedToId { get; init; }
    public DateTimeOffset? ClosedAt { get; init; }
    public int Priority { get; init; }
    public List<TicketNoteDto> Notes { get; init; } = new();
    public DateTimeOffset Created { get; init; }
}

public record TicketNoteDto
{
    public int Id { get; init; }
    public string Note { get; init; } = string.Empty;
    public string CreatedById { get; init; } = string.Empty;
    public DateTimeOffset Created { get; init; }
}

