using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Logistics;

namespace Toss.Application.Logistics.Commands.AssignDriver;

public record AssignDriverCommand : IRequest<bool>
{
    public int DeliveryRunId { get; init; }
    public int DriverId { get; init; }
}

public class AssignDriverCommandHandler : IRequestHandler<AssignDriverCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public AssignDriverCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(AssignDriverCommand request, CancellationToken cancellationToken)
    {
        var deliveryRun = await _context.SharedDeliveryRuns
            .FindAsync(new object[] { request.DeliveryRunId }, cancellationToken);

        if (deliveryRun == null)
            throw new NotFoundException(nameof(SharedDeliveryRun), request.DeliveryRunId.ToString());

        var driver = await _context.Drivers
            .FindAsync(new object[] { request.DriverId }, cancellationToken);

        if (driver == null)
            throw new NotFoundException(nameof(Driver), request.DriverId.ToString());

        if (!driver.IsAvailable)
            return false; // Driver not available

        deliveryRun.DriverId = request.DriverId;
        deliveryRun.AssignedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

