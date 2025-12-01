using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Tasks.Commands.LinkTask;

public record LinkTaskCommand : IRequest<bool>
{
    public int TaskId { get; init; }
    public string? LinkedType { get; init; }
    public int? LinkedId { get; init; }
}

public class LinkTaskCommandHandler : IRequestHandler<LinkTaskCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public LinkTaskCommandHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<bool> Handle(LinkTaskCommand request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        var task = await _context.TaskItems
            .FirstOrDefaultAsync(t => t.Id == request.TaskId
                && t.BusinessId == _businessContext.CurrentBusinessId!.Value, cancellationToken);

        if (task == null)
        {
            return false;
        }

        // Unlink if both are null
        if (string.IsNullOrWhiteSpace(request.LinkedType) || !request.LinkedId.HasValue)
        {
            task.LinkedType = null;
            task.LinkedId = null;
        }
        else
        {
            task.LinkedType = request.LinkedType;
            task.LinkedId = request.LinkedId;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}

