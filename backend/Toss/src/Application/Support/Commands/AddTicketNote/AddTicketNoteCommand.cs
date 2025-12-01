using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Application.Common.Interfaces.Authentication;
using Toss.Domain.Entities.Support;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Support.Commands.AddTicketNote;

public record AddTicketNoteCommand : IRequest<int>
{
    public int TicketId { get; init; }
    public string Note { get; init; } = string.Empty;
}

public class AddTicketNoteCommandHandler : IRequestHandler<AddTicketNoteCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;
    private readonly IUser _user;

    public AddTicketNoteCommandHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext,
        IUser user)
    {
        _context = context;
        _businessContext = businessContext;
        _user = user;
    }

    public async Task<int> Handle(AddTicketNoteCommand request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        if (string.IsNullOrWhiteSpace(request.Note))
        {
            throw new ValidationException("Note content is required.");
        }

        // Validate ticket exists
        var ticket = await _context.Tickets
            .FirstOrDefaultAsync(t => t.Id == request.TicketId
                && t.BusinessId == _businessContext.CurrentBusinessId!.Value, cancellationToken);

        if (ticket == null)
        {
            throw new NotFoundException("Ticket", request.TicketId.ToString());
        }

        // Don't allow notes on closed or cancelled tickets
        if (ticket.Status == TicketStatus.Closed || ticket.Status == TicketStatus.Cancelled)
        {
            throw new ValidationException("Cannot add notes to a closed or cancelled ticket.");
        }

        var note = new TicketNote
        {
            BusinessId = _businessContext.CurrentBusinessId!.Value,
            TicketId = request.TicketId,
            Note = request.Note,
            CreatedById = _user.Id ?? string.Empty
        };

        _context.TicketNotes.Add(note);
        await _context.SaveChangesAsync(cancellationToken);

        return note.Id;
    }
}

