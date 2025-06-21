using TossErp.Domain.SeedWork;

namespace TossErp.Domain.AggregatesModel.SaleAggregate;

public interface ISaleRepository : IRepository<Sale>
{
    Task<Sale?> GetByIdAsync(Guid id);
    Task<Sale?> GetBySaleNumberAsync(string saleNumber, Guid businessId);
    Task<IEnumerable<Sale>> GetByBusinessIdAsync(Guid businessId, DateTime? fromDate = null, DateTime? toDate = null);
    Task<IEnumerable<Sale>> GetByCustomerIdAsync(Guid customerId, Guid businessId);
    Task<IEnumerable<Sale>> GetOfflineSalesAsync(Guid businessId);
    Task<IEnumerable<Sale>> GetByDateRangeAsync(Guid businessId, DateTime fromDate, DateTime toDate);
    Task<decimal> GetTotalSalesAsync(Guid businessId, DateTime fromDate, DateTime toDate);
    Task<int> GetSalesCountAsync(Guid businessId, DateTime fromDate, DateTime toDate);
    Task<bool> ExistsAsync(Guid id);
} 
