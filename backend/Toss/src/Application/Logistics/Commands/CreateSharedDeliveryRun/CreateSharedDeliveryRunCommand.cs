using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Stores;
using Toss.Domain.Entities;
using Toss.Domain.Entities.Logistics;
using Toss.Domain.Enums;
using Toss.Domain.ValueObjects;

namespace Toss.Application.Logistics.Commands.CreateSharedDeliveryRun;

public record CreateSharedDeliveryRunCommand : IRequest<int>
{
    public int? GroupBuyPoolId { get; init; }
    public DateTimeOffset ScheduledDate { get; init; }
    public string? AreaGroup { get; init; }
    public decimal TotalDeliveryCost { get; init; }
    public List<DeliveryStopDto> Stops { get; init; } = new();
}

public record DeliveryStopDto
{
    public int ShopId { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public string? DeliveryInstructions { get; init; }
    public int? PurchaseOrderId { get; init; }
    public int? PoolParticipationId { get; init; }
}

public class CreateSharedDeliveryRunCommandHandler : IRequestHandler<CreateSharedDeliveryRunCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateSharedDeliveryRunCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateSharedDeliveryRunCommand request, CancellationToken cancellationToken)
    {
        if (request.Stops.Count == 0)
            throw new InvalidOperationException("Delivery run must have at least one stop");

        var deliveryRun = new SharedDeliveryRun
        {
            RunNumber = await GenerateRunNumber(cancellationToken),
            GroupBuyPoolId = request.GroupBuyPoolId,
            ScheduledDate = request.ScheduledDate,
            Status = DeliveryStatus.Scheduled,
            AreaGroup = request.AreaGroup,
            TotalDeliveryCost = request.TotalDeliveryCost,
            ParticipantCount = request.Stops.Count,
            CostPerStop = request.TotalDeliveryCost / request.Stops.Count
        };

        // Create delivery stops
        int sequenceNumber = 1;
        foreach (var stopDto in request.Stops)
        {
            var shop = await _context.Shops.FindAsync(new object[] { stopDto.ShopId }, cancellationToken);
            if (shop == null)
                throw new NotFoundException(nameof(Store), stopDto.ShopId.ToString());

            var stop = new DeliveryStop
            {
                ShopId = stopDto.ShopId,
                SequenceNumber = sequenceNumber++,
                DeliveryLocation = new Location(stopDto.Latitude, stopDto.Longitude),
                DeliveryInstructions = stopDto.DeliveryInstructions,
                CostShare = deliveryRun.CostPerStop,
                Status = DeliveryStatus.Scheduled,
                PurchaseOrderId = stopDto.PurchaseOrderId,
                PoolParticipationId = stopDto.PoolParticipationId
            };

            deliveryRun.Stops.Add(stop);
        }

        _context.SharedDeliveryRuns.Add(deliveryRun);
        await _context.SaveChangesAsync(cancellationToken);

        return deliveryRun.Id;
    }

    private async Task<string> GenerateRunNumber(CancellationToken cancellationToken)
    {
        var date = DateTimeOffset.UtcNow;
        var count = await _context.SharedDeliveryRuns
            .Where(r => r.ScheduledDate.Date == date.Date)
            .CountAsync(cancellationToken);

        return $"RUN-{date:yyyyMMdd}-{count + 1:D4}";
    }
}

