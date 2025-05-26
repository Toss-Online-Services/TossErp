using AutoMapper;
using Catalog.Domain.AggregatesModel.CatalogAggregate;
using Catalog.Domain.DTOs;
using Catalog.Domain.Interfaces;
using Catalog.Domain.Services;
using Catalog.Domain.ValueObjects;
using Catalog.Domain.Entities;
using Catalog.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Services;

/// <summary>
/// Catalog service implementation
/// </summary>
public class CatalogService : ICatalogService
{
    private readonly ICatalogRepository _catalogRepository;
    private readonly ICatalogAI _catalogAI;
    private readonly IMapper _mapper;
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IManufacturerRepository _manufacturerRepository;
    private readonly IProductAttributeRepository _productAttributeRepository;
    private readonly IProductPictureRepository _productPictureRepository;
    private readonly IProductVideoRepository _productVideoRepository;
    private readonly IProductReviewRepository _productReviewRepository;
    private readonly IProductTagRepository _productTagRepository;
    private readonly IProductWarehouseInventoryRepository _productWarehouseInventoryRepository;
    private readonly IRelatedProductRepository _relatedProductRepository;
    private readonly ICrossSellProductRepository _crossSellProductRepository;
    private readonly ITierPriceRepository _tierPriceRepository;
    private readonly IDiscountProductMappingRepository _discountProductMappingRepository;
    private readonly ISpecificationAttributeRepository _specificationAttributeRepository;
    private readonly IProductTemplateRepository _productTemplateRepository;
    private readonly IProductAttributeParser _productAttributeParser;
    private readonly IProductAttributeFormatter _productAttributeFormatter;
    private readonly IProductReviewHelpfulnessRepository _productReviewHelpfulnessRepository;
    private readonly IProductStoreMappingRepository _productStoreMappingRepository;
    private readonly IProductAttributeCombinationRepository _productAttributeCombinationRepository;
    private readonly ILocalizedPropertyRepository _localizedPropertyRepository;
    private readonly IStockQuantityHistoryRepository _stockQuantityHistoryRepository;
    private readonly IProductAvailabilityRangeRepository _productAvailabilityRangeRepository;
    private readonly IProductBundleItemRepository _productBundleItemRepository;
    private readonly IBackInStockSubscriptionRepository _backInStockSubscriptionRepository;
    private readonly IDownloadRepository _downloadRepository;
    private readonly IGiftCardRepository _giftCardRepository;
    private readonly IOrderService _orderService;
    private readonly INotificationService _notificationService;

    public CatalogService(
        ICatalogRepository catalogRepository,
        ICatalogAI catalogAI,
        IMapper mapper,
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IManufacturerRepository manufacturerRepository,
        IProductAttributeRepository productAttributeRepository,
        IProductPictureRepository productPictureRepository,
        IProductVideoRepository productVideoRepository,
        IProductReviewRepository productReviewRepository,
        IProductTagRepository productTagRepository,
        IProductWarehouseInventoryRepository productWarehouseInventoryRepository,
        IRelatedProductRepository relatedProductRepository,
        ICrossSellProductRepository crossSellProductRepository,
        ITierPriceRepository tierPriceRepository,
        IDiscountProductMappingRepository discountProductMappingRepository,
        ISpecificationAttributeRepository specificationAttributeRepository,
        IProductTemplateRepository productTemplateRepository,
        IProductAttributeParser productAttributeParser,
        IProductAttributeFormatter productAttributeFormatter,
        IProductReviewHelpfulnessRepository productReviewHelpfulnessRepository,
        IProductStoreMappingRepository productStoreMappingRepository,
        IProductAttributeCombinationRepository productAttributeCombinationRepository,
        ILocalizedPropertyRepository localizedPropertyRepository,
        IStockQuantityHistoryRepository stockQuantityHistoryRepository,
        IProductAvailabilityRangeRepository productAvailabilityRangeRepository,
        IProductBundleItemRepository productBundleItemRepository,
        IBackInStockSubscriptionRepository backInStockSubscriptionRepository,
        IDownloadRepository downloadRepository,
        IGiftCardRepository giftCardRepository,
        IOrderService orderService,
        INotificationService notificationService)
    {
        _catalogRepository = catalogRepository;
        _catalogAI = catalogAI;
        _mapper = mapper;
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _manufacturerRepository = manufacturerRepository;
        _productAttributeRepository = productAttributeRepository;
        _productPictureRepository = productPictureRepository;
        _productVideoRepository = productVideoRepository;
        _productReviewRepository = productReviewRepository;
        _productTagRepository = productTagRepository;
        _productWarehouseInventoryRepository = productWarehouseInventoryRepository;
        _relatedProductRepository = relatedProductRepository;
        _crossSellProductRepository = crossSellProductRepository;
        _tierPriceRepository = tierPriceRepository;
        _discountProductMappingRepository = discountProductMappingRepository;
        _specificationAttributeRepository = specificationAttributeRepository;
        _productTemplateRepository = productTemplateRepository;
        _productAttributeParser = productAttributeParser;
        _productAttributeFormatter = productAttributeFormatter;
        _productReviewHelpfulnessRepository = productReviewHelpfulnessRepository;
        _productStoreMappingRepository = productStoreMappingRepository;
        _productAttributeCombinationRepository = productAttributeCombinationRepository;
        _localizedPropertyRepository = localizedPropertyRepository;
        _stockQuantityHistoryRepository = stockQuantityHistoryRepository;
        _productAvailabilityRangeRepository = productAvailabilityRangeRepository;
        _productBundleItemRepository = productBundleItemRepository;
        _backInStockSubscriptionRepository = backInStockSubscriptionRepository;
        _downloadRepository = downloadRepository;
        _giftCardRepository = giftCardRepository;
        _orderService = orderService;
        _notificationService = notificationService;
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

    #region Products

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

    public async Task<Product> GetProductBySkuAsync(string sku)
    {
        if (string.IsNullOrEmpty(sku))
            return null;

        return await _productRepository.GetAllAsync(query =>
            query.Where(p => !p.Deleted && p.Sku == sku.Trim())
                 .FirstOrDefaultAsync());
    }

    public async Task<IEnumerable<Product>> GetProductsBySkuAsync(string[] skuArray, int vendorId = 0)
    {
        if (skuArray == null || !skuArray.Any())
            return Enumerable.Empty<Product>();

        var query = _productRepository.GetAll()
            .Where(p => !p.Deleted && skuArray.Contains(p.Sku));

        if (vendorId > 0)
            query = query.Where(p => p.VendorId == vendorId);

        return await query.ToListAsync();
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

    public async Task<int> GetNumberOfProductsInCategoryAsync(IList<int> categoryIds = null, int storeId = 0)
    {
        var query = _productRepository.GetAll()
            .Where(p => p.Published && !p.Deleted);

        if (categoryIds != null && categoryIds.Any())
            query = query.Where(p => p.Categories.Any(c => categoryIds.Contains(c.Id)));

        if (storeId > 0)
            query = query.Where(p => p.StoreId == storeId);

        return await query.CountAsync();
    }

    public async Task<int> GetNumberOfProductsByVendorIdAsync(int vendorId)
    {
        return await _productRepository.GetAll()
            .Where(p => p.Published && !p.Deleted && p.VendorId == vendorId)
            .CountAsync();
    }

    public async Task<bool> HasAnyDownloadableProductAsync(int[] productIds)
    {
        if (productIds == null || !productIds.Any())
            return false;

        return await _productRepository.GetAll()
            .Where(p => productIds.Contains(p.Id) && p.IsDownload)
            .AnyAsync();
    }

    public async Task<bool> HasAnyGiftCardProductAsync(int[] productIds)
    {
        if (productIds == null || !productIds.Any())
            return false;

        return await _productRepository.GetAll()
            .Where(p => productIds.Contains(p.Id) && p.IsGiftCard)
            .AnyAsync();
    }

    public async Task<bool> HasAnyRecurringProductAsync(int[] productIds)
    {
        if (productIds == null || !productIds.Any())
            return false;

        return await _productRepository.GetAll()
            .Where(p => productIds.Contains(p.Id) && p.IsRecurring)
            .AnyAsync();
    }

    public async Task<string[]> GetNotExistingProductsAsync(string[] productSku)
    {
        if (productSku == null || !productSku.Any())
            return Array.Empty<string>();

        var existingSku = await _productRepository.GetAll()
            .Where(p => productSku.Contains(p.Sku))
            .Select(p => p.Sku)
            .ToListAsync();

        return productSku.Except(existingSku).ToArray();
    }

    #endregion

    #region Categories

    public async Task<Category> GetCategoryByIdAsync(int categoryId)
    {
        return await _categoryRepository.GetAsync(categoryId);
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
    {
        return await _categoryRepository.GetAllAsync();
    }

    public async Task<IEnumerable<Category>> GetCategoriesByParentIdAsync(int parentId)
    {
        return await _categoryRepository.GetAllAsync(query =>
            query.Where(c => c.ParentCategoryId == parentId)
                 .OrderBy(c => c.DisplayOrder)
                 .ThenBy(c => c.Id));
    }

    public async Task<Category> CreateCategoryAsync(Category category)
    {
        return await _categoryRepository.AddAsync(category);
    }

    public async Task UpdateCategoryAsync(Category category)
    {
        await _categoryRepository.UpdateAsync(category);
    }

    public async Task DeleteCategoryAsync(int categoryId)
    {
        await _categoryRepository.DeleteAsync(categoryId);
    }

    public async Task DeleteCategoriesAsync(IEnumerable<Category> categories)
    {
        await _categoryRepository.DeleteRangeAsync(categories);
    }

    public async Task<IEnumerable<Category>> GetCategoriesByProductIdAsync(int productId)
    {
        var product = await _productRepository.GetAsync(productId);
        return product?.Categories ?? Enumerable.Empty<Category>();
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(int categoryId, bool showHidden = false)
    {
        var query = _productRepository.GetAll()
            .Where(p => p.Categories.Any(c => c.Id == categoryId) && !p.Deleted);

        if (!showHidden)
            query = query.Where(p => p.Published);

        return await query.OrderBy(p => p.DisplayOrder).ThenBy(p => p.Id).ToListAsync();
    }

    #endregion

    #region Manufacturers

    public async Task<Manufacturer> GetManufacturerByIdAsync(int manufacturerId)
    {
        return await _manufacturerRepository.GetAsync(manufacturerId);
    }

    public async Task<IEnumerable<Manufacturer>> GetAllManufacturersAsync()
    {
        return await _manufacturerRepository.GetAllAsync();
    }

    public async Task<Manufacturer> CreateManufacturerAsync(Manufacturer manufacturer)
    {
        return await _manufacturerRepository.AddAsync(manufacturer);
    }

    public async Task UpdateManufacturerAsync(Manufacturer manufacturer)
    {
        await _manufacturerRepository.UpdateAsync(manufacturer);
    }

    public async Task DeleteManufacturerAsync(int manufacturerId)
    {
        await _manufacturerRepository.DeleteAsync(manufacturerId);
    }

    public async Task DeleteManufacturersAsync(IEnumerable<Manufacturer> manufacturers)
    {
        await _manufacturerRepository.DeleteRangeAsync(manufacturers);
    }

    public async Task<IEnumerable<Manufacturer>> GetManufacturersByProductIdAsync(int productId)
    {
        var product = await _productRepository.GetAsync(productId);
        return product?.Manufacturers ?? Enumerable.Empty<Manufacturer>();
    }

    public async Task<IEnumerable<Product>> GetProductsByManufacturerIdAsync(int manufacturerId, bool showHidden = false)
    {
        var query = _productRepository.GetAll()
            .Where(p => p.Manufacturers.Any(m => m.Id == manufacturerId) && !p.Deleted);

        if (!showHidden)
            query = query.Where(p => p.Published);

        return await query.OrderBy(p => p.DisplayOrder).ThenBy(p => p.Id).ToListAsync();
    }

    #endregion

    #region Product Attributes

    public async Task<ProductAttribute> GetProductAttributeByIdAsync(int productAttributeId)
    {
        return await _productAttributeRepository.GetAsync(productAttributeId);
    }

    public async Task<IEnumerable<ProductAttribute>> GetAllProductAttributesAsync()
    {
        return await _productAttributeRepository.GetAllAsync();
    }

    public async Task<ProductAttribute> CreateProductAttributeAsync(ProductAttribute productAttribute)
    {
        return await _productAttributeRepository.AddAsync(productAttribute);
    }

    public async Task UpdateProductAttributeAsync(ProductAttribute productAttribute)
    {
        await _productAttributeRepository.UpdateAsync(productAttribute);
    }

    public async Task DeleteProductAttributeAsync(int productAttributeId)
    {
        await _productAttributeRepository.DeleteAsync(productAttributeId);
    }

    public async Task DeleteProductAttributesAsync(IEnumerable<ProductAttribute> productAttributes)
    {
        await _productAttributeRepository.DeleteRangeAsync(productAttributes);
    }

    public async Task<IEnumerable<ProductAttribute>> GetProductAttributesByProductIdAsync(int productId)
    {
        var product = await _productRepository.GetAsync(productId);
        return product?.ProductAttributes ?? Enumerable.Empty<ProductAttribute>();
    }

    public async Task<string> FormatAttributesAsync(Product product, string attributesXml, bool includePrices = true)
    {
        return await _productAttributeFormatter.FormatAttributesAsync(product, attributesXml, includePrices);
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

    #endregion

    #region Product Pictures

    public async Task<ProductPicture> GetProductPictureByIdAsync(int productPictureId)
    {
        return await _productPictureRepository.GetAsync(productPictureId);
    }

    public async Task<IEnumerable<ProductPicture>> GetProductPicturesByProductIdAsync(int productId)
    {
        return await _productPictureRepository.GetAllAsync(query =>
            query.Where(pp => pp.ProductId == productId)
                 .OrderBy(pp => pp.DisplayOrder)
                 .ThenBy(pp => pp.Id));
    }

    public async Task<ProductPicture> CreateProductPictureAsync(ProductPicture productPicture)
    {
        return await _productPictureRepository.AddAsync(productPicture);
    }

    public async Task UpdateProductPictureAsync(ProductPicture productPicture)
    {
        await _productPictureRepository.UpdateAsync(productPicture);
    }

    public async Task DeleteProductPictureAsync(int productPictureId)
    {
        await _productPictureRepository.DeleteAsync(productPictureId);
    }

    public async Task DeleteProductPicturesAsync(IEnumerable<ProductPicture> productPictures)
    {
        await _productPictureRepository.DeleteRangeAsync(productPictures);
    }

    public async Task<IDictionary<int, int[]>> GetProductsImagesIdsAsync(int[] productsIds)
    {
        var result = new Dictionary<int, int[]>();
        
        if (productsIds == null || !productsIds.Any())
            return result;

        var pictures = await _productPictureRepository.GetAllAsync(query =>
            query.Where(pp => productsIds.Contains(pp.ProductId))
                 .OrderBy(pp => pp.DisplayOrder)
                 .ThenBy(pp => pp.Id));

        foreach (var productId in productsIds)
        {
            var productPictures = pictures.Where(pp => pp.ProductId == productId)
                                        .Select(pp => pp.PictureId)
                                        .ToArray();
            result.Add(productId, productPictures);
        }

        return result;
    }

    #endregion

    #region Product Videos

    public async Task<ProductVideo> GetProductVideoByIdAsync(int productVideoId)
    {
        return await _productVideoRepository.GetAsync(productVideoId);
    }

    public async Task<IEnumerable<ProductVideo>> GetProductVideosByProductIdAsync(int productId)
    {
        return await _productVideoRepository.GetAllAsync(query =>
            query.Where(pv => pv.ProductId == productId)
                 .OrderBy(pv => pv.DisplayOrder)
                 .ThenBy(pv => pv.Id));
    }

    public async Task<ProductVideo> CreateProductVideoAsync(ProductVideo productVideo)
    {
        return await _productVideoRepository.AddAsync(productVideo);
    }

    public async Task UpdateProductVideoAsync(ProductVideo productVideo)
    {
        await _productVideoRepository.UpdateAsync(productVideo);
    }

    public async Task DeleteProductVideoAsync(int productVideoId)
    {
        await _productVideoRepository.DeleteAsync(productVideoId);
    }

    public async Task DeleteProductVideosAsync(IEnumerable<ProductVideo> productVideos)
    {
        await _productVideoRepository.DeleteRangeAsync(productVideos);
    }

    #endregion

    #region Product Reviews

    public async Task<ProductReview> GetProductReviewByIdAsync(int productReviewId)
    {
        return await _productReviewRepository.GetAsync(productReviewId);
    }

    public async Task<IEnumerable<ProductReview>> GetAllProductReviewsAsync(
        int customerId = 0,
        bool? approved = null,
        DateTime? fromUtc = null,
        DateTime? toUtc = null,
        string message = null,
        int storeId = 0,
        int productId = 0,
        int vendorId = 0,
        bool showHidden = false,
        int pageIndex = 0,
        int pageSize = int.MaxValue)
    {
        var query = _productReviewRepository.GetAll()
            .Where(pr => !pr.Deleted);

        if (customerId > 0)
            query = query.Where(pr => pr.CustomerId == customerId);

        if (approved.HasValue)
            query = query.Where(pr => pr.IsApproved == approved.Value);

        if (fromUtc.HasValue)
            query = query.Where(pr => pr.CreatedOnUtc >= fromUtc.Value);

        if (toUtc.HasValue)
            query = query.Where(pr => pr.CreatedOnUtc <= toUtc.Value);

        if (!string.IsNullOrEmpty(message))
            query = query.Where(pr => pr.Title.Contains(message) || pr.ReviewText.Contains(message));

        if (storeId > 0)
            query = query.Where(pr => pr.StoreId == storeId);

        if (productId > 0)
            query = query.Where(pr => pr.ProductId == productId);

        if (vendorId > 0)
            query = query.Where(pr => pr.VendorId == vendorId);

        if (!showHidden)
            query = query.Where(pr => pr.IsApproved);

        return await query.OrderByDescending(pr => pr.CreatedOnUtc)
                         .Skip(pageIndex * pageSize)
                         .Take(pageSize)
                         .ToListAsync();
    }

    public async Task<ProductReview> CreateProductReviewAsync(ProductReview productReview)
    {
        return await _productReviewRepository.AddAsync(productReview);
    }

    public async Task UpdateProductReviewAsync(ProductReview productReview)
    {
        await _productReviewRepository.UpdateAsync(productReview);
    }

    public async Task DeleteProductReviewAsync(int productReviewId)
    {
        await _productReviewRepository.DeleteAsync(productReviewId);
    }

    public async Task DeleteProductReviewsAsync(IEnumerable<ProductReview> productReviews)
    {
        await _productReviewRepository.DeleteRangeAsync(productReviews);
    }

    public async Task<bool> CanAddReviewAsync(int productId, int storeId = 0)
    {
        var product = await _productRepository.GetAsync(productId);
        if (product == null || !product.Published || product.Deleted)
            return false;

        if (storeId > 0 && product.StoreId != storeId)
            return false;

        return true;
    }

    #endregion

    #region Product Tags

    public async Task<ProductTag> GetProductTagByIdAsync(int productTagId)
    {
        return await _productTagRepository.GetAsync(productTagId);
    }

    public async Task<IEnumerable<ProductTag>> GetAllProductTagsAsync()
    {
        return await _productTagRepository.GetAllAsync();
    }

    public async Task<ProductTag> CreateProductTagAsync(ProductTag productTag)
    {
        return await _productTagRepository.AddAsync(productTag);
    }

    public async Task UpdateProductTagAsync(ProductTag productTag)
    {
        await _productTagRepository.UpdateAsync(productTag);
    }

    public async Task DeleteProductTagAsync(int productTagId)
    {
        await _productTagRepository.DeleteAsync(productTagId);
    }

    public async Task DeleteProductTagsAsync(IEnumerable<ProductTag> productTags)
    {
        await _productTagRepository.DeleteRangeAsync(productTags);
    }

    public async Task<IEnumerable<ProductTag>> GetProductTagsByProductIdAsync(int productId)
    {
        var product = await _productRepository.GetAsync(productId);
        return product?.ProductTags ?? Enumerable.Empty<ProductTag>();
    }

    public async Task<IEnumerable<Product>> GetProductsByTagIdAsync(int tagId, bool showHidden = false)
    {
        var query = _productRepository.GetAll()
            .Where(p => p.ProductTags.Any(pt => pt.Id == tagId) && !p.Deleted);

        if (!showHidden)
            query = query.Where(p => p.Published);

        return await query.OrderBy(p => p.DisplayOrder).ThenBy(p => p.Id).ToListAsync();
    }

    #endregion

    #region Product Warehouses

    public async Task<ProductWarehouseInventory> GetProductWarehouseInventoryByIdAsync(int productWarehouseInventoryId)
    {
        return await _productWarehouseInventoryRepository.GetAsync(productWarehouseInventoryId);
    }

    public async Task<IEnumerable<ProductWarehouseInventory>> GetProductWarehouseInventoriesByProductIdAsync(int productId)
    {
        return await _productWarehouseInventoryRepository.GetAllAsync(query =>
            query.Where(pwi => pwi.ProductId == productId));
    }

    public async Task<ProductWarehouseInventory> CreateProductWarehouseInventoryAsync(ProductWarehouseInventory productWarehouseInventory)
    {
        return await _productWarehouseInventoryRepository.AddAsync(productWarehouseInventory);
    }

    public async Task UpdateProductWarehouseInventoryAsync(ProductWarehouseInventory productWarehouseInventory)
    {
        await _productWarehouseInventoryRepository.UpdateAsync(productWarehouseInventory);
    }

    public async Task DeleteProductWarehouseInventoryAsync(int productWarehouseInventoryId)
    {
        await _productWarehouseInventoryRepository.DeleteAsync(productWarehouseInventoryId);
    }

    public async Task DeleteProductWarehouseInventoriesAsync(IEnumerable<ProductWarehouseInventory> productWarehouseInventories)
    {
        await _productWarehouseInventoryRepository.DeleteRangeAsync(productWarehouseInventories);
    }

    public async Task<int> GetTotalStockQuantityAsync(Product product, bool useReservedQuantity = true, int warehouseId = 0)
    {
        if (product == null)
            return 0;

        var query = _productWarehouseInventoryRepository.GetAll()
            .Where(pwi => pwi.ProductId == product.Id);

        if (warehouseId > 0)
            query = query.Where(pwi => pwi.WarehouseId == warehouseId);

        var totalQuantity = await query.SumAsync(pwi => pwi.StockQuantity);

        if (useReservedQuantity)
            totalQuantity -= await query.SumAsync(pwi => pwi.ReservedQuantity);

        return totalQuantity;
    }

    #endregion

    #region Related Products

    public async Task<RelatedProduct> GetRelatedProductByIdAsync(int relatedProductId)
    {
        return await _relatedProductRepository.GetAsync(relatedProductId);
    }

    public async Task<IEnumerable<RelatedProduct>> GetRelatedProductsByProductIdAsync(int productId, bool showHidden = false)
    {
        var query = _relatedProductRepository.GetAll()
            .Where(rp => rp.ProductId1 == productId);

        if (!showHidden)
            query = query.Where(rp => rp.Product2.Published && !rp.Product2.Deleted);

        return await query.OrderBy(rp => rp.DisplayOrder).ThenBy(rp => rp.Id).ToListAsync();
    }

    public async Task<RelatedProduct> CreateRelatedProductAsync(RelatedProduct relatedProduct)
    {
        return await _relatedProductRepository.AddAsync(relatedProduct);
    }

    public async Task UpdateRelatedProductAsync(RelatedProduct relatedProduct)
    {
        await _relatedProductRepository.UpdateAsync(relatedProduct);
    }

    public async Task DeleteRelatedProductAsync(int relatedProductId)
    {
        await _relatedProductRepository.DeleteAsync(relatedProductId);
    }

    public async Task DeleteRelatedProductsAsync(IEnumerable<RelatedProduct> relatedProducts)
    {
        await _relatedProductRepository.DeleteRangeAsync(relatedProducts);
    }

    #endregion

    #region Cross-sell Products

    public async Task<CrossSellProduct> GetCrossSellProductByIdAsync(int crossSellProductId)
    {
        return await _crossSellProductRepository.GetAsync(crossSellProductId);
    }

    public async Task<IEnumerable<CrossSellProduct>> GetCrossSellProductsByProductIdAsync(int productId, bool showHidden = false)
    {
        var query = _crossSellProductRepository.GetAll()
            .Where(csp => csp.ProductId1 == productId);

        if (!showHidden)
            query = query.Where(csp => csp.Product2.Published && !csp.Product2.Deleted);

        return await query.OrderBy(csp => csp.Id).ToListAsync();
    }

    public async Task<CrossSellProduct> CreateCrossSellProductAsync(CrossSellProduct crossSellProduct)
    {
        return await _crossSellProductRepository.AddAsync(crossSellProduct);
    }

    public async Task UpdateCrossSellProductAsync(CrossSellProduct crossSellProduct)
    {
        await _crossSellProductRepository.UpdateAsync(crossSellProduct);
    }

    public async Task DeleteCrossSellProductAsync(int crossSellProductId)
    {
        await _crossSellProductRepository.DeleteAsync(crossSellProductId);
    }

    public async Task DeleteCrossSellProductsAsync(IEnumerable<CrossSellProduct> crossSellProducts)
    {
        await _crossSellProductRepository.DeleteRangeAsync(crossSellProducts);
    }

    public async Task<IEnumerable<Product>> GetCrossSellProductsByShoppingCartAsync(IList<ShoppingCartItem> cart, int numberOfProducts)
    {
        if (cart == null || !cart.Any())
            return Enumerable.Empty<Product>();

        var productIds = cart.Select(item => item.ProductId).Distinct().ToArray();
        var crossSellProducts = await _crossSellProductRepository.GetAllAsync(query =>
            query.Where(csp => productIds.Contains(csp.ProductId1) && csp.Product2.Published && !csp.Product2.Deleted)
                 .Select(csp => csp.Product2)
                 .Distinct()
                 .Take(numberOfProducts));

        return crossSellProducts;
    }

    #endregion

    #region Tier Prices

    public async Task<TierPrice> GetTierPriceByIdAsync(int tierPriceId)
    {
        return await _tierPriceRepository.GetAsync(tierPriceId);
    }

    public async Task<IEnumerable<TierPrice>> GetTierPricesByProductIdAsync(int productId)
    {
        return await _tierPriceRepository.GetAllAsync(query =>
            query.Where(tp => tp.ProductId == productId)
                 .OrderBy(tp => tp.Quantity));
    }

    public async Task<TierPrice> CreateTierPriceAsync(TierPrice tierPrice)
    {
        return await _tierPriceRepository.AddAsync(tierPrice);
    }

    public async Task UpdateTierPriceAsync(TierPrice tierPrice)
    {
        await _tierPriceRepository.UpdateAsync(tierPrice);
    }

    public async Task DeleteTierPriceAsync(int tierPriceId)
    {
        await _tierPriceRepository.DeleteAsync(tierPriceId);
    }

    public async Task DeleteTierPricesAsync(IEnumerable<TierPrice> tierPrices)
    {
        await _tierPriceRepository.DeleteRangeAsync(tierPrices);
    }

    public async Task<TierPrice> GetPreferredTierPriceAsync(Product product, Customer customer, Store store, int quantity)
    {
        if (product == null)
            return null;

        var tierPrices = await _tierPriceRepository.GetAllAsync(query =>
            query.Where(tp => tp.ProductId == product.Id && tp.Quantity <= quantity)
                 .OrderByDescending(tp => tp.Quantity));

        return tierPrices.FirstOrDefault();
    }

    #endregion

    #region Product Discounts

    public async Task<DiscountProductMapping> GetDiscountProductMappingByIdAsync(int discountProductMappingId)
    {
        return await _discountProductMappingRepository.GetAsync(discountProductMappingId);
    }

    public async Task<IEnumerable<DiscountProductMapping>> GetDiscountProductMappingsByProductIdAsync(int productId)
    {
        return await _discountProductMappingRepository.GetAllAsync(query =>
            query.Where(dpm => dpm.ProductId == productId));
    }

    public async Task<DiscountProductMapping> CreateDiscountProductMappingAsync(DiscountProductMapping discountProductMapping)
    {
        return await _discountProductMappingRepository.AddAsync(discountProductMapping);
    }

    public async Task UpdateDiscountProductMappingAsync(DiscountProductMapping discountProductMapping)
    {
        await _discountProductMappingRepository.UpdateAsync(discountProductMapping);
    }

    public async Task DeleteDiscountProductMappingAsync(int discountProductMappingId)
    {
        await _discountProductMappingRepository.DeleteAsync(discountProductMappingId);
    }

    public async Task DeleteDiscountProductMappingsAsync(IEnumerable<DiscountProductMapping> discountProductMappings)
    {
        await _discountProductMappingRepository.DeleteRangeAsync(discountProductMappings);
    }

    public async Task ClearDiscountProductMappingAsync(Discount discount)
    {
        if (discount == null)
            return;

        var mappings = await _discountProductMappingRepository.GetAllAsync(query =>
            query.Where(dpm => dpm.DiscountId == discount.Id));

        await _discountProductMappingRepository.DeleteRangeAsync(mappings);
    }

    #endregion

    #region Product Specifications

    public async Task<SpecificationAttribute> GetSpecificationAttributeByIdAsync(int specificationAttributeId)
    {
        return await _specificationAttributeRepository.GetAsync(specificationAttributeId);
    }

    public async Task<IEnumerable<SpecificationAttribute>> GetAllSpecificationAttributesAsync()
    {
        return await _specificationAttributeRepository.GetAllAsync();
    }

    public async Task<SpecificationAttribute> CreateSpecificationAttributeAsync(SpecificationAttribute specificationAttribute)
    {
        return await _specificationAttributeRepository.AddAsync(specificationAttribute);
    }

    public async Task UpdateSpecificationAttributeAsync(SpecificationAttribute specificationAttribute)
    {
        await _specificationAttributeRepository.UpdateAsync(specificationAttribute);
    }

    public async Task DeleteSpecificationAttributeAsync(int specificationAttributeId)
    {
        await _specificationAttributeRepository.DeleteAsync(specificationAttributeId);
    }

    public async Task DeleteSpecificationAttributesAsync(IEnumerable<SpecificationAttribute> specificationAttributes)
    {
        await _specificationAttributeRepository.DeleteRangeAsync(specificationAttributes);
    }

    public async Task<IEnumerable<SpecificationAttribute>> GetSpecificationAttributesByProductIdAsync(int productId)
    {
        var product = await _productRepository.GetAsync(productId);
        return product?.SpecificationAttributes ?? Enumerable.Empty<SpecificationAttribute>();
    }

    #endregion

    #region Product Templates

    public async Task<ProductTemplate> GetProductTemplateByIdAsync(int productTemplateId)
    {
        return await _productTemplateRepository.GetAsync(productTemplateId);
    }

    public async Task<IEnumerable<ProductTemplate>> GetAllProductTemplatesAsync()
    {
        return await _productTemplateRepository.GetAllAsync();
    }

    public async Task<ProductTemplate> CreateProductTemplateAsync(ProductTemplate productTemplate)
    {
        return await _productTemplateRepository.AddAsync(productTemplate);
    }

    public async Task UpdateProductTemplateAsync(ProductTemplate productTemplate)
    {
        await _productTemplateRepository.UpdateAsync(productTemplate);
    }

    public async Task DeleteProductTemplateAsync(int productTemplateId)
    {
        await _productTemplateRepository.DeleteAsync(productTemplateId);
    }

    public async Task DeleteProductTemplatesAsync(IEnumerable<ProductTemplate> productTemplates)
    {
        await _productTemplateRepository.DeleteRangeAsync(productTemplates);
    }

    #endregion

    #region Compare Products

    private readonly IList<int> _comparedProducts = new List<int>();

    public void ClearCompareProducts()
    {
        _comparedProducts.Clear();
    }

    public async Task<IList<Product>> GetComparedProductsAsync()
    {
        var products = new List<Product>();
        foreach (var productId in _comparedProducts)
        {
            var product = await _productRepository.GetAsync(productId);
            if (product != null)
                products.Add(product);
        }
        return products;
    }

    public async Task RemoveProductFromCompareListAsync(int productId)
    {
        if (_comparedProducts.Contains(productId))
            _comparedProducts.Remove(productId);
    }

    public async Task AddProductToCompareListAsync(int productId)
    {
        if (!_comparedProducts.Contains(productId))
            _comparedProducts.Add(productId);
    }

    #endregion

    #region Copy Product

    public async Task<Product> CopyProductAsync(Product product, string newName,
        bool isPublished = true, bool copyMultimedia = true, bool copyAssociatedProducts = true)
    {
        if (product == null)
            throw new ArgumentNullException(nameof(product));

        var newProduct = new Product
        {
            Name = newName,
            ShortDescription = product.ShortDescription,
            FullDescription = product.FullDescription,
            AdminComment = product.AdminComment,
            ProductTypeId = product.ProductTypeId,
            TemplateId = product.TemplateId,
            ShowOnHomepage = product.ShowOnHomepage,
            MetaKeywords = product.MetaKeywords,
            MetaDescription = product.MetaDescription,
            MetaTitle = product.MetaTitle,
            SeName = product.SeName,
            AllowCustomerReviews = product.AllowCustomerReviews,
            Published = isPublished,
            Sku = product.Sku,
            ManufacturerPartNumber = product.ManufacturerPartNumber,
            Gtin = product.Gtin,
            IsGiftCard = product.IsGiftCard,
            GiftCardTypeId = product.GiftCardTypeId,
            RequireOtherProducts = product.RequireOtherProducts,
            RequiredProductIds = product.RequiredProductIds,
            AutomaticallyAddRequiredProducts = product.AutomaticallyAddRequiredProducts,
            IsDownload = product.IsDownload,
            DownloadId = product.DownloadId,
            UnlimitedDownloads = product.UnlimitedDownloads,
            MaxNumberOfDownloads = product.MaxNumberOfDownloads,
            DownloadExpirationDays = product.DownloadExpirationDays,
            DownloadActivationTypeId = product.DownloadActivationTypeId,
            HasSampleDownload = product.HasSampleDownload,
            SampleDownloadId = product.SampleDownloadId,
            HasUserAgreement = product.HasUserAgreement,
            UserAgreementText = product.UserAgreementText,
            IsRecurring = product.IsRecurring,
            RecurringCycleLength = product.RecurringCycleLength,
            RecurringCyclePeriodId = product.RecurringCyclePeriodId,
            RecurringTotalCycles = product.RecurringTotalCycles,
            IsRental = product.IsRental,
            RentalPriceLength = product.RentalPriceLength,
            RentalPricePeriodId = product.RentalPricePeriodId,
            IsShipEnabled = product.IsShipEnabled,
            IsFreeShipping = product.IsFreeShipping,
            ShipSeparately = product.ShipSeparately,
            AdditionalShippingCharge = product.AdditionalShippingCharge,
            DeliveryDateId = product.DeliveryDateId,
            IsTaxExempt = product.IsTaxExempt,
            TaxCategoryId = product.TaxCategoryId,
            IsTelecommunicationsOrBroadcastingOrElectronicServices = product.IsTelecommunicationsOrBroadcastingOrElectronicServices,
            ManageInventoryMethodId = product.ManageInventoryMethodId,
            ProductAvailabilityRangeId = product.ProductAvailabilityRangeId,
            UseMultipleWarehouses = product.UseMultipleWarehouses,
            WarehouseId = product.WarehouseId,
            StockQuantity = product.StockQuantity,
            DisplayStockAvailability = product.DisplayStockAvailability,
            DisplayStockQuantity = product.DisplayStockQuantity,
            MinStockQuantity = product.MinStockQuantity,
            LowStockActivityId = product.LowStockActivityId,
            NotifyAdminForQuantityBelow = product.NotifyAdminForQuantityBelow,
            BackorderModeId = product.BackorderModeId,
            AllowBackInStockSubscriptions = product.AllowBackInStockSubscriptions,
            OrderMinimumQuantity = product.OrderMinimumQuantity,
            OrderMaximumQuantity = product.OrderMaximumQuantity,
            AllowedQuantities = product.AllowedQuantities,
            AllowAddingOnlyExistingAttributeCombinations = product.AllowAddingOnlyExistingAttributeCombinations,
            NotReturnable = product.NotReturnable,
            DisableBuyButton = product.DisableBuyButton,
            DisableWishlistButton = product.DisableWishlistButton,
            DisableAddToCompareListButton = product.DisableAddToCompareListButton,
            AvailableForPreOrder = product.AvailableForPreOrder,
            PreOrderAvailabilityStartDateTimeUtc = product.PreOrderAvailabilityStartDateTimeUtc,
            CallForPrice = product.CallForPrice,
            Price = product.Price,
            OldPrice = product.OldPrice,
            ProductCost = product.ProductCost,
            CustomerEntersPrice = product.CustomerEntersPrice,
            MinimumCustomerEnteredPrice = product.MinimumCustomerEnteredPrice,
            MaximumCustomerEnteredPrice = product.MaximumCustomerEnteredPrice,
            BasepriceEnabled = product.BasepriceEnabled,
            BasepriceAmount = product.BasepriceAmount,
            BasepriceUnitId = product.BasepriceUnitId,
            BasepriceBaseAmount = product.BasepriceBaseAmount,
            BasepriceBaseUnitId = product.BasepriceBaseUnitId,
            MarkAsNew = product.MarkAsNew,
            MarkAsNewStartDateTimeUtc = product.MarkAsNewStartDateTimeUtc,
            MarkAsNewEndDateTimeUtc = product.MarkAsNewEndDateTimeUtc,
            Weight = product.Weight,
            Length = product.Length,
            Width = product.Width,
            Height = product.Height,
            DisplayOrder = product.DisplayOrder,
            DisplayOrderCategory = product.DisplayOrderCategory,
            DisplayOrderManufacturer = product.DisplayOrderManufacturer,
            OnSale = product.OnSale,
            Deleted = false,
            CreatedOnUtc = DateTime.UtcNow
        };

        // Copy product pictures
        if (copyMultimedia)
        {
            var productPictures = await GetProductPicturesByProductIdAsync(product.Id);
            foreach (var productPicture in productPictures)
            {
                var newProductPicture = new ProductPicture
                {
                    PictureId = productPicture.PictureId,
                    DisplayOrder = productPicture.DisplayOrder
                };
                newProduct.ProductPictures.Add(newProductPicture);
            }
        }

        // Copy product videos
        if (copyMultimedia)
        {
            var productVideos = await GetProductVideosByProductIdAsync(product.Id);
            foreach (var productVideo in productVideos)
            {
                var newProductVideo = new ProductVideo
                {
                    VideoId = productVideo.VideoId,
                    DisplayOrder = productVideo.DisplayOrder
                };
                newProduct.ProductVideos.Add(newProductVideo);
            }
        }

        // Copy associated products
        if (copyAssociatedProducts)
        {
            var associatedProducts = await GetAssociatedProductsAsync(product.Id);
            foreach (var associatedProduct in associatedProducts)
            {
                var newAssociatedProduct = new RelatedProduct
                {
                    ProductId2 = associatedProduct.Id,
                    DisplayOrder = associatedProduct.DisplayOrder
                };
                newProduct.RelatedProducts.Add(newAssociatedProduct);
            }
        }

        return await _productRepository.AddAsync(newProduct);
    }

    #endregion

    #region Advanced Product Search

    public async Task<IPagedList<Product>> SearchProductsAsync(
        int pageIndex = 0,
        int pageSize = int.MaxValue,
        IList<int> categoryIds = null,
        IList<int> manufacturerIds = null,
        int storeId = 0,
        int vendorId = 0,
        int warehouseId = 0,
        ProductType? productType = null,
        bool visibleIndividuallyOnly = false,
        bool excludeFeaturedProducts = false,
        decimal? priceMin = null,
        decimal? priceMax = null,
        int productTagId = 0,
        string keywords = null,
        bool searchDescriptions = false,
        bool searchManufacturerPartNumber = true,
        bool searchSku = true,
        bool searchProductTags = false,
        int languageId = 0,
        IList<SpecificationAttributeOption> filteredSpecOptions = null,
        ProductSortingEnum orderBy = ProductSortingEnum.Position,
        bool showHidden = false,
        bool? overridePublished = null)
    {
        var query = _productRepository.GetAll()
            .Where(p => !p.Deleted);

        if (categoryIds != null && categoryIds.Any())
            query = query.Where(p => p.Categories.Any(c => categoryIds.Contains(c.Id)));

        if (manufacturerIds != null && manufacturerIds.Any())
            query = query.Where(p => p.Manufacturers.Any(m => manufacturerIds.Contains(m.Id)));

        if (storeId > 0)
            query = query.Where(p => p.StoreId == storeId);

        if (vendorId > 0)
            query = query.Where(p => p.VendorId == vendorId);

        if (warehouseId > 0)
            query = query.Where(p => p.WarehouseId == warehouseId);

        if (productType.HasValue)
            query = query.Where(p => p.ProductTypeId == (int)productType.Value);

        if (visibleIndividuallyOnly)
            query = query.Where(p => p.VisibleIndividually);

        if (excludeFeaturedProducts)
            query = query.Where(p => !p.Featured);

        if (priceMin.HasValue)
            query = query.Where(p => p.Price >= priceMin.Value);

        if (priceMax.HasValue)
            query = query.Where(p => p.Price <= priceMax.Value);

        if (productTagId > 0)
            query = query.Where(p => p.ProductTags.Any(pt => pt.Id == productTagId));

        if (!string.IsNullOrEmpty(keywords))
        {
            var searchTerm = keywords.ToLower();
            query = query.Where(p =>
                p.Name.ToLower().Contains(searchTerm) ||
                (searchDescriptions && p.ShortDescription.ToLower().Contains(searchTerm)) ||
                (searchDescriptions && p.FullDescription.ToLower().Contains(searchTerm)) ||
                (searchManufacturerPartNumber && p.ManufacturerPartNumber.ToLower().Contains(searchTerm)) ||
                (searchSku && p.Sku.ToLower().Contains(searchTerm)) ||
                (searchProductTags && p.ProductTags.Any(pt => pt.Name.ToLower().Contains(searchTerm))));
        }

        if (filteredSpecOptions != null && filteredSpecOptions.Any())
        {
            query = query.Where(p => p.SpecificationAttributes
                .Any(sa => filteredSpecOptions.Contains(sa.SpecificationAttributeOption)));
        }

        if (!showHidden)
            query = query.Where(p => p.Published);

        if (overridePublished.HasValue)
            query = query.Where(p => p.Published == overridePublished.Value);

        // Apply sorting
        query = orderBy switch
        {
            ProductSortingEnum.Position => query.OrderBy(p => p.DisplayOrder),
            ProductSortingEnum.NameAsc => query.OrderBy(p => p.Name),
            ProductSortingEnum.NameDesc => query.OrderByDescending(p => p.Name),
            ProductSortingEnum.PriceAsc => query.OrderBy(p => p.Price),
            ProductSortingEnum.PriceDesc => query.OrderByDescending(p => p.Price),
            ProductSortingEnum.CreatedOn => query.OrderByDescending(p => p.CreatedOnUtc),
            _ => query
        };

        return await query.ToPagedListAsync(pageIndex, pageSize);
    }

    #endregion

    #region Product Review Totals Management

    public async Task UpdateProductReviewTotalsAsync(Product product)
    {
        if (product == null)
            throw new ArgumentNullException(nameof(product));

        var reviews = await _productReviewRepository.GetAllAsync(query =>
            query.Where(pr => pr.ProductId == product.Id && pr.IsApproved));

        product.ApprovedRatingSum = reviews.Sum(r => r.Rating);
        product.NotApprovedRatingSum = reviews.Where(r => !r.IsApproved).Sum(r => r.Rating);
        product.ApprovedTotalReviews = reviews.Count(r => r.IsApproved);
        product.NotApprovedTotalReviews = reviews.Count(r => !r.IsApproved);

        await _productRepository.UpdateAsync(product);
    }

    #endregion

    #region Product Required Products Management

    public int[] ParseRequiredProductIds(Product product)
    {
        if (product == null || string.IsNullOrEmpty(product.RequiredProductIds))
            return Array.Empty<int>();

        return product.RequiredProductIds
            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(id => int.Parse(id.Trim()))
            .ToArray();
    }

    public async Task<bool> HasRequiredProductsAsync(Product product)
    {
        if (product == null || !product.RequireOtherProducts)
            return false;

        var requiredProductIds = ParseRequiredProductIds(product);
        if (!requiredProductIds.Any())
            return false;

        var requiredProducts = await _productRepository.GetAllAsync(query =>
            query.Where(p => requiredProductIds.Contains(p.Id) && p.Published && !p.Deleted));

        return requiredProducts.Any();
    }

    #endregion

    #region Product Allowed Quantities Management

    public int[] ParseAllowedQuantities(Product product)
    {
        if (product == null || string.IsNullOrEmpty(product.AllowedQuantities))
            return Array.Empty<int>();

        return product.AllowedQuantities
            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(qty => int.Parse(qty.Trim()))
            .ToArray();
    }

    public async Task<bool> IsQuantityAllowedAsync(Product product, int quantity)
    {
        if (product == null)
            return false;

        if (product.AllowedQuantities == null)
            return true;

        var allowedQuantities = ParseAllowedQuantities(product);
        return allowedQuantities.Contains(quantity);
    }

    #endregion

    #region Product Rental Periods Management

    public int GetRentalPeriods(Product product, DateTime startDate, DateTime endDate)
    {
        if (product == null || !product.IsRental)
            return 0;

        if (startDate >= endDate)
            return 0;

        var totalDays = (endDate - startDate).TotalDays;
        var periodLength = product.RentalPriceLength;

        return product.RentalPricePeriodId switch
        {
            1 => (int)Math.Ceiling(totalDays / periodLength), // Days
            2 => (int)Math.Ceiling(totalDays / (periodLength * 7)), // Weeks
            3 => (int)Math.Ceiling(totalDays / (periodLength * 30)), // Months
            4 => (int)Math.Ceiling(totalDays / (periodLength * 365)), // Years
            _ => 0
        };
    }

    public string FormatRentalDate(Product product, DateTime date)
    {
        if (product == null || !product.IsRental)
            return string.Empty;

        return date.ToString("g");
    }

    #endregion

    #region Product Stock Message Formatting

    public async Task<string> FormatStockMessageAsync(Product product, string attributesXml)
    {
        if (product == null)
            return string.Empty;

        var stockMessage = string.Empty;

        if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock)
        {
            if (product.DisplayStockAvailability)
            {
                var stockQuantity = await GetTotalStockQuantityAsync(product);
                if (stockQuantity > 0)
                {
                    if (product.DisplayStockQuantity)
                        stockMessage = $"{stockQuantity} in stock";
                    else
                        stockMessage = "In stock";
                }
                else
                {
                    switch (product.BackorderMode)
                    {
                        case BackorderMode.NoBackorders:
                            stockMessage = "Out of stock";
                            break;
                        case BackorderMode.AllowQtyBelow0:
                            stockMessage = "Available for backorder";
                            break;
                        case BackorderMode.AllowQtyBelow0AndNotifyCustomer:
                            stockMessage = "Available for backorder";
                            break;
                    }
                }
            }
        }

        return stockMessage;
    }

    #endregion

    #region Product Store Mapping Management

    public async Task UpdateProductStoreMappingsAsync(Product product, IList<int> limitedToStoresIds)
    {
        if (product == null)
            throw new ArgumentNullException(nameof(product));

        var existingMappings = await _productStoreMappingRepository.GetAllAsync(query =>
            query.Where(psm => psm.ProductId == product.Id));

        // Remove mappings for stores that are not in the new list
        var mappingsToDelete = existingMappings.Where(psm => !limitedToStoresIds.Contains(psm.StoreId));
        await _productStoreMappingRepository.DeleteRangeAsync(mappingsToDelete);

        // Add new mappings
        var existingStoreIds = existingMappings.Select(psm => psm.StoreId);
        var newStoreIds = limitedToStoresIds.Except(existingStoreIds);

        foreach (var storeId in newStoreIds)
        {
            var mapping = new ProductStoreMapping
            {
                ProductId = product.Id,
                StoreId = storeId
            };
            await _productStoreMappingRepository.AddAsync(mapping);
        }
    }

    #endregion

    #region Product Download Management

    public async Task<bool> IsDownloadAllowedAsync(Product product, Customer customer)
    {
        if (product == null || !product.IsDownload)
            return false;

        if (customer == null)
            return false;

        // Check if customer has purchased the product
        var hasPurchased = await _orderService.HasCustomerPurchasedProductAsync(customer.Id, product.Id);
        if (!hasPurchased)
            return false;

        // Check download count limit
        if (!product.UnlimitedDownloads)
        {
            var downloadCount = await _downloadRepository.GetAllAsync(query =>
                query.Where(d => d.ProductId == product.Id && d.CustomerId == customer.Id)
                     .CountAsync());

            if (downloadCount >= product.MaxNumberOfDownloads)
                return false;
        }

        // Check download expiration
        if (product.DownloadExpirationDays.HasValue)
        {
            var order = await _orderService.GetOrderByProductIdAsync(customer.Id, product.Id);
            if (order != null)
            {
                var expirationDate = order.CreatedOnUtc.AddDays(product.DownloadExpirationDays.Value);
                if (DateTime.UtcNow > expirationDate)
                    return false;
            }
        }

        return true;
    }

    #endregion

    #region Product Gift Card Management

    public async Task<bool> IsGiftCardValidAsync(GiftCard giftCard)
    {
        if (giftCard == null)
            return false;

        if (!giftCard.IsGiftCardActivated)
            return false;

        if (giftCard.IsGiftCardUsed)
            return false;

        if (giftCard.GiftCardEndDate.HasValue && DateTime.UtcNow > giftCard.GiftCardEndDate.Value)
            return false;

        return true;
    }

    #endregion

    #region Product Recurring Payment Management

    public async Task<bool> IsRecurringPaymentValidAsync(Product product, Customer customer)
    {
        if (product == null || !product.IsRecurring)
            return false;

        if (customer == null)
            return false;

        // Check if customer has any active recurring payments
        var hasActiveRecurringPayment = await _orderService.HasCustomerActiveRecurringPaymentAsync(customer.Id, product.Id);
        if (hasActiveRecurringPayment)
            return false;

        return true;
    }

    public async Task<decimal> CalculateRecurringPaymentAmountAsync(Product product, int cycles)
    {
        if (product == null || !product.IsRecurring)
            return 0;

        if (cycles <= 0)
            return 0;

        return product.Price * cycles;
    }

    #endregion

    #region Product Bundles

    public async Task<ProductBundleItem> GetProductBundleItemByIdAsync(int bundleItemId)
    {
        return await _productBundleItemRepository.GetAsync(bundleItemId);
    }

    public async Task<IEnumerable<ProductBundleItem>> GetProductBundleItemsByProductIdAsync(int productId)
    {
        return await _productBundleItemRepository.GetAllAsync(query =>
            query.Where(pbi => pbi.ProductId == productId)
                 .OrderBy(pbi => pbi.DisplayOrder)
                 .ThenBy(pbi => pbi.Id));
    }

    public async Task<ProductBundleItem> CreateProductBundleItemAsync(ProductBundleItem bundleItem)
    {
        return await _productBundleItemRepository.AddAsync(bundleItem);
    }

    public async Task UpdateProductBundleItemAsync(ProductBundleItem bundleItem)
    {
        await _productBundleItemRepository.UpdateAsync(bundleItem);
    }

    public async Task DeleteProductBundleItemAsync(int bundleItemId)
    {
        await _productBundleItemRepository.DeleteAsync(bundleItemId);
    }

    public async Task<decimal> GetBundleItemPriceAsync(ProductBundleItem bundleItem, decimal productPrice)
    {
        if (bundleItem == null)
            return 0;

        return bundleItem.BundleItemType switch
        {
            BundleItemType.FixedPrice => bundleItem.Price,
            BundleItemType.DiscountPercentage => productPrice * (1 - bundleItem.DiscountPercentage / 100),
            _ => productPrice
        };
    }

    #endregion

    #region Back In Stock Subscriptions

    public async Task<BackInStockSubscription> GetBackInStockSubscriptionByIdAsync(int subscriptionId)
    {
        return await _backInStockSubscriptionRepository.GetAsync(subscriptionId);
    }

    public async Task<IEnumerable<BackInStockSubscription>> GetBackInStockSubscriptionsByProductIdAsync(int productId)
    {
        return await _backInStockSubscriptionRepository.GetAllAsync(query =>
            query.Where(biss => biss.ProductId == productId)
                 .OrderByDescending(biss => biss.CreatedOnUtc));
    }

    public async Task<BackInStockSubscription> CreateBackInStockSubscriptionAsync(BackInStockSubscription subscription)
    {
        return await _backInStockSubscriptionRepository.AddAsync(subscription);
    }

    public async Task DeleteBackInStockSubscriptionAsync(int subscriptionId)
    {
        await _backInStockSubscriptionRepository.DeleteAsync(subscriptionId);
    }

    public async Task SendNotificationsToSubscribersAsync(Product product)
    {
        if (product == null || !product.AllowBackInStockSubscriptions)
            return;

        var subscriptions = await GetBackInStockSubscriptionsByProductIdAsync(product.Id);
        foreach (var subscription in subscriptions)
        {
            await _notificationService.SendBackInStockNotificationAsync(subscription);
            await DeleteBackInStockSubscriptionAsync(subscription.Id);
        }
    }

    #endregion

    #region Product Availability Range

    public async Task<ProductAvailabilityRange> GetProductAvailabilityRangeByIdAsync(int availabilityRangeId)
    {
        return await _productAvailabilityRangeRepository.GetAsync(availabilityRangeId);
    }

    public async Task<IEnumerable<ProductAvailabilityRange>> GetAllProductAvailabilityRangesAsync()
    {
        return await _productAvailabilityRangeRepository.GetAllAsync();
    }

    public async Task<ProductAvailabilityRange> CreateProductAvailabilityRangeAsync(ProductAvailabilityRange availabilityRange)
    {
        return await _productAvailabilityRangeRepository.AddAsync(availabilityRange);
    }

    public async Task UpdateProductAvailabilityRangeAsync(ProductAvailabilityRange availabilityRange)
    {
        await _productAvailabilityRangeRepository.UpdateAsync(availabilityRange);
    }

    public async Task DeleteProductAvailabilityRangeAsync(int availabilityRangeId)
    {
        await _productAvailabilityRangeRepository.DeleteAsync(availabilityRangeId);
    }

    #endregion

    #region Product Base Price Management

    public async Task<decimal> CalculateBasePriceAsync(Product product)
    {
        if (product == null || !product.BasepriceEnabled)
            return 0;

        if (product.BasepriceAmount <= 0)
            return 0;

        var basePrice = product.BasepriceAmount;
        if (product.BasepriceBaseAmount > 0)
        {
            basePrice = (product.BasepriceAmount * product.BasepriceBaseAmount) / product.BasepriceBaseUnitId;
        }

        return basePrice;
    }

    public async Task<string> FormatBasePriceAsync(Product product)
    {
        if (product == null || !product.BasepriceEnabled)
            return string.Empty;

        var basePrice = await CalculateBasePriceAsync(product);
        if (basePrice <= 0)
            return string.Empty;

        return $"{basePrice:C} per {product.BasepriceUnitId}";
    }

    #endregion

    #region Product Mark as New Management

    public async Task<bool> IsProductMarkedAsNewAsync(Product product)
    {
        if (product == null || !product.MarkAsNew)
            return false;

        if (product.MarkAsNewStartDateTimeUtc.HasValue && DateTime.UtcNow < product.MarkAsNewStartDateTimeUtc.Value)
            return false;

        if (product.MarkAsNewEndDateTimeUtc.HasValue && DateTime.UtcNow > product.MarkAsNewEndDateTimeUtc.Value)
            return false;

        return true;
    }

    public async Task<DateTime?> GetMarkAsNewStartDateAsync(Product product)
    {
        if (product == null || !product.MarkAsNew)
            return null;

        return product.MarkAsNewStartDateTimeUtc;
    }

    public async Task<DateTime?> GetMarkAsNewEndDateAsync(Product product)
    {
        if (product == null || !product.MarkAsNew)
            return null;

        return product.MarkAsNewEndDateTimeUtc;
    }

    #endregion

    #region Product Backorder Management

    public async Task<bool> IsBackorderAllowedAsync(Product product)
    {
        if (product == null)
            return false;

        return product.BackorderMode != BackorderMode.NoBackorders;
    }

    public async Task<bool> ShouldNotifyCustomerForBackorderAsync(Product product)
    {
        if (product == null)
            return false;

        return product.BackorderMode == BackorderMode.AllowQtyBelow0AndNotifyCustomer;
    }

    public async Task<bool> ShouldNotifyAdminForBackorderAsync(Product product)
    {
        if (product == null)
            return false;

        return product.BackorderMode != BackorderMode.NoBackorders && 
               product.NotifyAdminForQuantityBelow.HasValue &&
               product.StockQuantity <= product.NotifyAdminForQuantityBelow.Value;
    }

    #endregion

    #region Product Inventory Management

    public async Task AdjustInventoryAsync(Product product, int quantityToChange, string attributesXml = "", string message = "")
    {
        if (product == null)
            return;

        if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock)
        {
            product.StockQuantity += quantityToChange;

            var historyEntry = new StockQuantityHistory
            {
                ProductId = product.Id,
                QuantityAdjustment = quantityToChange,
                StockQuantity = product.StockQuantity,
                Message = message,
                CreatedOnUtc = DateTime.UtcNow
            };
            await _stockQuantityHistoryRepository.AddAsync(historyEntry);

            if (product.NotifyAdminForQuantityBelow.HasValue &&
                product.StockQuantity <= product.NotifyAdminForQuantityBelow.Value)
            {
                await _notificationService.SendLowStockNotificationAsync(product);
            }

            if (product.StockQuantity > 0 && product.AllowBackInStockSubscriptions)
            {
                await SendNotificationsToSubscribersAsync(product);
            }

            await _productRepository.UpdateAsync(product);
        }
    }

    public async Task BookReservedInventoryAsync(Product product, int warehouseId, int quantity, string message = "")
    {
        if (product == null || quantity >= 0)
            return;

        if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock)
        {
            var warehouseInventory = await _productWarehouseInventoryRepository.GetAllAsync(query =>
                query.Where(pwi => pwi.ProductId == product.Id && pwi.WarehouseId == warehouseId)
                     .FirstOrDefaultAsync());

            if (warehouseInventory != null)
            {
                warehouseInventory.ReservedQuantity += Math.Abs(quantity);
                await _productWarehouseInventoryRepository.UpdateAsync(warehouseInventory);
            }
        }
    }

    public async Task UnbookReservedInventoryAsync(Product product, int warehouseId, int quantity)
    {
        if (product == null || quantity <= 0)
            return;

        if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock)
        {
            var warehouseInventory = await _productWarehouseInventoryRepository.GetAllAsync(query =>
                query.Where(pwi => pwi.ProductId == product.Id && pwi.WarehouseId == warehouseId)
                     .FirstOrDefaultAsync());

            if (warehouseInventory != null)
            {
                warehouseInventory.ReservedQuantity = Math.Max(0, warehouseInventory.ReservedQuantity - quantity);
                await _productWarehouseInventoryRepository.UpdateAsync(warehouseInventory);
            }
        }
    }

    #endregion

    #region Product Pre-order Management

    public async Task<bool> IsPreOrderAvailableAsync(Product product)
    {
        if (product == null || !product.AvailableForPreOrder)
            return false;

        if (product.PreOrderAvailabilityStartDateTimeUtc.HasValue)
        {
            if (DateTime.UtcNow < product.PreOrderAvailabilityStartDateTimeUtc.Value)
                return false;
        }

        return true;
    }

    public async Task<DateTime?> GetPreOrderAvailabilityDateAsync(Product product)
    {
        if (product == null || !product.AvailableForPreOrder)
            return null;

        return product.PreOrderAvailabilityStartDateTimeUtc;
    }

    #endregion

    #region Recently Viewed Products

    private readonly IList<int> _recentlyViewedProducts = new List<int>();
    private const int RecentlyViewedProductsNumber = 20;

    public async Task<IList<Product>> GetRecentlyViewedProductsAsync(int number)
    {
        var products = new List<Product>();
        var productIds = _recentlyViewedProducts.Take(number).ToList();

        foreach (var productId in productIds)
        {
            var product = await _productRepository.GetAsync(productId);
            if (product != null && !product.Deleted && product.Published)
                products.Add(product);
        }

        return products;
    }

    public async Task AddProductToRecentlyViewedListAsync(int productId)
    {
        if (productId <= 0)
            return;

        if (_recentlyViewedProducts.Contains(productId))
            _recentlyViewedProducts.Remove(productId);

        _recentlyViewedProducts.Insert(0, productId);

        while (_recentlyViewedProducts.Count > RecentlyViewedProductsNumber)
            _recentlyViewedProducts.RemoveAt(_recentlyViewedProducts.Count - 1);
    }

    #endregion

    #region Search Provider

    public async Task<List<int>> SearchProductsAsync(string keywords, bool isLocalized)
    {
        if (string.IsNullOrEmpty(keywords))
            return new List<int>();

        var query = _productRepository.GetAll()
            .Where(p => !p.Deleted && p.Published);

        if (isLocalized)
        {
            query = query.Where(p =>
                p.Name.Contains(keywords) ||
                p.ShortDescription.Contains(keywords) ||
                p.FullDescription.Contains(keywords) ||
                p.MetaKeywords.Contains(keywords) ||
                p.MetaDescription.Contains(keywords) ||
                p.MetaTitle.Contains(keywords) ||
                p.Sku.Contains(keywords) ||
                p.ManufacturerPartNumber.Contains(keywords) ||
                p.Gtin.Contains(keywords));
        }
        else
        {
            query = query.Where(p =>
                p.Name.Contains(keywords) ||
                p.Sku.Contains(keywords) ||
                p.ManufacturerPartNumber.Contains(keywords) ||
                p.Gtin.Contains(keywords));
        }

        return await query.Select(p => p.Id).ToListAsync();
    }

    #endregion
} 
