using Toss.Application.Common.Interfaces;
using Toss.Domain.Enums;

namespace Toss.Application.GroupBuying.Queries.GetActivePools;

public record GetActivePoolsQuery : IRequest<List<PoolSummaryDto>>
{
    public string? AreaGroup { get; init; }
    public int? ProductId { get; init; }
}

public class GetActivePoolsQueryHandler : IRequestHandler<GetActivePoolsQuery, List<PoolSummaryDto>>
{
    private readonly IApplicationDbContext _context;

    public GetActivePoolsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PoolSummaryDto>> Handle(GetActivePoolsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.GroupBuyPools
            .Include(p => p.Product)
            .Include(p => p.Supplier)
            .Include(p => p.Participations)
            .Where(p => p.Status == PoolStatus.Open && p.CloseDate > DateTimeOffset.UtcNow);

        if (!string.IsNullOrWhiteSpace(request.AreaGroup))
            query = query.Where(p => p.AreaGroup == request.AreaGroup);

        if (request.ProductId.HasValue)
            query = query.Where(p => p.ProductId == request.ProductId.Value);

        var pools = await query
            .OrderByDescending(p => p.OpenDate)
            .ToListAsync(cancellationToken);

        return pools.Select(p => new PoolSummaryDto
        {
            Id = p.Id,
            PoolNumber = p.PoolNumber,
            Title = p.Title,
            ProductName = p.Product.Name,
            SupplierName = p.Supplier.Name,
            MinimumQuantity = p.MinimumQuantity,
            CurrentQuantity = p.CurrentQuantity,
            MaximumQuantity = p.MaximumQuantity,
            UnitPrice = p.UnitPrice,
            BulkDiscountPercentage = p.BulkDiscountPercentage,
            FinalUnitPrice = p.FinalUnitPrice,
            ParticipantCount = p.Participations.Count,
            CloseDate = p.CloseDate,
            PercentageFilled = p.MaximumQuantity.HasValue 
                ? (decimal)p.CurrentQuantity / p.MaximumQuantity.Value * 100 
                : (decimal)p.CurrentQuantity / p.MinimumQuantity * 100,
            CanJoin = p.CurrentQuantity < (p.MaximumQuantity ?? int.MaxValue)
        }).ToList();
    }
}

public class PoolSummaryDto
{
    public int Id { get; init; }
    public string PoolNumber { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public string SupplierName { get; init; } = string.Empty;
    public int MinimumQuantity { get; init; }
    public int CurrentQuantity { get; init; }
    public int? MaximumQuantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal BulkDiscountPercentage { get; init; }
    public decimal FinalUnitPrice { get; init; }
    public int ParticipantCount { get; init; }
    public DateTimeOffset CloseDate { get; init; }
    public decimal PercentageFilled { get; init; }
    public bool CanJoin { get; init; }
}

