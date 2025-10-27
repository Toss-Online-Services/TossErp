using Toss.Application.Common.Interfaces;

namespace Toss.Application.Logistics.Queries.GetDeliveryTracking;

public record DeliveryTrackingDto
{
    public int RunId { get; init; }
    public string RunNumber { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string? DriverName { get; init; }
    public string? DriverPhone { get; init; }
    public DateTimeOffset? StartedAt { get; init; }
    public DateTimeOffset? CompletedAt { get; init; }
    public List<DeliveryStopTrackingDto> Stops { get; init; } = new();
}

public record DeliveryStopTrackingDto
{
    public int StopId { get; init; }
    public int SequenceNumber { get; init; }
    public string ShopName { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTimeOffset? ArrivalTime { get; init; }
    public DateTimeOffset? CompletionTime { get; init; }
    public bool ProofOfDelivery { get; init; }
}

public record GetDeliveryTrackingQuery : IRequest<DeliveryTrackingDto>
{
    public int RunId { get; init; }
}

public class GetDeliveryTrackingQueryHandler : IRequestHandler<GetDeliveryTrackingQuery, DeliveryTrackingDto>
{
    private readonly IApplicationDbContext _context;

    public GetDeliveryTrackingQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DeliveryTrackingDto> Handle(GetDeliveryTrackingQuery request, CancellationToken cancellationToken)
    {
        var run = await _context.SharedDeliveryRuns
            .Include(r => r.Driver)
            .Include(r => r.Stops)
                .ThenInclude(s => s.Shop)
            .Include(r => r.Stops)
                .ThenInclude(s => s.ProofOfDeliveries)
            .FirstOrDefaultAsync(r => r.Id == request.RunId, cancellationToken);

        if (run == null)
            throw new NotFoundException("SharedDeliveryRun", request.RunId.ToString());

        return new DeliveryTrackingDto
        {
            RunId = run.Id,
            RunNumber = run.RunNumber,
            Status = run.Status.ToString(),
            DriverName = run.Driver?.Name,
            DriverPhone = run.Driver?.Phone.ToString(),
            StartedAt = run.StartedAt,
            CompletedAt = run.CompletedAt,
            Stops = run.Stops
                .OrderBy(s => s.SequenceNumber)
                .Select(s => new DeliveryStopTrackingDto
                {
                    StopId = s.Id,
                    SequenceNumber = s.SequenceNumber,
                    ShopName = s.Shop.Name,
                    Address = s.DeliveryLocation.ToString(),
                    Status = s.Status.ToString(),
                    ArrivalTime = s.ArrivalTime,
                    CompletionTime = s.CompletionTime,
                    ProofOfDelivery = s.ProofOfDeliveries.Any()
                })
                .ToList()
        };
    }
}

