using Catalog.Domain.Entities;

namespace Catalog.API.Services;

public interface IProductTagService
{
    Task<ProductTag?> GetProductTagByIdAsync(int productTagId);
    Task<IEnumerable<ProductTag>> GetAllProductTagsAsync();
    Task<IEnumerable<ProductTag>> GetProductTagsByProductIdAsync(int productId);
    Task<IEnumerable<ProductTag>> GetProductTagsByNameAsync(string name);
    Task<ProductTag> InsertProductTagAsync(ProductTag productTag);
    Task<ProductTag> UpdateProductTagAsync(ProductTag productTag);
    Task DeleteProductTagAsync(ProductTag productTag);
    Task DeleteProductTagsAsync(IList<ProductTag> productTags);
    Task<IList<ProductTag>> GetAllProductTagsAsync(bool showHidden = false);
    Task<IList<ProductTag>> GetProductTagsByProductIdAsync(int productId, bool showHidden = false);
    Task<IList<ProductTag>> GetProductTagsByNameAsync(string name, bool showHidden = false);
    Task<IList<ProductTag>> GetAllProductTagsAsync(bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue);
    Task<IList<ProductTag>> GetProductTagsByProductIdAsync(int productId, bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue);
    Task<IList<ProductTag>> GetProductTagsByNameAsync(string name, bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue);
    Task<int> GetProductCountByProductTagIdAsync(int productTagId, bool showHidden = false);
    Task<IList<Product>> GetProductsByProductTagIdAsync(int productTagId, bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue);
    Task<IList<ProductTag>> GetProductTagsByProductIdAsync(int productId, bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue, bool includeDeleted = false);
    Task<IList<ProductTag>> GetProductTagsByProductIdAsync(int productId, bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue, bool includeDeleted = false, int? storeId = null);
    Task<IList<ProductTag>> GetProductTagsByProductIdAsync(int productId, bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue, bool includeDeleted = false, int? storeId = null, int? languageId = null);
} 
