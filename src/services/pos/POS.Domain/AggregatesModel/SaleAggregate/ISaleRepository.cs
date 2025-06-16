using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.SaleAggregate;

public interface ISaleRepository : IRepository<Sale>
{
    Task<Sale?> GetByIdAsync(Guid id);
    Task<IEnumerable<Sale>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<Sale>> GetByStaffIdAsync(Guid staffId);
    Task<Sale> AddAsync(Sale sale);
    Task UpdateAsync(Sale sale);
    Task DeleteAsync(Guid id);
} 
