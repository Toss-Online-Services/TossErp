#nullable enable
using eShop.POS.Domain.SeedWork;

namespace eShop.POS.Domain.Repositories;

public interface IRepository<T> where T : class, IAggregateRoot
{
    Task<T?> GetByIdAsync(string id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<bool> ExistsAsync(string id);
} 
