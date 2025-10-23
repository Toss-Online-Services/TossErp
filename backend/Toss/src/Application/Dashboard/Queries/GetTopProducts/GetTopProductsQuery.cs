using Toss.Application.Common.Interfaces;
using Toss.Domain.Enums;

namespace Toss.Application.Dashboard.Queries.GetTopProducts;

public record TopProductDto
{
    public int ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public int QuantitySold { get; init; }
    public decimal Revenue { get; init; }
}

public record GetTopProductsQuery : IRequest<List<TopProductDto>>
{
    public int ShopId { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public int TopN { get; init; } = 10;
}

public class GetTopProductsQueryHandler : IRequestHandler<GetTopProductsQuery, List<TopProductDto>>
{
    private readonly IApplicationDbContext _context;

    public GetTopProductsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TopProductDto>> Handle(GetTopProductsQuery request, CancellationToken cancellationToken)
    {
        var startDate = request.StartDate ?? DateTime.UtcNow.AddDays(-30);
        var endDate = request.EndDate ?? DateTime.UtcNow;

        var topProducts = await _context.SaleItems
            .Where(si => si.Sale.ShopId == request.ShopId &&
                        si.Sale.SaleDate >= startDate &&
                        si.Sale.SaleDate <= endDate &&
                        si.Sale.Status != SaleStatus.Voided)
            .GroupBy(si => new { si.ProductId, si.Product.Name })
            .Select(g => new TopProductDto
            {
                ProductId = g.Key.ProductId,
                ProductName = g.Key.Name,
                QuantitySold = g.Sum(si => si.Quantity),
                Revenue = g.Sum(si => si.LineTotal)
            })
            .OrderByDescending(p => p.Revenue)
            .Take(request.TopN)
            .ToListAsync(cancellationToken);

        return topProducts;
    }
}

