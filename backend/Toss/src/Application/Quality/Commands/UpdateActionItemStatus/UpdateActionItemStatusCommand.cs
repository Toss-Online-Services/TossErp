using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Quality.Commands.UpdateActionItemStatus;

public record UpdateActionItemStatusCommand : IRequest<bool>
{
    public int ActionItemId { get; init; }
    public ActionItemStatus NewStatus { get; init; }
}

public class UpdateActionItemStatusCommandHandler : IRequestHandler<UpdateActionItemStatusCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public UpdateActionItemStatusCommandHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<bool> Handle(UpdateActionItemStatusCommand request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        var actionItem = await _context.ActionItems
            .FirstOrDefaultAsync(a => a.Id == request.ActionItemId
                && a.BusinessId == _businessContext.CurrentBusinessId!.Value, cancellationToken);

        if (actionItem == null)
        {
            return false;
        }

        actionItem.Status = request.NewStatus;

        // Set completion date if status is Completed
        if (request.NewStatus == ActionItemStatus.Completed)
        {
            actionItem.CompletedAt = DateTimeOffset.UtcNow;
        }
        else if (request.NewStatus != ActionItemStatus.Completed && actionItem.CompletedAt.HasValue)
        {
            // Clear completion date if status changes from Completed
            actionItem.CompletedAt = null;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}

