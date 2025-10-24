using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities;
using Toss.Domain.Entities.Logistics;
using Toss.Domain.Enums;

namespace Toss.Application.Logistics.Queries.GetDriverRunView;

public record DriverRunViewDto
{
    public int RunId { get; init; }
    public string RunNumber { get; init; } = string.Empty;
    public DateTimeOffset ScheduledDate { get; init; }
    public string Status { get; init; } = string.Empty;
    public int TotalStops { get; init; }
    public int CompletedStops { get; init; }
    public decimal TotalDistance { get; init; }
    public List<StopDto> Stops { get; init; } = new();
}

public record StopDto
{
    public int Id { get; init; }
    public int SequenceNumber { get; init; }
    public string ShopName { get; init; } = string.Empty;
    public string? Address { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTimeOffset? ActualDeliveryTime { get; init; }
    public bool HasProofOfDelivery { get; init; }
}

public record GetDriverRunViewQuery : IRequest<DriverRunViewDto>
{
    public int RunId { get; init; }
    public int DriverId { get; init; }
}

public class GetDriverRunViewQueryHandler : IRequestHandler<GetDriverRunViewQuery, DriverRunViewDto>
{
    private readonly IApplicationDbContext _context;

    public GetDriverRunViewQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DriverRunViewDto> Handle(GetDriverRunViewQuery request, CancellationToken cancellationToken)
    {
        var run = await _context.SharedDeliveryRuns
            .Include(r => r.Stops)
                .ThenInclude(s => s.Shop)
            .Include(r => r.Stops)
                .ThenInclude(s => s.ProofOfDeliveries)
            .FirstOrDefaultAsync(r => r.Id == request.RunId && r.DriverId == request.DriverId, cancellationToken);

        if (run == null)
            throw new NotFoundException(nameof(SharedDeliveryRun), request.RunId.ToString());

        return new DriverRunViewDto
        {
            RunId = run.Id,
            RunNumber = run.RunNumber,
            ScheduledDate = run.ScheduledDate,
            Status = run.Status.ToString(),
            TotalStops = run.Stops.Count,
            CompletedStops = run.Stops.Count(s => s.Status == DeliveryStatus.Completed),
            TotalDistance = run.TotalDistance,
            Stops = run.Stops
                .OrderBy(s => s.SequenceNumber)
                .Select(s => new StopDto
                {
                    Id = s.Id,
                    SequenceNumber = s.SequenceNumber,
                    ShopName = s.Shop.Name,
                    Address = s.Shop.Address != null ? $"{s.Shop.Address.Street}, {s.Shop.Address.City}" : null,
                    Status = s.Status.ToString(),
                    ActualDeliveryTime = s.ActualDeliveryTime,
                    HasProofOfDelivery = s.ProofOfDeliveries.Any()
                })
                .ToList()
        };
    }
}

