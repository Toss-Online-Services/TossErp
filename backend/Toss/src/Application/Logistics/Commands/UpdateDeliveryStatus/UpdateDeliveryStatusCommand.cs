using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Logistics;
using Toss.Domain.Enums;

namespace Toss.Application.Logistics.Commands.UpdateDeliveryStatus;

public record UpdateDeliveryStatusCommand : IRequest<bool>
{
    public int DeliveryRunId { get; init; }
    public DeliveryStatus NewStatus { get; init; }
    public string? Notes { get; init; }
}

public class UpdateDeliveryStatusCommandHandler : IRequestHandler<UpdateDeliveryStatusCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UpdateDeliveryStatusCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateDeliveryStatusCommand request, CancellationToken cancellationToken)
    {
        var deliveryRun = await _context.SharedDeliveryRuns
            .FindAsync(new object[] { request.DeliveryRunId }, cancellationToken);

        if (deliveryRun == null)
            throw new NotFoundException(nameof(SharedDeliveryRun), request.DeliveryRunId.ToString());

        deliveryRun.Status = request.NewStatus;

        if (request.NewStatus == DeliveryStatus.InProgress && !deliveryRun.ActualDepartureTime.HasValue)
        {
            deliveryRun.ActualDepartureTime = DateTime.UtcNow;
        }
        else if (request.NewStatus == DeliveryStatus.Completed && !deliveryRun.ActualArrivalTime.HasValue)
        {
            deliveryRun.ActualArrivalTime = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

