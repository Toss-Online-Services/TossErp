using TossErp.Application.DTOs;

namespace TossErp.Application.Services;

public interface ISaleService
{
    Task<SaleDto> GetByIdAsync(Guid id);
    Task<IEnumerable<SaleDto>> GetAllAsync();
    Task<IEnumerable<SaleDto>> GetByBusinessAsync(Guid businessId);
    Task<IEnumerable<SaleDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<SaleDto> CreateAsync(CreateSaleDto createSaleDto);
    Task<SaleDto> CompleteSaleAsync(Guid id);
    Task<SaleDto> CancelSaleAsync(Guid id, string reason = "");
    Task<decimal> GetTodaySalesAsync();
    Task<int> GetTodayOrderCountAsync();
    Task<IEnumerable<SaleDto>> GetRecentSalesAsync(int count);
    Task<IEnumerable<SalesSummaryDto>> GetSalesDataForLast7DaysAsync();
} 
