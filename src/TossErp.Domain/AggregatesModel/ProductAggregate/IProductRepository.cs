using TossErp.Domain.SeedWork;

namespace TossErp.Domain.AggregatesModel.ProductAggregate;

public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetByIdAsync(Guid id);
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByBarcodeAsync(string barcode, Guid businessId);
    Task<Product?> GetBySkuAsync(string sku, Guid businessId);
    Task<IEnumerable<Product>> GetByBusinessIdAsync(Guid businessId);
    Task<IEnumerable<Product>> GetLowStockProductsAsync(Guid businessId);
    Task<IEnumerable<Product>> GetByCategoryAsync(string category, Guid businessId);
    Task<IEnumerable<Product>> SearchAsync(string searchTerm, Guid businessId);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> BarcodeExistsAsync(string barcode, Guid businessId);
    Task<bool> SkuExistsAsync(string sku, Guid businessId);
    Task<Product> AddAsync(Product product);
    Task UpdateAsync(Product product);
} 
