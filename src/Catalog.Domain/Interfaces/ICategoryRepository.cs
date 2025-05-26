using Catalog.Domain.Entities;

namespace Catalog.Domain.Interfaces;

public interface ICategoryRepository
{
    Task<Category?> GetAsync(int id);
    Task<IEnumerable<Category?>> GetAllAsync();
    Task<IEnumerable<Category?>> GetByParentAsync(int parentId);
    Task<IEnumerable<Category?>> GetPublishedAsync();
    Task<Category> AddAsync(Category category);
    Task<Category> UpdateAsync(Category category);
    Task DeleteAsync(int categoryId);
    Task<bool> ExistsAsync(int categoryId);
    Task<int> GetTotalCountAsync();
} 
