#nullable enable
using POS.Domain.AggregatesModel.SaleAggregate;
using POS.Domain.Repositories;

namespace POS.Domain.Repositories;

public interface ISaleRepository : IRepository<Sale>
{
    Task<Sale?> GetByIdAsync(Guid id);
    Task<IEnumerable<Sale>> GetByStoreIdAsync(Guid storeId);
    Task<IEnumerable<Sale>> GetByStaffIdAsync(Guid staffId);
    Task<IEnumerable<Sale>> GetByCustomerIdAsync(Guid customerId);
    Task<IEnumerable<Sale>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task AddAsync(Sale sale);
    void Update(Sale sale);
    void Delete(Sale sale);
    Task<IEnumerable<Sale>> GetByStoreAsync(Guid storeId, DateTime startDate, DateTime endDate);
    Task<IEnumerable<Sale>> GetByStaffAsync(Guid staffId, DateTime startDate, DateTime endDate);
    Task<IEnumerable<Sale>> GetOfflineSalesAsync();
    Task<IEnumerable<Sale>> GetAllAsync();
    Task<bool> ExistsAsync(Guid id);
    Task<decimal> GetTotalSalesByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<decimal> GetTotalSalesByStoreAsync(Guid storeId);
    Task<decimal> GetTotalSalesByStaffAsync(Guid staffId);
} 
