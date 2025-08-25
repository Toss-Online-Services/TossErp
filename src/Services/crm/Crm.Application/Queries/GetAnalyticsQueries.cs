using Crm.Application.DTOs;
using Crm.Application.Interfaces;
using MediatR;

namespace Crm.Application.Queries;

public class GetCustomerSegmentsQuery : IRequest<List<CustomerSegmentDto>>
{
}

public class GetCustomerSegmentsQueryHandler : IRequestHandler<GetCustomerSegmentsQuery, List<CustomerSegmentDto>>
{
    private readonly IAnalyticsRepository _analyticsRepository;

    public GetCustomerSegmentsQueryHandler(IAnalyticsRepository analyticsRepository)
    {
        _analyticsRepository = analyticsRepository;
    }

    public async Task<List<CustomerSegmentDto>> Handle(GetCustomerSegmentsQuery request, CancellationToken cancellationToken)
    {
        var segments = await _analyticsRepository.GetCustomerSegmentsAsync(cancellationToken);
        
        return segments.Select(s => new CustomerSegmentDto
        {
            Segment = s.Segment.ToString(),
            Count = s.Count,
            TotalSpent = s.TotalSpent,
            AverageSpent = s.Count > 0 ? s.TotalSpent / s.Count : 0,
            Percentage = s.Percentage
        }).ToList();
    }
}

public class GetSalesTrendsQuery : IRequest<SalesTrendsDto>
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string GroupBy { get; set; } = "month"; // day, week, month, quarter, year
}

public class GetSalesTrendsQueryHandler : IRequestHandler<GetSalesTrendsQuery, SalesTrendsDto>
{
    private readonly ISalesRepository _salesRepository;

    public GetSalesTrendsQueryHandler(ISalesRepository salesRepository)
    {
        _salesRepository = salesRepository;
    }

    public async Task<SalesTrendsDto> Handle(GetSalesTrendsQuery request, CancellationToken cancellationToken)
    {
        var trends = await _salesRepository.GetSalesTrendsDataAsync(
            request.StartDate,
            request.EndDate,
            request.GroupBy,
            cancellationToken);

        var totalSales = trends.Sum(t => t.TotalAmount);
        var totalTransactions = trends.Sum(t => t.TransactionCount);
        var averageOrderValue = totalTransactions > 0 ? totalSales / totalTransactions : 0;

        return new SalesTrendsDto
        {
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            GroupBy = request.GroupBy,
            TotalSales = totalSales,
            TotalTransactions = totalTransactions,
            AverageOrderValue = averageOrderValue,
            TrendData = trends.Select(t => new SalesTrendDataPoint
            {
                Period = t.Period,
                TotalAmount = t.TotalAmount,
                TransactionCount = t.TransactionCount,
                AverageOrderValue = t.TransactionCount > 0 ? t.TotalAmount / t.TransactionCount : 0,
                NewCustomers = t.NewCustomers,
                ReturningCustomers = t.ReturningCustomers
            }).ToList(),
            GrowthRate = CalculateGrowthRate(trends),
            Seasonality = AnalyzeSeasonality(trends)
        };
    }

    private static decimal CalculateGrowthRate(List<SalesTrendData> trends)
    {
        if (trends.Count < 2) return 0;

        var firstPeriod = trends.First().TotalAmount;
        var lastPeriod = trends.Last().TotalAmount;

        if (firstPeriod == 0) return 0;

        return ((lastPeriod - firstPeriod) / firstPeriod) * 100;
    }

    private static List<SeasonalityData> AnalyzeSeasonality(List<SalesTrendData> trends)
    {
        // Simple seasonality analysis - group by month and calculate averages
        var monthlyData = trends
            .GroupBy(t => DateTime.Parse(t.Period).Month)
            .Select(g => new SeasonalityData
            {
                Period = g.Key.ToString(),
                AverageAmount = g.Average(x => x.TotalAmount),
                PerformanceIndex = 0 // Will be calculated after we have the overall average
            })
            .ToList();

        var overallAverage = monthlyData.Average(m => m.AverageAmount);
        
        foreach (var month in monthlyData)
        {
            month.PerformanceIndex = overallAverage > 0 ? (month.AverageAmount / overallAverage) * 100 : 100;
        }

        return monthlyData;
    }
}
