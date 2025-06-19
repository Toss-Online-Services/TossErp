using TossErp.Domain.SeedWork;
using TossErp.POS.Domain.AggregatesModel.SaleAggregate;
using TossErp.POS.Domain.Enums;

namespace TossErp.POS.Infrastructure.Repositories
{
    public interface ISaleRepository : IRepository<Sale>
    {
        Task<Sale?> GetBySaleNumberAsync(string saleNumber);
        Task<IEnumerable<Sale>> GetSalesByCustomerAsync(Guid customerId);
        Task<IEnumerable<Sale>> GetSalesByCashierAsync(Guid cashierId);
        Task<IEnumerable<Sale>> GetSalesByStatusAsync(SaleStatus status);
        Task<IEnumerable<Sale>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Sale>> GetCompletedSalesAsync();
        Task<decimal> GetTotalSalesForPeriodAsync(DateTime startDate, DateTime endDate);
        Task<bool> SaleNumberExistsAsync(string saleNumber);
    }
} 
