using POS.Domain.AggregatesModel.BuyerAggregate;
using POS.Domain.AggregatesModel.SaleAggregate;
using POS.Domain.Enums;

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
        Task<decimal> GetTotalSalesByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
        Task<decimal> GetTotalSalesByStoreAsync(Guid storeId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
        Task<IEnumerable<Sale>> GetTopSellingProductsAsync(int count, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
        Task RecordSaleCompleted(string storeId, string staffId, decimal total, bool isOffline, DateTime completedAt, CancellationToken cancellationToken = default);
        Task RecordSaleRefunded(string storeId, string staffId, decimal amount, string reason, DateTime refundedAt, CancellationToken cancellationToken = default);
        Task RecordPayment(string storeId, string staffId, PaymentMethod method, decimal amount, DateTime paymentDate, CancellationToken cancellationToken = default);
        Task RecordDiscount(string storeId, string staffId, DiscountType type, decimal amount, DateTime createdAt, CancellationToken cancellationToken = default);
        Task RecordSaleSynced(string storeId, string staffId, DateTime syncedAt, CancellationToken cancellationToken = default);
        Task<decimal> GetTotalSalesAsync(string storeId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
        Task<decimal> GetTotalRefundsAsync(string storeId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
        Task<IDictionary<PaymentMethod, decimal>> GetPaymentMethodTotalsAsync(string storeId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
        Task<IDictionary<string, decimal>> GetStaffPerformanceAsync(string storeId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
        Task<IDictionary<string, int>> GetProductSalesCountAsync(string storeId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
        Task<IDictionary<string, decimal>> GetProductSalesRevenueAsync(string storeId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    }
} 
