using Catalog.Domain.AggregatesModel.CatalogAggregate;

namespace Catalog.Domain.Interfaces;

public interface ICatalogRepository
{
    Task<IEnumerable<CatalogItem>> GetProductsAsync();
    Task<CatalogItem?> GetProductByIdAsync(int id);
    Task<IEnumerable<CatalogItem>> GetProductsByBrandAsync(int brandId);
    Task<IEnumerable<CatalogItem>> GetProductsByTypeAsync(int typeId);
    Task<IEnumerable<CatalogBrand>> GetBrandsAsync();
    Task<IEnumerable<CatalogType>> GetTypesAsync();
    Task<CatalogItem> AddProductAsync(CatalogItem product);
    Task<CatalogItem> UpdateProductAsync(CatalogItem product);
    Task DeleteProductAsync(int id);
} 
