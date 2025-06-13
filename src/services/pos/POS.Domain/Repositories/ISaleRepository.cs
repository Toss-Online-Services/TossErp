#nullable enable
using eShop.POS.Domain.AggregatesModel.SaleAggregate;

namespace eShop.POS.Domain.Repositories;

public interface ISaleRepository : IRepository<Sale>
{
    Task<Sale?> GetAsync(int saleId);
    Task<IEnumerable<Sale>> GetByStoreAsync(string storeId, DateTime startDate, DateTime endDate);
    Task<IEnumerable<Sale>> GetByStaffAsync(string staffId, DateTime startDate, DateTime endDate);
    Task<IEnumerable<Sale>> GetOfflineSalesAsync();
    Task<IEnumerable<Sale>> GetByCustomerAsync(string customerId);
    Task DeleteAsync(int saleId);
} 
