using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.Tasks;
using Toss.Domain.Enums;
using TaskStatus = Toss.Domain.Enums.TaskStatus;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Tasks.Commands.UpdateTaskStatus;

public record UpdateTaskStatusCommand : IRequest<bool>
{
    public int TaskId { get; init; }
    public TaskStatus Status { get; init; }
}

public class UpdateTaskStatusCommandHandler : IRequestHandler<UpdateTaskStatusCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public UpdateTaskStatusCommandHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<bool> Handle(UpdateTaskStatusCommand request, CancellationToken cancellationToken)
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

        // Validate status transition
        if (task.Status == TaskStatus.Cancelled && request.Status != TaskStatus.Cancelled)
        {
            throw new ValidationException("Cannot change status of a cancelled task.");
        }

        task.Status = request.Status;

        // Set completion date when status changes to Done
        if (request.Status == TaskStatus.Done && task.CompletedDate == null)
        {
            task.CompletedDate = DateTimeOffset.UtcNow;
        }
        else if (request.Status != TaskStatus.Done)
        {
            task.CompletedDate = null;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}

