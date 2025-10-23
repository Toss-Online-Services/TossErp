using Toss.Application.Common.Interfaces;
using Toss.Domain.Enums;

namespace Toss.Application.Sales.Queries.GetDailySummary;

public record GetDailySummaryQuery : IRequest<DailySummaryDto>
{
    public int ShopId { get; init; }
    public DateTimeOffset? Date { get; init; }
}

public class GetDailySummaryQueryHandler : IRequestHandler<GetDailySummaryQuery, DailySummaryDto>
{
    private readonly IApplicationDbContext _context;

    public GetDailySummaryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DailySummaryDto> Handle(GetDailySummaryQuery request, CancellationToken cancellationToken)
    {
        var date = request.Date ?? DateTimeOffset.UtcNow;
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1);

        var sales = await _context.Sales
            .Where(s => s.ShopId == request.ShopId 
                && s.SaleDate >= startOfDay 
                && s.SaleDate < endOfDay
                && s.Status == SaleStatus.Completed)
            .ToListAsync(cancellationToken);

        return new DailySummaryDto
        {
            Date = date,
            TotalSales = sales.Sum(s => s.Total),
            SaleCount = sales.Count,
            AverageSaleValue = sales.Any() ? sales.Average(s => s.Total) : 0,
            CashSales = sales.Where(s => s.PaymentMethod == PaymentType.Cash).Sum(s => s.Total),
            CardSales = sales.Where(s => s.PaymentMethod == PaymentType.Card).Sum(s => s.Total),
            MobileMoneySales = sales.Where(s => s.PaymentMethod == PaymentType.MobileMoney).Sum(s => s.Total)
        };
    }
}

public class DailySummaryDto
{
    public DateTimeOffset Date { get; init; }
    public decimal TotalSales { get; init; }
    public int SaleCount { get; init; }
    public decimal AverageSaleValue { get; init; }
    public decimal CashSales { get; init; }
    public decimal CardSales { get; init; }
    public decimal MobileMoneySales { get; init; }
}

