using Catalog.Domain.Entities;

namespace Catalog.API.Services;

public interface IProductAttributeService
{
    Task<ProductAttribute?> GetProductAttributeByIdAsync(int productAttributeId);
    Task<IEnumerable<ProductAttribute>> GetAllProductAttributesAsync();
    Task<IEnumerable<ProductAttribute>> GetProductAttributesByIdsAsync(int[] productAttributeIds);
    Task<IEnumerable<ProductAttribute>> GetProductAttributesByNameAsync(string name);
    Task<IEnumerable<ProductAttribute>> GetProductAttributesByDescriptionAsync(string description);
    Task<ProductAttribute> InsertProductAttributeAsync(ProductAttribute productAttribute);
    Task<ProductAttribute> UpdateProductAttributeAsync(ProductAttribute productAttribute);
    Task DeleteProductAttributeAsync(ProductAttribute productAttribute);
    Task DeleteProductAttributesAsync(IList<ProductAttribute> productAttributes);
    Task<IList<ProductAttribute>> GetAllProductAttributesAsync(bool showHidden = false);
    Task<IList<ProductAttribute>> GetProductAttributesByIdsAsync(int[] productAttributeIds, bool showHidden = false);
    Task<IList<ProductAttribute>> GetProductAttributesByNameAsync(string name, bool showHidden = false);
    Task<IList<ProductAttribute>> GetProductAttributesByDescriptionAsync(string description, bool showHidden = false);
    Task<IList<ProductAttribute>> GetAllProductAttributesAsync(bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue);
    Task<IList<ProductAttribute>> GetProductAttributesByIdsAsync(int[] productAttributeIds, bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue);
    Task<IList<ProductAttribute>> GetProductAttributesByNameAsync(string name, bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue);
    Task<IList<ProductAttribute>> GetProductAttributesByDescriptionAsync(string description, bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue);
} 
