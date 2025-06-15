using POS.Domain.AggregatesModel.SaleAggregate;

namespace POS.Domain.Repositories
{
    public interface ISaleAnalyticsRepository
    {
        Task<decimal> GetTotalSalesAsync(CancellationToken cancellationToken = default);
        Task<decimal> GetTotalSalesByPaymentMethodAsync(PaymentType method, CancellationToken cancellationToken = default);
        Task<decimal> GetTotalSalesByProductAsync(int productId, CancellationToken cancellationToken = default);
        Task<decimal> GetTotalSalesByStaffAsync(int staffId, CancellationToken cancellationToken = default);
        Task<IEnumerable<dynamic>> GetDailySalesAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<dynamic>> GetSalesByCategoryAsync(CancellationToken cancellationToken = default);
    }
} 
