using Catalog.Domain.DTOs;

namespace Catalog.Domain.Services;

public interface IRecommendationService
{
    Task<IEnumerable<CatalogItemDto>> GetPersonalizedRecommendationsAsync(string userId, int count = 5);
    Task<IEnumerable<CatalogItemDto>> GetRelatedItemsAsync(int catalogItemId, int count = 4);
    Task<IEnumerable<CatalogItemDto>> GetFrequentlyBoughtTogetherAsync(int catalogItemId, int count = 4);
    Task<IEnumerable<CatalogItemDto>> GetTrendingItemsAsync(int count = 5);
    Task<IEnumerable<CatalogItemDto>> GetNewArrivalsAsync(int count = 5);
} 
