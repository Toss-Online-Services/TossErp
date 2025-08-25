using Crm.Application.DTOs;
using Crm.Domain.Entities;

namespace Crm.Application.Interfaces;

public interface IAnalyticsRepository
{
    Task<CustomerAnalyticsData> GetCustomerAnalyticsDataAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<List<CustomerSegmentData>> GetCustomerSegmentsAsync(CancellationToken cancellationToken = default);
    Task<SalesTrendsDto> GetSalesTrendsAsync(DateTime startDate, DateTime endDate, string groupBy, CancellationToken cancellationToken = default);
}

public interface ISalesRepository
{
    Task<List<SalesTrendData>> GetSalesTrendsDataAsync(DateTime startDate, DateTime endDate, string groupBy, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalSalesAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<int> GetTotalTransactionsAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<decimal> GetAverageOrderValueAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<decimal> GetGrowthRateAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<List<SeasonalityData>> GetSeasonalityDataAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
}
