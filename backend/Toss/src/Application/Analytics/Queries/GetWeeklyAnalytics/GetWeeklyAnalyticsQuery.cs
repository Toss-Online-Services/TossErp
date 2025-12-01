using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Analytics.Queries.GetWeeklyAnalytics;

public record GetWeeklyAnalyticsQuery : IRequest<List<WeeklyAnalyticsDto>>
{
    public int? Weeks { get; init; } = 4; // Default to last 4 weeks
}

public record WeeklyAnalyticsDto
{
    public DateTimeOffset WeekStart { get; init; }
    public DateTimeOffset WeekEnd { get; init; }
    public int Logins { get; init; }
    public int PosSales { get; init; }
    public decimal SalesAmount { get; init; }
    public int StockAlertsResolved { get; init; }
    public int StockOuts { get; init; }
}

public class GetWeeklyAnalyticsQueryHandler : IRequestHandler<GetWeeklyAnalyticsQuery, List<WeeklyAnalyticsDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public GetWeeklyAnalyticsQueryHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<List<WeeklyAnalyticsDto>> Handle(GetWeeklyAnalyticsQuery request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        var weeks = request.Weeks ?? 4;
        var endDate = DateTimeOffset.UtcNow.Date;
        var startDate = endDate.AddDays(-(weeks * 7));

        var events = await _context.BusinessEvents
            .Where(e => e.BusinessId == _businessContext.CurrentBusinessId!.Value
                && e.OccurredAt >= startDate
                && e.OccurredAt <= endDate)
            .ToListAsync(cancellationToken);

        var results = new List<WeeklyAnalyticsDto>();

        for (int i = 0; i < weeks; i++)
        {
            var weekStart = endDate.AddDays(-(i + 1) * 7);
            var weekEnd = endDate.AddDays(-i * 7);

            var weekEvents = events.Where(e => e.OccurredAt >= weekStart && e.OccurredAt < weekEnd).ToList();

            decimal salesAmount = 0;
            foreach (var saleEvent in weekEvents.Where(e => e.EventType == BusinessEventType.PosSale))
            {
                if (!string.IsNullOrEmpty(saleEvent.EventData))
                {
                    try
                    {
                        using var doc = System.Text.Json.JsonDocument.Parse(saleEvent.EventData);
                        if (doc.RootElement.TryGetProperty("TotalAmount", out var amountElement))
                        {
                            if (amountElement.ValueKind == System.Text.Json.JsonValueKind.Number)
                            {
                                salesAmount += amountElement.GetDecimal();
                            }
                        }
                    }
                    catch
                    {
                        // Skip invalid JSON
                    }
                }
            }

            results.Add(new WeeklyAnalyticsDto
            {
                WeekStart = weekStart,
                WeekEnd = weekEnd,
                Logins = weekEvents.Count(e => e.EventType == BusinessEventType.Login),
                PosSales = weekEvents.Count(e => e.EventType == BusinessEventType.PosSale),
                SalesAmount = salesAmount,
                StockAlertsResolved = weekEvents.Count(e => e.EventType == BusinessEventType.StockAlertResolved),
                StockOuts = weekEvents.Count(e => e.EventType == BusinessEventType.StockOut)
            });
        }

        return results.OrderBy(r => r.WeekStart).ToList();
    }
}

