using Toss.Application.Common.Interfaces;
using Toss.Domain.Enums;

namespace Toss.Application.GroupBuying.Queries.GetMyParticipations;

public record ParticipationDto
{
    public int PoolId { get; init; }
    public string PoolTitle { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public int MyQuantity { get; init; }
    public decimal MyCommitment { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime JoinedDate { get; init; }
    public DateTime? TargetDate { get; init; }
}

public record GetMyParticipationsQuery : IRequest<List<ParticipationDto>>
{
    public int ShopId { get; init; }
    public bool ActiveOnly { get; init; } = true;
}

public class GetMyParticipationsQueryHandler : IRequestHandler<GetMyParticipationsQuery, List<ParticipationDto>>
{
    private readonly IApplicationDbContext _context;

    public GetMyParticipationsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ParticipationDto>> Handle(GetMyParticipationsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.PoolParticipations
            .Include(pp => pp.Pool)
                .ThenInclude(p => p.Product)
            .Where(pp => pp.ShopId == request.ShopId)
            .AsQueryable();

        if (request.ActiveOnly)
        {
            query = query.Where(pp => pp.Pool.Status == PoolStatus.Open || pp.Pool.Status == PoolStatus.Confirmed);
        }

        var participations = await query
            .OrderByDescending(pp => pp.JoinedDate)
            .Select(pp => new ParticipationDto
            {
                PoolId = pp.PoolId,
                PoolTitle = pp.Pool.Title,
                ProductName = pp.Pool.Product.Name,
                MyQuantity = pp.Quantity,
                MyCommitment = pp.Quantity * pp.Pool.ProductPrice,
                Status = pp.Pool.Status.ToString(),
                JoinedDate = pp.JoinedDate,
                TargetDate = pp.Pool.TargetDate
            })
            .ToListAsync(cancellationToken);

        return participations;
    }
}

