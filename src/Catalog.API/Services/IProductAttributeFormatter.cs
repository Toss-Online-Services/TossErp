using Catalog.Domain.Entities;

namespace Catalog.API.Services;

public interface IProductAttributeFormatter
{
    Task<string> FormatAttributesAsync(Product product, string attributesXml, Customer customer = null, string separator = "<br />", bool includePrices = true, bool includeHyperlinks = true);
    Task<string> FormatAttributesAsync(Product product, string attributesXml, string separator = "<br />", bool includePrices = true, bool includeHyperlinks = true);
    Task<string> FormatAttributesAsync(Product product, string attributesXml, Customer customer = null, string separator = "<br />", bool includePrices = true, bool includeHyperlinks = true, bool includeGiftCardAttributes = true, bool includeHyperlinksForAssociatedProducts = true);
    Task<string> FormatAttributesAsync(Product product, string attributesXml, string separator = "<br />", bool includePrices = true, bool includeHyperlinks = true, bool includeGiftCardAttributes = true, bool includeHyperlinksForAssociatedProducts = true);
    Task<string> FormatAttributesAsync(Product product, string attributesXml, Customer customer = null, string separator = "<br />", bool includePrices = true, bool includeHyperlinks = true, bool includeGiftCardAttributes = true, bool includeHyperlinksForAssociatedProducts = true, bool includeDownloadAttributes = true);
    Task<string> FormatAttributesAsync(Product product, string attributesXml, string separator = "<br />", bool includePrices = true, bool includeHyperlinks = true, bool includeGiftCardAttributes = true, bool includeHyperlinksForAssociatedProducts = true, bool includeDownloadAttributes = true);
    Task<string> FormatAttributesAsync(Product product, string attributesXml, Customer customer = null, string separator = "<br />", bool includePrices = true, bool includeHyperlinks = true, bool includeGiftCardAttributes = true, bool includeHyperlinksForAssociatedProducts = true, bool includeDownloadAttributes = true, bool includeFileUploadAttributes = true);
    Task<string> FormatAttributesAsync(Product product, string attributesXml, string separator = "<br />", bool includePrices = true, bool includeHyperlinks = true, bool includeGiftCardAttributes = true, bool includeHyperlinksForAssociatedProducts = true, bool includeDownloadAttributes = true, bool includeFileUploadAttributes = true);
} 
