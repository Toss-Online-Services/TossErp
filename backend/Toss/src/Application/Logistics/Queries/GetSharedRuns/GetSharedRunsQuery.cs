using Toss.Application.Common.Interfaces;
using Toss.Domain.Enums;

namespace Toss.Application.Logistics.Queries.GetSharedRuns;

public record SharedRunDto
{
    public int Id { get; init; }
    public string RunNumber { get; init; } = string.Empty;
    public DateTime ScheduledDate { get; init; }
    public string Status { get; init; } = string.Empty;
    public int? DriverId { get; init; }
    public string? DriverName { get; init; }
    public int StopCount { get; init; }
    public decimal TotalDistance { get; init; }
    public decimal TotalCost { get; init; }
}

public record GetSharedRunsQuery : IRequest<List<SharedRunDto>>
{
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public DeliveryStatus? Status { get; init; }
}

public class GetSharedRunsQueryHandler : IRequestHandler<GetSharedRunsQuery, List<SharedRunDto>>
{
    private readonly IApplicationDbContext _context;

    public GetSharedRunsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<SharedRunDto>> Handle(GetSharedRunsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.SharedDeliveryRuns
            .Include(sdr => sdr.Driver)
            .Include(sdr => sdr.Stops)
            .AsQueryable();

        if (request.StartDate.HasValue)
            query = query.Where(sdr => sdr.ScheduledDate >= request.StartDate.Value);

        if (request.EndDate.HasValue)
            query = query.Where(sdr => sdr.ScheduledDate <= request.EndDate.Value);

        if (request.Status.HasValue)
            query = query.Where(sdr => sdr.Status == request.Status.Value);

        var runs = await query
            .OrderByDescending(sdr => sdr.ScheduledDate)
            .Select(sdr => new SharedRunDto
            {
                Id = sdr.Id,
                RunNumber = sdr.RunNumber,
                ScheduledDate = sdr.ScheduledDate,
                Status = sdr.Status.ToString(),
                DriverId = sdr.DriverId,
                DriverName = sdr.Driver != null ? sdr.Driver.Name : null,
                StopCount = sdr.Stops.Count,
                TotalDistance = sdr.TotalDistance,
                TotalCost = sdr.TotalCost
            })
            .ToListAsync(cancellationToken);

        return runs;
    }
}

