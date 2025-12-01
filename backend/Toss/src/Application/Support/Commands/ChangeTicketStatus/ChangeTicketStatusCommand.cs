using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.Support;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using MediatR;

namespace Toss.Application.Support.Commands.ChangeTicketStatus;

public record ChangeTicketStatusCommand : IRequest<bool>
{
    public int TicketId { get; init; }
    public TicketStatus NewStatus { get; init; }
}

public class ChangeTicketStatusCommandHandler : IRequestHandler<ChangeTicketStatusCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;
    private readonly IPublisher _publisher;

    public ChangeTicketStatusCommandHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext,
        IPublisher publisher)
    {
        _context = context;
        _businessContext = businessContext;
        _publisher = publisher;
    }

    public async Task<bool> Handle(ChangeTicketStatusCommand request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        var ticket = await _context.Tickets
            .FirstOrDefaultAsync(t => t.Id == request.TicketId
                && t.BusinessId == _businessContext.CurrentBusinessId!.Value, cancellationToken);

        if (ticket == null)
        {
            return false;
        }

        var oldStatus = ticket.Status;

        // Validate status transition
        if (!IsValidTransition(oldStatus, request.NewStatus))
        {
            throw new ValidationException($"Invalid status transition from {oldStatus} to {request.NewStatus}.");
        }

        // Prevent duplicate closing
        if (request.NewStatus == TicketStatus.Closed && ticket.Status == TicketStatus.Closed)
        {
            throw new ValidationException("Ticket is already closed.");
        }

        ticket.Status = request.NewStatus;

        // Set closed date if status is Closed
        if (request.NewStatus == TicketStatus.Closed)
        {
            ticket.ClosedAt = DateTimeOffset.UtcNow;
        }
        else if (request.NewStatus != TicketStatus.Closed && ticket.ClosedAt.HasValue)
        {
            // Clear closed date if status changes from Closed
            ticket.ClosedAt = null;
        }

        await _context.SaveChangesAsync(cancellationToken);

        // Publish domain event for notifications (if status changed)
        if (oldStatus != request.NewStatus)
        {
            await _publisher.Publish(new TicketStatusChangedEvent
            {
                TicketId = ticket.Id,
                OldStatus = oldStatus,
                NewStatus = request.NewStatus,
                BusinessId = ticket.BusinessId
            }, cancellationToken);
        }

        return true;
    }

    private static bool IsValidTransition(TicketStatus from, TicketStatus to)
    {
        // Define allowed transitions
        return from switch
        {
            TicketStatus.New => to == TicketStatus.Open || to == TicketStatus.Cancelled,
            TicketStatus.Open => to == TicketStatus.InProgress || to == TicketStatus.Resolved || to == TicketStatus.Cancelled,
            TicketStatus.InProgress => to == TicketStatus.Resolved || to == TicketStatus.Open || to == TicketStatus.Cancelled,
            TicketStatus.Resolved => to == TicketStatus.Closed || to == TicketStatus.InProgress,
            TicketStatus.Closed => false, // Cannot transition from closed
            TicketStatus.Cancelled => false, // Cannot transition from cancelled
            _ => false
        };
    }
}

/// <summary>
/// Domain event published when a ticket status changes
/// </summary>
public record TicketStatusChangedEvent : INotification
{
    public int TicketId { get; init; }
    public TicketStatus OldStatus { get; init; }
    public TicketStatus NewStatus { get; init; }
    public int BusinessId { get; init; }
}

