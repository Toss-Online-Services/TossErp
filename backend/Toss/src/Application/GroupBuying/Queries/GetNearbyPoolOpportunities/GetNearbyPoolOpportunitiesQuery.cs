using Toss.Application.Common.Interfaces;
using Toss.Domain.Enums;

namespace Toss.Application.GroupBuying.Queries.GetNearbyPoolOpportunities;

public record PoolOpportunityDto
{
    public int PoolId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public decimal ProductPrice { get; init; }
    public int CurrentParticipants { get; init; }
    public int TargetParticipants { get; init; }
    public decimal PotentialSavings { get; init; }
    public DateTime? TargetDate { get; init; }
    public double? DistanceKm { get; init; }
}

public record GetNearbyPoolOpportunitiesQuery : IRequest<List<PoolOpportunityDto>>
{
    public int ShopId { get; init; }
    public double MaxDistanceKm { get; init; } = 10.0;
    public int? ProductId { get; init; }
}

public class GetNearbyPoolOpportunitiesQueryHandler : IRequestHandler<GetNearbyPoolOpportunitiesQuery, List<PoolOpportunityDto>>
{
    private readonly IApplicationDbContext _context;

    public GetNearbyPoolOpportunitiesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PoolOpportunityDto>> Handle(GetNearbyPoolOpportunitiesQuery request, CancellationToken cancellationToken)
    {
        // Get requesting shop's location
        var shop = await _context.Shops.FindAsync(new object[] { request.ShopId }, cancellationToken);
        
        if (shop == null)
            throw new NotFoundException(nameof(Shop), request.ShopId.ToString());

        // Get open pools (simplified - in production, calculate actual distance using geospatial queries)
        var query = _context.GroupBuyPools
            .Include(p => p.Product)
            .Include(p => p.Participations)
            .Where(p => p.Status == PoolStatus.Open && p.CreatorShopId != request.ShopId)
            .AsQueryable();

        if (request.ProductId.HasValue)
        {
            query = query.Where(p => p.ProductId == request.ProductId.Value);
        }

        var pools = await query
            .OrderByDescending(p => p.Created)
            .Select(p => new PoolOpportunityDto
            {
                PoolId = p.Id,
                Title = p.Title,
                ProductName = p.Product.Name,
                ProductPrice = p.ProductPrice,
                CurrentParticipants = p.Participations.Count,
                TargetParticipants = p.TargetParticipants,
                PotentialSavings = p.ProductPrice * 0.15m, // Simplified: 15% savings estimate
                TargetDate = p.TargetDate,
                DistanceKm = null // TODO: Calculate actual distance using Location value object
            })
            .Take(20)
            .ToListAsync(cancellationToken);

        return pools;
    }
}

