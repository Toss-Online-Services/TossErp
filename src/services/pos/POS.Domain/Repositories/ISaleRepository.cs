#nullable enable
using POS.Domain.AggregatesModel.SaleAggregate;
using POS.Domain.Repositories;

namespace POS.Domain.Repositories;

public interface ISaleRepository : IRepository<Sale>
{
    Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Sale>> GetByStoreAsync(Guid storeId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Sale>> GetByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Sale>> GetByStaffAsync(Guid staffId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Sale>> GetByDateRangeAsync(Guid storeId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalSalesByDateRangeAsync(Guid storeId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<Sale>> GetDraftSalesAsync(Guid storeId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Sale>> GetByStoreIdAsync(Guid storeId);
    Task<IEnumerable<Sale>> GetByStaffIdAsync(Guid staffId);
    Task AddAsync(Sale sale);
    void Update(Sale sale);
    void Delete(Sale sale);
    Task<IEnumerable<Sale>> GetByStoreAsync(Guid storeId, DateTime startDate, DateTime endDate);
    Task<IEnumerable<Sale>> GetByStaffAsync(Guid staffId, DateTime startDate, DateTime endDate);
    Task<IEnumerable<Sale>> GetOfflineSalesAsync();
    Task<IEnumerable<Sale>> GetAllAsync();
    Task<bool> ExistsAsync(Guid id);
    Task<decimal> GetTotalSalesByStoreAsync(Guid storeId);
    Task<decimal> GetTotalSalesByStaffAsync(Guid staffId);
} 
