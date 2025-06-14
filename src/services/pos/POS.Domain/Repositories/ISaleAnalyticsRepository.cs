using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TossErp.POS.Domain.AggregatesModel.SaleAggregate;

namespace TossErp.POS.Domain.Repositories
{
    public interface ISaleAnalyticsRepository
    {
        Task<decimal> GetTotalSalesAsync(int storeId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
        Task<decimal> GetTotalSalesByPaymentMethodAsync(int storeId, PaymentMethod method, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
        Task<decimal> GetTotalSalesByProductAsync(int storeId, int productId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
        Task<decimal> GetTotalSalesByStaffAsync(int storeId, int staffId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
        Task<IDictionary<DateTime, decimal>> GetDailySalesAsync(int storeId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
        Task<IDictionary<string, decimal>> GetSalesByCategoryAsync(int storeId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    }
} 
