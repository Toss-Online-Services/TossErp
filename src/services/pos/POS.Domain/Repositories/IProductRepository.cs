#nullable enable
using eShop.POS.Domain.AggregatesModel.ProductAggregate;
using eShop.POS.Domain.Repositories;

namespace eShop.POS.Domain.Repositories;

public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetAsync(string productId);
    Task<IEnumerable<Product>> GetByStoreAsync(string storeId);
    Task<IEnumerable<Product>> GetByCategoryAsync(string storeId, string category);
    Task<IEnumerable<Product>> GetLowStockAsync(string storeId, int threshold);
    Task<IEnumerable<Product>> GetByStoreIdAsync(string storeId);
    Task<IEnumerable<Product>> GetByBrandAsync(string brand);
    Task<IEnumerable<Product>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice);
    Task<IEnumerable<Product>> GetByStockLevelAsync(int minStock);
} 
