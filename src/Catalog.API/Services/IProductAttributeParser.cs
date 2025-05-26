using Catalog.Domain.Entities;

namespace Catalog.API.Services;

public interface IProductAttributeParser
{
    Task<IList<ProductAttribute>> ParseProductAttributesAsync(string attributesXml);
    Task<IList<ProductAttributeValue>> ParseProductAttributeValuesAsync(string attributesXml);
    Task<IList<ProductAttributeValue>> ParseProductAttributeValuesAsync(Product product, string attributesXml);
    Task<IList<ProductAttributeValue>> ParseProductAttributeValuesAsync(Product product, string attributesXml, bool includeNonCombinableAttributes = true);
    Task<IList<ProductAttributeValue>> ParseProductAttributeValuesAsync(Product product, string attributesXml, bool includeNonCombinableAttributes = true, bool includeHiddenAttributes = true);
    Task<IList<ProductAttributeValue>> ParseProductAttributeValuesAsync(Product product, string attributesXml, bool includeNonCombinableAttributes = true, bool includeHiddenAttributes = true, bool includeGiftCardAttributes = true);
    Task<IList<ProductAttributeValue>> ParseProductAttributeValuesAsync(Product product, string attributesXml, bool includeNonCombinableAttributes = true, bool includeHiddenAttributes = true, bool includeGiftCardAttributes = true, bool includeDownloadAttributes = true);
    Task<IList<ProductAttributeValue>> ParseProductAttributeValuesAsync(Product product, string attributesXml, bool includeNonCombinableAttributes = true, bool includeHiddenAttributes = true, bool includeGiftCardAttributes = true, bool includeDownloadAttributes = true, bool includeFileUploadAttributes = true);
    Task<string> AddProductAttributeAsync(string attributesXml, ProductAttribute productAttribute, string value);
    Task<string> AddProductAttributeAsync(string attributesXml, ProductAttribute productAttribute, int valueId);
    Task<string> AddProductAttributeAsync(string attributesXml, ProductAttribute productAttribute, IList<int> valueIds);
    Task<string> AddProductAttributeAsync(string attributesXml, ProductAttribute productAttribute, IList<string> values);
    Task<string> AddProductAttributeAsync(string attributesXml, ProductAttribute productAttribute, IList<ProductAttributeValue> values);
    Task<string> AddProductAttributeAsync(string attributesXml, ProductAttribute productAttribute, IList<ProductAttributeValue> values, bool includeNonCombinableAttributes = true);
    Task<string> AddProductAttributeAsync(string attributesXml, ProductAttribute productAttribute, IList<ProductAttributeValue> values, bool includeNonCombinableAttributes = true, bool includeHiddenAttributes = true);
    Task<string> AddProductAttributeAsync(string attributesXml, ProductAttribute productAttribute, IList<ProductAttributeValue> values, bool includeNonCombinableAttributes = true, bool includeHiddenAttributes = true, bool includeGiftCardAttributes = true);
    Task<string> AddProductAttributeAsync(string attributesXml, ProductAttribute productAttribute, IList<ProductAttributeValue> values, bool includeNonCombinableAttributes = true, bool includeHiddenAttributes = true, bool includeGiftCardAttributes = true, bool includeDownloadAttributes = true);
    Task<string> AddProductAttributeAsync(string attributesXml, ProductAttribute productAttribute, IList<ProductAttributeValue> values, bool includeNonCombinableAttributes = true, bool includeHiddenAttributes = true, bool includeGiftCardAttributes = true, bool includeDownloadAttributes = true, bool includeFileUploadAttributes = true);
    Task<string> RemoveProductAttributeAsync(string attributesXml, ProductAttribute productAttribute);
    Task<string> RemoveProductAttributeAsync(string attributesXml, ProductAttribute productAttribute, int valueId);
    Task<string> RemoveProductAttributeAsync(string attributesXml, ProductAttribute productAttribute, IList<int> valueIds);
    Task<string> RemoveProductAttributeAsync(string attributesXml, ProductAttribute productAttribute, IList<string> values);
    Task<string> RemoveProductAttributeAsync(string attributesXml, ProductAttribute productAttribute, IList<ProductAttributeValue> values);
    Task<string> RemoveProductAttributeAsync(string attributesXml, ProductAttribute productAttribute, IList<ProductAttributeValue> values, bool includeNonCombinableAttributes = true);
    Task<string> RemoveProductAttributeAsync(string attributesXml, ProductAttribute productAttribute, IList<ProductAttributeValue> values, bool includeNonCombinableAttributes = true, bool includeHiddenAttributes = true);
    Task<string> RemoveProductAttributeAsync(string attributesXml, ProductAttribute productAttribute, IList<ProductAttributeValue> values, bool includeNonCombinableAttributes = true, bool includeHiddenAttributes = true, bool includeGiftCardAttributes = true);
    Task<string> RemoveProductAttributeAsync(string attributesXml, ProductAttribute productAttribute, IList<ProductAttributeValue> values, bool includeNonCombinableAttributes = true, bool includeHiddenAttributes = true, bool includeGiftCardAttributes = true, bool includeDownloadAttributes = true);
    Task<string> RemoveProductAttributeAsync(string attributesXml, ProductAttribute productAttribute, IList<ProductAttributeValue> values, bool includeNonCombinableAttributes = true, bool includeHiddenAttributes = true, bool includeGiftCardAttributes = true, bool includeDownloadAttributes = true, bool includeFileUploadAttributes = true);
    Task<bool> AreProductAttributesEqualAsync(string attributesXml1, string attributesXml2, bool ignoreNonCombinableAttributes = true);
    Task<bool> AreProductAttributesEqualAsync(string attributesXml1, string attributesXml2, bool ignoreNonCombinableAttributes = true, bool ignoreHiddenAttributes = true);
    Task<bool> AreProductAttributesEqualAsync(string attributesXml1, string attributesXml2, bool ignoreNonCombinableAttributes = true, bool ignoreHiddenAttributes = true, bool ignoreGiftCardAttributes = true);
    Task<bool> AreProductAttributesEqualAsync(string attributesXml1, string attributesXml2, bool ignoreNonCombinableAttributes = true, bool ignoreHiddenAttributes = true, bool ignoreGiftCardAttributes = true, bool ignoreDownloadAttributes = true);
    Task<bool> AreProductAttributesEqualAsync(string attributesXml1, string attributesXml2, bool ignoreNonCombinableAttributes = true, bool ignoreHiddenAttributes = true, bool ignoreGiftCardAttributes = true, bool ignoreDownloadAttributes = true, bool ignoreFileUploadAttributes = true);
    Task<bool> IsConditionMetAsync(Product product, string attributesXml);
    Task<bool> IsConditionMetAsync(Product product, string attributesXml, bool includeNonCombinableAttributes = true);
    Task<bool> IsConditionMetAsync(Product product, string attributesXml, bool includeNonCombinableAttributes = true, bool includeHiddenAttributes = true);
    Task<bool> IsConditionMetAsync(Product product, string attributesXml, bool includeNonCombinableAttributes = true, bool includeHiddenAttributes = true, bool includeGiftCardAttributes = true);
    Task<bool> IsConditionMetAsync(Product product, string attributesXml, bool includeNonCombinableAttributes = true, bool includeHiddenAttributes = true, bool includeGiftCardAttributes = true, bool includeDownloadAttributes = true);
    Task<bool> IsConditionMetAsync(Product product, string attributesXml, bool includeNonCombinableAttributes = true, bool includeHiddenAttributes = true, bool includeGiftCardAttributes = true, bool includeDownloadAttributes = true, bool includeFileUploadAttributes = true);
} 