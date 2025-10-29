using Toss.Application.Common.Interfaces;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Dashboard.Queries.GetCategorySales;

public record CategorySalesDto
{
    public int CategoryId { get; init; }
    public string CategoryName { get; init; } = string.Empty;
    public decimal TotalSales { get; init; }
    public decimal Percentage { get; init; }
    public int TransactionCount { get; init; }
}

public record GetCategorySalesQuery : IRequest<List<CategorySalesDto>>
{
    public int ShopId { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
}

public class GetCategorySalesQueryHandler : IRequestHandler<GetCategorySalesQuery, List<CategorySalesDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCategorySalesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CategorySalesDto>> Handle(GetCategorySalesQuery request, CancellationToken cancellationToken)
    {
        var startDate = request.StartDate ?? DateTime.UtcNow.AddDays(-30);
        var endDate = request.EndDate ?? DateTime.UtcNow;

        var sales = await _context.SaleItems
            .Include(si => si.Sale)
            .Include(si => si.Product)
                .ThenInclude(p => p.Category)
            .Where(si => si.Sale.ShopId == request.ShopId &&
                        si.Sale.SaleDate >= startDate &&
                        si.Sale.SaleDate <= endDate &&
                        si.Sale.Status != SaleStatus.Voided)
            .ToListAsync(cancellationToken);

        var totalSales = sales.Sum(si => si.LineTotal);

        var categorySales = sales
            .Where(si => si.Product.CategoryId.HasValue)
            .GroupBy(si => new
            {
                CategoryId = si.Product.CategoryId!.Value,
                CategoryName = si.Product.Category!.Name ?? "Uncategorized"
            })
            .Select(g => new CategorySalesDto
            {
                CategoryId = g.Key.CategoryId,
                CategoryName = g.Key.CategoryName,
                TotalSales = g.Sum(si => si.LineTotal),
                Percentage = totalSales > 0 ? (g.Sum(si => si.LineTotal) / totalSales * 100) : 0,
                TransactionCount = g.Select(si => si.SaleId).Distinct().Count()
            })
            .OrderByDescending(cs => cs.TotalSales)
            .ToList();

        // Add uncategorized sales
        var uncategorizedSales = sales
            .Where(si => !si.Product.CategoryId.HasValue)
            .ToList();

        if (uncategorizedSales.Any())
        {
            categorySales.Add(new CategorySalesDto
            {
                CategoryId = 0,
                CategoryName = "Uncategorized",
                TotalSales = uncategorizedSales.Sum(si => si.LineTotal),
                Percentage = totalSales > 0 ? (uncategorizedSales.Sum(si => si.LineTotal) / totalSales * 100) : 0,
                TransactionCount = uncategorizedSales.Select(si => si.SaleId).Distinct().Count()
            });
        }

        return categorySales.OrderByDescending(cs => cs.TotalSales).ToList();
    }
}

