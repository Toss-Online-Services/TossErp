using Catalog.Domain.Entities;

namespace Catalog.Domain.Interfaces;

public interface IProductAttributeValueRepository
{
    Task<ProductAttributeValue?> GetAsync(int id);
    Task<IEnumerable<ProductAttributeValue?>> GetAllAsync();
    Task<IEnumerable<ProductAttributeValue?>> GetByProductAsync(int productId);
    Task<IEnumerable<ProductAttributeValue?>> GetByAttributeAsync(int attributeId);
    Task<ProductAttributeValue?> GetByProductAndAttributeAsync(int productId, int attributeId);
    Task<ProductAttributeValue> AddAsync(ProductAttributeValue value);
    Task<ProductAttributeValue> UpdateAsync(ProductAttributeValue value);
    Task DeleteAsync(int valueId);
    Task<bool> ExistsAsync(int valueId);
    Task<bool> ExistsByNameAsync(int productId, int attributeId, string name);
    Task<int> GetTotalCountAsync();
    Task<int> GetNextDisplayOrderAsync(int productId, int attributeId);
} 
