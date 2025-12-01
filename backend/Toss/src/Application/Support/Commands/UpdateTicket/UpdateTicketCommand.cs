using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Support.Commands.UpdateTicket;

public record UpdateTicketCommand : IRequest<bool>
{
    public int Id { get; init; }
    public string? Title { get; init; }
    public string? Description { get; init; }
    public string? AssignedToId { get; init; }
    public int? Priority { get; init; }
}

public class UpdateTicketCommandHandler : IRequestHandler<UpdateTicketCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public UpdateTicketCommandHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<bool> Handle(UpdateTicketCommand request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        var ticket = await _context.Tickets
            .FirstOrDefaultAsync(t => t.Id == request.Id
                && t.BusinessId == _businessContext.CurrentBusinessId!.Value, cancellationToken);

        if (ticket == null)
        {
            return false;
        }

        // Don't allow updates to closed or cancelled tickets
        if (ticket.Status == TicketStatus.Closed || ticket.Status == TicketStatus.Cancelled)
        {
            throw new ValidationException("Cannot update a closed or cancelled ticket.");
        }

        if (!string.IsNullOrWhiteSpace(request.Title))
        {
            ticket.Title = request.Title;
        }

        if (request.Description != null)
        {
            ticket.Description = request.Description;
        }

        if (request.AssignedToId != null)
        {
            ticket.AssignedToId = request.AssignedToId;
        }

        if (request.Priority.HasValue)
        {
            if (request.Priority.Value < 1 || request.Priority.Value > 3)
            {
                throw new ValidationException("Priority must be between 1 (Low) and 3 (High).");
            }
            ticket.Priority = request.Priority.Value;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}

