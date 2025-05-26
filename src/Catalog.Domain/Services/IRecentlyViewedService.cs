using Catalog.Domain.DTOs;

namespace Catalog.Domain.Services;

public interface IRecentlyViewedService
{
    Task<IEnumerable<CatalogItemDto>> GetRecentlyViewedAsync(string userId, int count = 5);
    Task AddToRecentlyViewedAsync(string userId, int catalogItemId);
    Task ClearRecentlyViewedAsync(string userId);
} 
