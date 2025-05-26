using Catalog.Domain.DTOs;

namespace Catalog.Domain.Services;

public interface ICatalogService
{
    Task<CatalogItemDto?> GetCatalogItemAsync(int id);
    Task<IEnumerable<CatalogItemDto>> GetCatalogItemsAsync(int pageIndex, int pageSize, CatalogFilterDto filter);
    Task<IEnumerable<CatalogItemDto>> GetCatalogItemsAsync(IEnumerable<int> ids);
    Task<IEnumerable<CatalogItemDto>> GetCatalogItemsWithSemanticRelevanceAsync(int page, int take, string text);
    Task<IEnumerable<string>> GetBrandsAsync();
    Task<IEnumerable<string>> GetTypesAsync();
    Task<CatalogItemDto> CreateCatalogItemAsync(CatalogItemDto item);
    Task<CatalogItemDto> UpdateCatalogItemAsync(int id, CatalogItemDto item);
    Task DeleteCatalogItemAsync(int id);
    Task<IEnumerable<CatalogItemDto>> GetSimilarItemsAsync(int itemId, int count = 5);
} 
