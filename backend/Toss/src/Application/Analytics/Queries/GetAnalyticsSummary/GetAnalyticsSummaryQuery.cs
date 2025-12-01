using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Analytics.Queries.GetAnalyticsSummary;

public record GetAnalyticsSummaryQuery : IRequest<AnalyticsSummaryDto>
{
    public DateTimeOffset? FromDate { get; init; }
    public DateTimeOffset? ToDate { get; init; }
}

public record AnalyticsSummaryDto
{
    public int TotalLogins { get; init; }
    public int TotalPosSales { get; init; }
    public decimal TotalSalesAmount { get; init; }
    public int StockAlertsResolved { get; init; }
    public int StockOuts { get; init; }
    public Dictionary<string, int> ModuleUsage { get; init; } = new();
    public Dictionary<BusinessEventType, int> EventTypeCounts { get; init; } = new();
}

public class GetAnalyticsSummaryQueryHandler : IRequestHandler<GetAnalyticsSummaryQuery, AnalyticsSummaryDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public GetAnalyticsSummaryQueryHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<AnalyticsSummaryDto> Handle(GetAnalyticsSummaryQuery request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        var fromDate = request.FromDate ?? DateTimeOffset.UtcNow.AddDays(-7); // Default to last 7 days
        var toDate = request.ToDate ?? DateTimeOffset.UtcNow;

        var query = _context.BusinessEvents
            .Where(e => e.BusinessId == _businessContext.CurrentBusinessId!.Value
                && e.OccurredAt >= fromDate
                && e.OccurredAt <= toDate);

        var events = await query.ToListAsync(cancellationToken);

        var summary = new AnalyticsSummaryDto
        {
            TotalLogins = events.Count(e => e.EventType == BusinessEventType.Login),
            TotalPosSales = events.Count(e => e.EventType == BusinessEventType.PosSale),
            StockAlertsResolved = events.Count(e => e.EventType == BusinessEventType.StockAlertResolved),
            StockOuts = events.Count(e => e.EventType == BusinessEventType.StockOut),
            ModuleUsage = events
                .Where(e => !string.IsNullOrEmpty(e.Module))
                .GroupBy(e => e.Module!)
                .ToDictionary(g => g.Key, g => g.Count()),
            EventTypeCounts = events
                .GroupBy(e => e.EventType)
                .ToDictionary(g => g.Key, g => g.Count())
        };

        // Calculate total sales amount from POS sale events
        decimal totalSales = 0;
        foreach (var saleEvent in events.Where(e => e.EventType == BusinessEventType.PosSale))
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
                            totalSales += amountElement.GetDecimal();
                        }
                    }
                }
                catch
                {
                    // Skip invalid JSON
                }
            }
        }

        return summary with { TotalSalesAmount = totalSales };
    }
}

