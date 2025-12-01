using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Application.Common.Interfaces.Authentication;
using Toss.Domain.Entities.Support;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Support.Commands.CreateTicket;

public record CreateTicketCommand : IRequest<int>
{
    public TicketType Type { get; init; } = TicketType.Issue;
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? LinkedEntityType { get; init; }
    public int? LinkedEntityId { get; init; }
    public string? AssignedToId { get; init; }
    public int Priority { get; init; } = 2; // Default to Medium
}

public class CreateTicketCommandHandler : IRequestHandler<CreateTicketCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;
    private readonly IUser _user;

    public CreateTicketCommandHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext,
        IUser user)
    {
        _context = context;
        _businessContext = businessContext;
        _user = user;
    }

    public async Task<int> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ValidationException("Ticket title is required.");
        }

        if (request.Priority < 1 || request.Priority > 3)
        {
            throw new ValidationException("Priority must be between 1 (Low) and 3 (High).");
        }

        // Validate linked entity exists if provided
        if (!string.IsNullOrWhiteSpace(request.LinkedEntityType) && request.LinkedEntityId.HasValue)
        {
            // Basic validation - could be extended to check specific entity types
            // For now, we'll just ensure the linked entity type is not empty
        }

        var ticket = new Ticket
        {
            BusinessId = _businessContext.CurrentBusinessId!.Value,
            Type = request.Type,
            Title = request.Title,
            Description = request.Description,
            Status = TicketStatus.New,
            LinkedEntityType = request.LinkedEntityType,
            LinkedEntityId = request.LinkedEntityId,
            AssignedToId = request.AssignedToId,
            Priority = request.Priority,
            CreatedById = _user.Id ?? string.Empty
        };

        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync(cancellationToken);

        return ticket.Id;
    }
}

