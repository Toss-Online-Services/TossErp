using Catalog.Domain.Entities;

namespace Catalog.API.Services;

public interface IRecentlyViewedProductsService
{
    Task<IList<Product>> GetRecentlyViewedProductsAsync(int customerId, int number);
    Task AddProductToRecentlyViewedListAsync(int customerId, int productId);
    Task ClearRecentlyViewedProductsAsync(int customerId);
    Task<IList<Product>> GetRecentlyViewedProductsAsync(int customerId, int number, bool showHidden = false);
    Task<IList<Product>> GetRecentlyViewedProductsAsync(int customerId, int number, bool showHidden = false, int? storeId = null);
    Task<IList<Product>> GetRecentlyViewedProductsAsync(int customerId, int number, bool showHidden = false, int? storeId = null, int? languageId = null);
    Task<IList<Product>> GetRecentlyViewedProductsAsync(int customerId, int number, bool showHidden = false, int? storeId = null, int? languageId = null, int pageIndex = 0, int pageSize = int.MaxValue);
} 
