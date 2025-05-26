using Catalog.Domain.AggregatesModel.CatalogAggregate;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;

namespace Catalog.API.Services;

public interface IProductService
{
    #region Core Product Operations

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

    #endregion

    #region Product Filtering

    Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
    Task<IEnumerable<Product>> GetProductsByManufacturerAsync(int manufacturerId);
    Task<IEnumerable<Product>> GetProductsByTagAsync(int tagId);
    Task<IEnumerable<Product>> GetProductsByVendorAsync(int vendorId);
    Task<IEnumerable<Product>> GetProductsByWarehouseAsync(int warehouseId);
    Task<IEnumerable<Product>> GetProductsByDiscountAsync(int discountId);
    Task<IEnumerable<Product>> GetProductsBySpecificationAttributeAsync(int specificationAttributeId);
    Task<IEnumerable<Product>> GetProductsByAttributeAsync(int attributeId);
    Task<IEnumerable<Product>> GetProductsByAttributeValueAsync(int attributeValueId);
    Task<IEnumerable<Product>> GetProductsByAttributeCombinationAsync(int attributeCombinationId);
    Task<IEnumerable<Product>> GetProductsByRelatedProductAsync(int relatedProductId);
    Task<IEnumerable<Product>> GetProductsByCrossSellProductAsync(int crossSellProductId);
    Task<IEnumerable<Product>> GetProductsByAssociatedProductAsync(int associatedProductId);
    Task<IEnumerable<Product>> GetProductsByPictureAsync(int pictureId);
    Task<IEnumerable<Product>> GetProductsByVideoAsync(int videoId);
    Task<IEnumerable<Product>> GetProductsByReviewAsync(int reviewId);
    Task<IEnumerable<Product>> GetProductsByTierPriceAsync(int tierPriceId);
    Task<IEnumerable<Product>> GetProductsByWarehouseInventoryAsync(int warehouseInventoryId);

    #endregion

    #region Product Search

    Task<IEnumerable<Product>> SearchProductsAsync(
        string keywords = null,
        int? categoryId = null,
        int? manufacturerId = null,
        int? tagId = null,
        decimal? priceMin = null,
        decimal? priceMax = null,
        bool searchDescriptions = false,
        bool searchSku = true,
        bool searchTags = false,
        bool showHidden = false);

    #endregion

    #region Product Attributes

    Task<string> FormatAttributesAsync(Product product, string attributesXml, bool includePrices = true);
    Task<IList<ProductAttribute>> ParseProductAttributesAsync(string attributesXml);
    Task<IList<ProductAttributeValue>> ParseProductAttributeValuesAsync(string attributesXml);
    Task<string> AddProductAttributeAsync(string attributesXml, ProductAttribute productAttribute, string value);
    Task<string> RemoveProductAttributeAsync(string attributesXml, ProductAttribute productAttribute);
    Task<bool> AreProductAttributesEqualAsync(string attributesXml1, string attributesXml2);
    Task<bool> IsConditionMetAsync(Product product, string attributesXml);

    #endregion

    #region Product Reviews

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

    Task<ProductReview> GetProductReviewByIdAsync(int productReviewId);
    Task<IList<ProductReview>> GetProductReviewsByIdsAsync(int[] productReviewIds);
    Task InsertProductReviewAsync(ProductReview productReview);
    Task DeleteProductReviewAsync(ProductReview productReview);
    Task DeleteProductReviewsAsync(IList<ProductReview> productReviews);
    Task UpdateProductReviewAsync(ProductReview productReview);
    Task<bool> CanAddReviewAsync(int productId, int storeId = 0);

    #endregion

    #region Product Pictures

    Task<IList<ProductPicture>> GetProductPicturesByProductIdAsync(int productId);
    Task<ProductPicture> GetProductPictureByIdAsync(int productPictureId);
    Task InsertProductPictureAsync(ProductPicture productPicture);
    Task UpdateProductPictureAsync(ProductPicture productPicture);
    Task DeleteProductPictureAsync(ProductPicture productPicture);
    Task<IDictionary<int, int[]>> GetProductsImagesIdsAsync(int[] productsIds);

    #endregion

    #region Product Videos

    Task<IList<ProductVideo>> GetProductVideosByProductIdAsync(int productId);
    Task<ProductVideo> GetProductVideoByIdAsync(int productVideoId);
    Task InsertProductVideoAsync(ProductVideo productVideo);
    Task UpdateProductVideoAsync(ProductVideo productVideo);
    Task DeleteProductVideoAsync(ProductVideo productVideo);

    #endregion

    #region Product Warehouses

    Task<IList<ProductWarehouseInventory>> GetAllProductWarehouseInventoryRecordsAsync(int productId);
    Task InsertProductWarehouseInventoryAsync(ProductWarehouseInventory pwi);
    Task UpdateProductWarehouseInventoryAsync(ProductWarehouseInventory pwi);
    Task DeleteProductWarehouseInventoryAsync(ProductWarehouseInventory pwi);

    #endregion

    #region Product Discounts

    Task<IList<DiscountProductMapping>> GetAllDiscountsAppliedToProductAsync(int productId);
    Task<DiscountProductMapping> GetDiscountAppliedToProductAsync(int productId, int discountId);
    Task InsertDiscountProductMappingAsync(DiscountProductMapping discountProductMapping);
    Task DeleteDiscountProductMappingAsync(DiscountProductMapping discountProductMapping);
    Task ClearDiscountProductMappingAsync(Discount discount);

    #endregion

    #region Related Products

    Task<IList<RelatedProduct>> GetRelatedProductsByProductId1Async(int productId1, bool showHidden = false);
    Task<RelatedProduct> GetRelatedProductByIdAsync(int relatedProductId);
    Task InsertRelatedProductAsync(RelatedProduct relatedProduct);
    Task UpdateRelatedProductAsync(RelatedProduct relatedProduct);
    Task DeleteRelatedProductAsync(RelatedProduct relatedProduct);

    #endregion

    #region Cross-sell Products

    Task<IList<CrossSellProduct>> GetCrossSellProductsByProductId1Async(int productId1, bool showHidden = false);
    Task<CrossSellProduct> GetCrossSellProductByIdAsync(int crossSellProductId);
    Task InsertCrossSellProductAsync(CrossSellProduct crossSellProduct);
    Task DeleteCrossSellProductAsync(CrossSellProduct crossSellProduct);
    Task<IList<Product>> GetCrossSellProductsByShoppingCartAsync(IList<ShoppingCartItem> cart, int numberOfProducts);

    #endregion

    #region Tier Prices

    Task<IList<TierPrice>> GetTierPricesAsync(Product product, Customer customer, Store store);
    Task<IList<TierPrice>> GetTierPricesByProductAsync(int productId);
    Task<TierPrice> GetTierPriceByIdAsync(int tierPriceId);
    Task InsertTierPriceAsync(TierPrice tierPrice);
    Task UpdateTierPriceAsync(TierPrice tierPrice);
    Task DeleteTierPriceAsync(TierPrice tierPrice);
    Task<TierPrice> GetPreferredTierPriceAsync(Product product, Customer customer, Store store, int quantity);

    #endregion
} 
