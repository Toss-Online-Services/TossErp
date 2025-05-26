using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using Catalog.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IProductAttributeParser _productAttributeParser;
    private readonly IProductAttributeService _productAttributeService;
    private readonly IProductAttributeFormatter _productAttributeFormatter;

    public ProductService(
        IProductRepository productRepository,
        IProductAttributeParser productAttributeParser,
        IProductAttributeService productAttributeService,
        IProductAttributeFormatter productAttributeFormatter)
    {
        _productRepository = productRepository;
        _productAttributeParser = productAttributeParser;
        _productAttributeService = productAttributeService;
        _productAttributeFormatter = productAttributeFormatter;
    }

    public async Task<Product> GetProductByIdAsync(int productId)
    {
        return await _productRepository.GetAsync(productId);
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        return await _productRepository.GetAllAsync();
    }

    public async Task<IEnumerable<Product>> GetAllProductsDisplayedOnHomepageAsync()
    {
        return await _productRepository.GetAllAsync(query =>
            query.Where(p => p.Published && !p.Deleted && p.ShowOnHomepage)
                 .OrderBy(p => p.DisplayOrder)
                 .ThenBy(p => p.Id));
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
    {
        return await _productRepository.GetAllAsync(query =>
            query.Where(p => p.Categories.Any(c => c.Id == categoryId) && p.Published && !p.Deleted)
                 .OrderBy(p => p.DisplayOrder)
                 .ThenBy(p => p.Id));
    }

    public async Task<IEnumerable<Product>> GetProductsByManufacturerAsync(int manufacturerId)
    {
        return await _productRepository.GetAllAsync(query =>
            query.Where(p => p.Manufacturers.Any(m => m.Id == manufacturerId) && p.Published && !p.Deleted)
                 .OrderBy(p => p.DisplayOrder)
                 .ThenBy(p => p.Id));
    }

    public async Task<IEnumerable<Product>> GetProductsByTagAsync(int tagId)
    {
        return await _productRepository.GetAllAsync(query =>
            query.Where(p => p.Tags.Any(t => t.Id == tagId) && p.Published && !p.Deleted)
                 .OrderBy(p => p.DisplayOrder)
                 .ThenBy(p => p.Id));
    }

    public async Task<Product> GetProductBySkuAsync(string sku)
    {
        if (string.IsNullOrEmpty(sku))
            return null;

        return await _productRepository.GetAllAsync(query =>
            query.Where(p => !p.Deleted && p.Sku == sku.Trim())
                 .FirstOrDefaultAsync());
    }

    public async Task<Product> CreateProductAsync(Product product)
    {
        return await _productRepository.AddAsync(product);
    }

    public async Task UpdateProductAsync(Product product)
    {
        await _productRepository.UpdateAsync(product);
    }

    public async Task DeleteProductAsync(int productId)
    {
        await _productRepository.DeleteAsync(productId);
    }

    public async Task DeleteProductsAsync(IEnumerable<Product> products)
    {
        await _productRepository.DeleteRangeAsync(products);
    }

    public async Task<IEnumerable<Product>> SearchProductsAsync(
        string keywords = null,
        int? categoryId = null,
        int? manufacturerId = null,
        int? tagId = null,
        decimal? priceMin = null,
        decimal? priceMax = null,
        bool searchDescriptions = false,
        bool searchSku = true,
        bool searchTags = false,
        bool showHidden = false)
    {
        var query = _productRepository.GetAll();

        if (!showHidden)
            query = query.Where(p => p.Published);

        query = query.Where(p => !p.Deleted);

        if (!string.IsNullOrEmpty(keywords))
        {
            query = query.Where(p =>
                p.Name.Contains(keywords) ||
                (searchDescriptions && (p.ShortDescription.Contains(keywords) || p.FullDescription.Contains(keywords))) ||
                (searchSku && p.Sku == keywords) ||
                (searchTags && p.Tags.Any(t => t.Name.Contains(keywords))));
        }

        if (categoryId.HasValue)
            query = query.Where(p => p.Categories.Any(c => c.Id == categoryId.Value));

        if (manufacturerId.HasValue)
            query = query.Where(p => p.Manufacturers.Any(m => m.Id == manufacturerId.Value));

        if (tagId.HasValue)
            query = query.Where(p => p.Tags.Any(t => t.Id == tagId.Value));

        if (priceMin.HasValue)
            query = query.Where(p => p.Price >= priceMin.Value);

        if (priceMax.HasValue)
            query = query.Where(p => p.Price <= priceMax.Value);

        return await query.OrderBy(p => p.DisplayOrder).ThenBy(p => p.Id).ToListAsync();
    }

    public async Task<bool> ProductIsAvailableAsync(Product product, DateTime? dateTime = null)
    {
        if (product == null)
            return false;

        dateTime ??= DateTime.UtcNow;

        if (product.AvailableStartDateTimeUtc.HasValue && product.AvailableStartDateTimeUtc.Value > dateTime)
            return false;

        if (product.AvailableEndDateTimeUtc.HasValue && product.AvailableEndDateTimeUtc.Value < dateTime)
            return false;

        return true;
    }

    public async Task<string> FormatAttributesAsync(Product product, string attributesXml, bool includePrices = true)
    {
        return await _productAttributeFormatter.FormatAttributesAsync(product, attributesXml, includePrices: includePrices);
    }

    public async Task<IList<ProductAttribute>> ParseProductAttributesAsync(string attributesXml)
    {
        return await _productAttributeParser.ParseProductAttributesAsync(attributesXml);
    }

    public async Task<IList<ProductAttributeValue>> ParseProductAttributeValuesAsync(string attributesXml)
    {
        return await _productAttributeParser.ParseProductAttributeValuesAsync(attributesXml);
    }

    public async Task<string> AddProductAttributeAsync(string attributesXml, ProductAttribute productAttribute, string value)
    {
        return await _productAttributeParser.AddProductAttributeAsync(attributesXml, productAttribute, value);
    }

    public async Task<string> RemoveProductAttributeAsync(string attributesXml, ProductAttribute productAttribute)
    {
        return await _productAttributeParser.RemoveProductAttributeAsync(attributesXml, productAttribute);
    }

    public async Task<bool> AreProductAttributesEqualAsync(string attributesXml1, string attributesXml2)
    {
        return await _productAttributeParser.AreProductAttributesEqualAsync(attributesXml1, attributesXml2);
    }

    public async Task<bool> IsConditionMetAsync(Product product, string attributesXml)
    {
        return await _productAttributeParser.IsConditionMetAsync(product, attributesXml);
    }
} 
