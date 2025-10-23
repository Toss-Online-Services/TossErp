using Toss.Application.Common.Interfaces;
using Toss.Domain.Enums;

namespace Toss.Application.Dashboard.Queries.GetSalesTrends;

public record SalesTrendDto
{
    public DateTime Date { get; init; }
    public decimal TotalSales { get; init; }
    public int TransactionCount { get; init; }
    public decimal AverageTransaction { get; init; }
}

public record GetSalesTrendsQuery : IRequest<List<SalesTrendDto>>
{
    public int ShopId { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
}

public class GetSalesTrendsQueryHandler : IRequestHandler<GetSalesTrendsQuery, List<SalesTrendDto>>
{
    private readonly IApplicationDbContext _context;

    public GetSalesTrendsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<SalesTrendDto>> Handle(GetSalesTrendsQuery request, CancellationToken cancellationToken)
    {
        var trends = await _context.Sales
            .Where(s => s.ShopId == request.ShopId &&
                       s.SaleDate >= request.StartDate &&
                       s.SaleDate <= request.EndDate &&
                       s.Status != SaleStatus.Voided)
            .GroupBy(s => s.SaleDate.Date)
            .Select(g => new SalesTrendDto
            {
                Date = g.Key,
                TotalSales = g.Sum(s => s.TotalAmount),
                TransactionCount = g.Count(),
                AverageTransaction = g.Average(s => s.TotalAmount)
            })
            .OrderBy(t => t.Date)
            .ToListAsync(cancellationToken);

        return trends;
    }
}

