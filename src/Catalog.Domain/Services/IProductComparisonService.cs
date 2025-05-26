using Catalog.Domain.DTOs;

namespace Catalog.Domain.Services;

public interface IProductComparisonService
{
    Task<IEnumerable<CatalogItemDto>> GetComparisonItemsAsync(string userId);
    Task AddToComparisonAsync(string userId, int catalogItemId);
    Task RemoveFromComparisonAsync(string userId, int catalogItemId);
    Task ClearComparisonAsync(string userId);
    Task<bool> IsInComparisonAsync(string userId, int catalogItemId);
    Task<int> GetComparisonCountAsync(string userId);
} 
