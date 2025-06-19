using TossErp.Shared.DTOs;

namespace TossErp.POS.API.Services
{
    public interface ISalesService
    {
        Task<SaleDto> CreateSaleAsync(CreateSaleDto request);
        Task<SaleDto?> GetSaleByIdAsync(Guid id);
        Task<List<SaleDto>> GetSalesAsync(DateTime? fromDate, DateTime? toDate, int page, int pageSize);
        Task<SaleDto?> CompleteSaleAsync(Guid id, CompleteSaleDto request);
        Task<SaleDto?> CancelSaleAsync(Guid id, CancelSaleDto request);
        Task<DailySummaryDto> GetDailySummaryAsync(DateTime date);
        Task<SalesReportDto> GetSalesReportAsync(DateTime fromDate, DateTime toDate);
    }
} 
