using eShop.POS.Domain.Repositories;

namespace eShop.POS.Domain.AggregatesModel.SaleAggregate;

public interface ISaleRepository : IRepository<Sale>
{
    Task<Sale?> GetByIdAsync(string id);
    Task<IEnumerable<Sale>> GetByStoreIdAsync(string storeId);
    Task<IEnumerable<Sale>> GetByStaffIdAsync(string staffId);
    Task<IEnumerable<Sale>> GetByCustomerIdAsync(string customerId);
    Task<IEnumerable<Sale>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
} 
