using AutoMapper;
using Catalog.Domain.AggregatesModel.CatalogAggregate;
using Catalog.Domain.DTOs;
using Catalog.Domain.Interfaces;
using Catalog.Domain.Services;
using Catalog.Domain.ValueObjects;

namespace Catalog.API.Services;

public class CatalogService : ICatalogService
{
    private readonly ICatalogRepository _catalogRepository;
    private readonly ICatalogAI _catalogAI;
    private readonly IMapper _mapper;

    public CatalogService(
        ICatalogRepository catalogRepository,
        ICatalogAI catalogAI,
        IMapper mapper)
    {
        _catalogRepository = catalogRepository;
        _catalogAI = catalogAI;
        _mapper = mapper;
    }

    public async Task<CatalogItemDto?> GetCatalogItemAsync(int id)
    {
        var item = await _catalogRepository.GetProductByIdAsync(id);
        return item != null ? _mapper.Map<CatalogItemDto>(item) : null;
    }

    public async Task<IEnumerable<CatalogItemDto>> GetCatalogItemsAsync(int pageIndex, int pageSize, CatalogFilterDto filter)
    {
        var items = await _catalogRepository.GetProductsAsync();
        
        // Apply filters
        if (filter.BrandId.HasValue)
            items = items.Where(i => i.CatalogBrandId == filter.BrandId.Value);
        
        if (filter.TypeId.HasValue)
            items = items.Where(i => i.CatalogTypeId == filter.TypeId.Value);

        if (filter.MinPrice.HasValue)
            items = items.Where(i => i.Price.Amount >= filter.MinPrice.Value);

        if (filter.MaxPrice.HasValue)
            items = items.Where(i => i.Price.Amount <= filter.MaxPrice.Value);

        if (filter.InStock.HasValue && filter.InStock.Value)
            items = items.Where(i => i.AvailableStock > 0);

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var searchTerm = filter.SearchTerm.ToLower();
            items = items.Where(i => 
                i.Name.ToLower().Contains(searchTerm) || 
                i.Description.ToLower().Contains(searchTerm));
        }

        // Apply sorting
        if (!string.IsNullOrWhiteSpace(filter.SortBy))
        {
            items = filter.SortBy.ToLower() switch
            {
                "name" => filter.SortDescending 
                    ? items.OrderByDescending(i => i.Name)
                    : items.OrderBy(i => i.Name),
                "price" => filter.SortDescending
                    ? items.OrderByDescending(i => i.Price.Amount)
                    : items.OrderBy(i => i.Price.Amount),
                _ => items
            };
        }

        return _mapper.Map<IEnumerable<CatalogItemDto>>(
            items.Skip(pageIndex * pageSize).Take(pageSize));
    }

    public async Task<IEnumerable<CatalogItemDto>> GetCatalogItemsAsync(IEnumerable<int> ids)
    {
        var items = await _catalogRepository.GetProductsAsync();
        return _mapper.Map<IEnumerable<CatalogItemDto>>(
            items.Where(i => ids.Contains(i.Id)));
    }

    public async Task<IEnumerable<CatalogItemDto>> GetCatalogItemsWithSemanticRelevanceAsync(int page, int take, string text)
    {
        if (!_catalogAI.IsEnabled)
            return Enumerable.Empty<CatalogItemDto>();

        var items = await _catalogAI.SearchProductsAsync(text);
        return _mapper.Map<IEnumerable<CatalogItemDto>>(
            items.Skip(page * take).Take(take));
    }

    public async Task<IEnumerable<string>> GetBrandsAsync()
    {
        var brands = await _catalogRepository.GetBrandsAsync();
        return brands.Select(b => b.Brand);
    }

    public async Task<IEnumerable<string>> GetTypesAsync()
    {
        var types = await _catalogRepository.GetTypesAsync();
        return types.Select(t => t.Type);
    }

    public async Task<CatalogItemDto> CreateCatalogItemAsync(CatalogItemDto item)
    {
        var catalogItem = new CatalogItem(
            item.Name,
            item.Description,
            new Money(item.Price, item.Currency),
            item.PictureUri,
            item.CatalogTypeId,
            item.CatalogBrandId,
            item.AvailableStock,
            item.RestockThreshold,
            item.MaxStockThreshold);

        var createdItem = await _catalogRepository.AddProductAsync(catalogItem);
        return _mapper.Map<CatalogItemDto>(createdItem);
    }

    public async Task<CatalogItemDto> UpdateCatalogItemAsync(int id, CatalogItemDto item)
    {
        var existingItem = await _catalogRepository.GetProductByIdAsync(id);
        if (existingItem == null)
            throw new KeyNotFoundException($"Catalog item with id {id} not found");

        existingItem.UpdateDetails(
            item.Name,
            item.Description,
            new Money(item.Price, item.Currency),
            item.PictureUri,
            item.CatalogTypeId,
            item.CatalogBrandId);

        existingItem.UpdateStock(item.AvailableStock);

        var updatedItem = await _catalogRepository.UpdateProductAsync(existingItem);
        return _mapper.Map<CatalogItemDto>(updatedItem);
    }

    public async Task DeleteCatalogItemAsync(int id)
    {
        await _catalogRepository.DeleteProductAsync(id);
    }

    public async Task<IEnumerable<CatalogItemDto>> GetSimilarItemsAsync(int itemId, int count = 5)
    {
        if (!_catalogAI.IsEnabled)
            return Enumerable.Empty<CatalogItemDto>();

        var items = await _catalogAI.GetSimilarProductsAsync(itemId);
        return _mapper.Map<IEnumerable<CatalogItemDto>>(items.Take(count));
    }
} 
