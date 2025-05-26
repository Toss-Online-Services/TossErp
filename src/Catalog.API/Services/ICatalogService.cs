using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;

namespace Catalog.API.Services;

/// <summary>
/// Catalog service interface
/// </summary>
public interface ICatalogService
{
    #region Products

    Task<Product> GetProductByIdAsync(int productId);
    Task<IEnumerable<Product>> GetAllProductsAsync();
    Task<IEnumerable<Product>> GetAllProductsDisplayedOnHomepageAsync();
    Task<Product> GetProductBySkuAsync(string sku);
    Task<IEnumerable<Product>> GetProductsBySkuAsync(string[] skuArray, int vendorId = 0);
    Task<Product> CreateProductAsync(Product product);
    Task UpdateProductAsync(Product product);
    Task DeleteProductAsync(int productId);
    Task DeleteProductsAsync(IEnumerable<Product> products);
    Task<bool> ProductIsAvailableAsync(Product product, DateTime? dateTime = null);
    Task<int> GetNumberOfProductsInCategoryAsync(IList<int> categoryIds = null, int storeId = 0);
    Task<int> GetNumberOfProductsByVendorIdAsync(int vendorId);
    Task<bool> HasAnyDownloadableProductAsync(int[] productIds);
    Task<bool> HasAnyGiftCardProductAsync(int[] productIds);
    Task<bool> HasAnyRecurringProductAsync(int[] productIds);
    Task<string[]> GetNotExistingProductsAsync(string[] productSku);

    #endregion

    #region Categories

    Task<Category> GetCategoryByIdAsync(int categoryId);
    Task<IEnumerable<Category>> GetAllCategoriesAsync();
    Task<IEnumerable<Category>> GetCategoriesByParentIdAsync(int parentId);
    Task<Category> CreateCategoryAsync(Category category);
    Task UpdateCategoryAsync(Category category);
    Task DeleteCategoryAsync(int categoryId);
    Task DeleteCategoriesAsync(IEnumerable<Category> categories);
    Task<IEnumerable<Category>> GetCategoriesByProductIdAsync(int productId);
    Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(int categoryId, bool showHidden = false);

    #endregion

    #region Manufacturers

    Task<Manufacturer> GetManufacturerByIdAsync(int manufacturerId);
    Task<IEnumerable<Manufacturer>> GetAllManufacturersAsync();
    Task<Manufacturer> CreateManufacturerAsync(Manufacturer manufacturer);
    Task UpdateManufacturerAsync(Manufacturer manufacturer);
    Task DeleteManufacturerAsync(int manufacturerId);
    Task DeleteManufacturersAsync(IEnumerable<Manufacturer> manufacturers);
    Task<IEnumerable<Manufacturer>> GetManufacturersByProductIdAsync(int productId);
    Task<IEnumerable<Product>> GetProductsByManufacturerIdAsync(int manufacturerId, bool showHidden = false);

    #endregion

    #region Product Attributes

    Task<ProductAttribute> GetProductAttributeByIdAsync(int productAttributeId);
    Task<IEnumerable<ProductAttribute>> GetAllProductAttributesAsync();
    Task<ProductAttribute> CreateProductAttributeAsync(ProductAttribute productAttribute);
    Task UpdateProductAttributeAsync(ProductAttribute productAttribute);
    Task DeleteProductAttributeAsync(int productAttributeId);
    Task DeleteProductAttributesAsync(IEnumerable<ProductAttribute> productAttributes);
    Task<IEnumerable<ProductAttribute>> GetProductAttributesByProductIdAsync(int productId);
    Task<string> FormatAttributesAsync(Product product, string attributesXml, bool includePrices = true);
    Task<IList<ProductAttribute>> ParseProductAttributesAsync(string attributesXml);
    Task<IList<ProductAttributeValue>> ParseProductAttributeValuesAsync(string attributesXml);
    Task<string> AddProductAttributeAsync(string attributesXml, ProductAttribute productAttribute, string value);
    Task<string> RemoveProductAttributeAsync(string attributesXml, ProductAttribute productAttribute);
    Task<bool> AreProductAttributesEqualAsync(string attributesXml1, string attributesXml2);
    Task<bool> IsConditionMetAsync(Product product, string attributesXml);

    #endregion

    #region Product Pictures

    Task<ProductPicture> GetProductPictureByIdAsync(int productPictureId);
    Task<IEnumerable<ProductPicture>> GetProductPicturesByProductIdAsync(int productId);
    Task<ProductPicture> CreateProductPictureAsync(ProductPicture productPicture);
    Task UpdateProductPictureAsync(ProductPicture productPicture);
    Task DeleteProductPictureAsync(int productPictureId);
    Task DeleteProductPicturesAsync(IEnumerable<ProductPicture> productPictures);
    Task<IDictionary<int, int[]>> GetProductsImagesIdsAsync(int[] productsIds);

    #endregion

    #region Product Videos

    Task<ProductVideo> GetProductVideoByIdAsync(int productVideoId);
    Task<IEnumerable<ProductVideo>> GetProductVideosByProductIdAsync(int productId);
    Task<ProductVideo> CreateProductVideoAsync(ProductVideo productVideo);
    Task UpdateProductVideoAsync(ProductVideo productVideo);
    Task DeleteProductVideoAsync(int productVideoId);
    Task DeleteProductVideosAsync(IEnumerable<ProductVideo> productVideos);

    #endregion

    #region Product Reviews

    Task<ProductReview> GetProductReviewByIdAsync(int productReviewId);
    Task<IEnumerable<ProductReview>> GetAllProductReviewsAsync(
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
        int pageSize = int.MaxValue);
    Task<ProductReview> CreateProductReviewAsync(ProductReview productReview);
    Task UpdateProductReviewAsync(ProductReview productReview);
    Task DeleteProductReviewAsync(int productReviewId);
    Task DeleteProductReviewsAsync(IEnumerable<ProductReview> productReviews);
    Task<bool> CanAddReviewAsync(int productId, int storeId = 0);

    #endregion

    #region Product Tags

    Task<ProductTag> GetProductTagByIdAsync(int productTagId);
    Task<IEnumerable<ProductTag>> GetAllProductTagsAsync();
    Task<ProductTag> CreateProductTagAsync(ProductTag productTag);
    Task UpdateProductTagAsync(ProductTag productTag);
    Task DeleteProductTagAsync(int productTagId);
    Task DeleteProductTagsAsync(IEnumerable<ProductTag> productTags);
    Task<IEnumerable<ProductTag>> GetProductTagsByProductIdAsync(int productId);
    Task<IEnumerable<Product>> GetProductsByTagIdAsync(int tagId, bool showHidden = false);

    #endregion

    #region Product Warehouses

    Task<ProductWarehouseInventory> GetProductWarehouseInventoryByIdAsync(int productWarehouseInventoryId);
    Task<IEnumerable<ProductWarehouseInventory>> GetProductWarehouseInventoriesByProductIdAsync(int productId);
    Task<ProductWarehouseInventory> CreateProductWarehouseInventoryAsync(ProductWarehouseInventory productWarehouseInventory);
    Task UpdateProductWarehouseInventoryAsync(ProductWarehouseInventory productWarehouseInventory);
    Task DeleteProductWarehouseInventoryAsync(int productWarehouseInventoryId);
    Task DeleteProductWarehouseInventoriesAsync(IEnumerable<ProductWarehouseInventory> productWarehouseInventories);
    Task<int> GetTotalStockQuantityAsync(Product product, bool useReservedQuantity = true, int warehouseId = 0);

    #endregion

    #region Related Products

    Task<RelatedProduct> GetRelatedProductByIdAsync(int relatedProductId);
    Task<IEnumerable<RelatedProduct>> GetRelatedProductsByProductIdAsync(int productId, bool showHidden = false);
    Task<RelatedProduct> CreateRelatedProductAsync(RelatedProduct relatedProduct);
    Task UpdateRelatedProductAsync(RelatedProduct relatedProduct);
    Task DeleteRelatedProductAsync(int relatedProductId);
    Task DeleteRelatedProductsAsync(IEnumerable<RelatedProduct> relatedProducts);

    #endregion

    #region Cross-sell Products

    Task<CrossSellProduct> GetCrossSellProductByIdAsync(int crossSellProductId);
    Task<IEnumerable<CrossSellProduct>> GetCrossSellProductsByProductIdAsync(int productId, bool showHidden = false);
    Task<CrossSellProduct> CreateCrossSellProductAsync(CrossSellProduct crossSellProduct);
    Task UpdateCrossSellProductAsync(CrossSellProduct crossSellProduct);
    Task DeleteCrossSellProductAsync(int crossSellProductId);
    Task DeleteCrossSellProductsAsync(IEnumerable<CrossSellProduct> crossSellProducts);
    Task<IEnumerable<Product>> GetCrossSellProductsByShoppingCartAsync(IList<ShoppingCartItem> cart, int numberOfProducts);

    #endregion

    #region Tier Prices

    Task<TierPrice> GetTierPriceByIdAsync(int tierPriceId);
    Task<IEnumerable<TierPrice>> GetTierPricesByProductIdAsync(int productId);
    Task<TierPrice> CreateTierPriceAsync(TierPrice tierPrice);
    Task UpdateTierPriceAsync(TierPrice tierPrice);
    Task DeleteTierPriceAsync(int tierPriceId);
    Task DeleteTierPricesAsync(IEnumerable<TierPrice> tierPrices);
    Task<TierPrice> GetPreferredTierPriceAsync(Product product, Customer customer, Store store, int quantity);

    #endregion

    #region Product Discounts

    Task<DiscountProductMapping> GetDiscountProductMappingByIdAsync(int discountProductMappingId);
    Task<IEnumerable<DiscountProductMapping>> GetDiscountProductMappingsByProductIdAsync(int productId);
    Task<DiscountProductMapping> CreateDiscountProductMappingAsync(DiscountProductMapping discountProductMapping);
    Task UpdateDiscountProductMappingAsync(DiscountProductMapping discountProductMapping);
    Task DeleteDiscountProductMappingAsync(int discountProductMappingId);
    Task DeleteDiscountProductMappingsAsync(IEnumerable<DiscountProductMapping> discountProductMappings);
    Task ClearDiscountProductMappingAsync(Discount discount);

    #endregion

    #region Product Specifications

    Task<SpecificationAttribute> GetSpecificationAttributeByIdAsync(int specificationAttributeId);
    Task<IEnumerable<SpecificationAttribute>> GetAllSpecificationAttributesAsync();
    Task<SpecificationAttribute> CreateSpecificationAttributeAsync(SpecificationAttribute specificationAttribute);
    Task UpdateSpecificationAttributeAsync(SpecificationAttribute specificationAttribute);
    Task DeleteSpecificationAttributeAsync(int specificationAttributeId);
    Task DeleteSpecificationAttributesAsync(IEnumerable<SpecificationAttribute> specificationAttributes);
    Task<IEnumerable<SpecificationAttribute>> GetSpecificationAttributesByProductIdAsync(int productId);

    #endregion

    #region Product Templates

    Task<ProductTemplate> GetProductTemplateByIdAsync(int productTemplateId);
    Task<IEnumerable<ProductTemplate>> GetAllProductTemplatesAsync();
    Task<ProductTemplate> CreateProductTemplateAsync(ProductTemplate productTemplate);
    Task UpdateProductTemplateAsync(ProductTemplate productTemplate);
    Task DeleteProductTemplateAsync(int productTemplateId);
    Task DeleteProductTemplatesAsync(IEnumerable<ProductTemplate> productTemplates);

    #endregion
} 
