namespace eShop.POS.Domain.AggregatesModel.SaleAggregate;

public interface ISaleRepository : IRepository<Sale>
{
    Task<Sale> GetAsync(int saleId);
    Task<IEnumerable<Sale>> GetByStoreAsync(string storeId, DateTime startDate, DateTime endDate);
    Task<IEnumerable<Sale>> GetByStaffAsync(string staffId, DateTime startDate, DateTime endDate);
    Task<IEnumerable<Sale>> GetOfflineSalesAsync();
    Task<IEnumerable<Sale>> GetByCustomerAsync(string customerId);
    Task<Sale> AddAsync(Sale sale);
    Task UpdateAsync(Sale sale);
    Task DeleteAsync(int saleId);
} 
