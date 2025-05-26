using Catalog.Domain.Entities;

namespace Catalog.Domain.Repositories;

public interface IProductTagRepository
{
    Task<ProductTag> GetAsync(int tagId);
    Task<IEnumerable<ProductTag>> GetAllAsync();
    Task<IEnumerable<ProductTag>> GetByProductAsync(int productId);
    Task<ProductTag> GetByNameAsync(string name);
    Task<ProductTag> AddAsync(ProductTag tag);
    Task UpdateAsync(ProductTag tag);
    Task DeleteAsync(int tagId);
    Task<bool> ExistsAsync(int tagId);
    Task<bool> ExistsByNameAsync(string name);
    Task<int> GetTotalCountAsync();
} 
