#nullable enable
using POS.Domain.AggregatesModel.ProductAggregate;

namespace POS.Domain.Repositories;

public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetAsync(string productId);
    Task<IEnumerable<Product>> GetByStoreAsync(string storeId);
    Task<IEnumerable<Product>> GetByCategoryAsync(string storeId, string category);
    Task<IEnumerable<Product>> GetLowStockAsync(string storeId, int threshold);
    Task<bool> ExistsAsync(string productId);
} 
