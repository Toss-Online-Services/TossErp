using Catalog.Domain.Entities;
using System.Text;
using System.Xml.Linq;

namespace Catalog.API.Services;

public class ProductAttributeFormatter : IProductAttributeFormatter
{
    private readonly IProductAttributeParser _productAttributeParser;
    private readonly IProductAttributeService _productAttributeService;
    private readonly IProductService _productService;

    public ProductAttributeFormatter(
        IProductAttributeParser productAttributeParser,
        IProductAttributeService productAttributeService,
        IProductService productService)
    {
        _productAttributeParser = productAttributeParser;
        _productAttributeService = productAttributeService;
        _productService = productService;
    }

    public async Task<string> FormatAttributesAsync(Product product, string attributesXml, Customer customer = null, string separator = "<br />", bool includePrices = true, bool includeHyperlinks = true)
    {
        return await FormatAttributesAsync(product, attributesXml, customer, separator, includePrices, includeHyperlinks, true, true, true, true);
    }

    public async Task<string> FormatAttributesAsync(Product product, string attributesXml, string separator = "<br />", bool includePrices = true, bool includeHyperlinks = true)
    {
        return await FormatAttributesAsync(product, attributesXml, null, separator, includePrices, includeHyperlinks);
    }

    public async Task<string> FormatAttributesAsync(Product product, string attributesXml, Customer customer = null, string separator = "<br />", bool includePrices = true, bool includeHyperlinks = true, bool includeGiftCardAttributes = true, bool includeHyperlinksForAssociatedProducts = true)
    {
        return await FormatAttributesAsync(product, attributesXml, customer, separator, includePrices, includeHyperlinks, includeGiftCardAttributes, includeHyperlinksForAssociatedProducts, true, true);
    }

    public async Task<string> FormatAttributesAsync(Product product, string attributesXml, string separator = "<br />", bool includePrices = true, bool includeHyperlinks = true, bool includeGiftCardAttributes = true, bool includeHyperlinksForAssociatedProducts = true)
    {
        return await FormatAttributesAsync(product, attributesXml, null, separator, includePrices, includeHyperlinks, includeGiftCardAttributes, includeHyperlinksForAssociatedProducts);
    }

    public async Task<string> FormatAttributesAsync(Product product, string attributesXml, Customer customer = null, string separator = "<br />", bool includePrices = true, bool includeHyperlinks = true, bool includeGiftCardAttributes = true, bool includeHyperlinksForAssociatedProducts = true, bool includeDownloadAttributes = true)
    {
        return await FormatAttributesAsync(product, attributesXml, customer, separator, includePrices, includeHyperlinks, includeGiftCardAttributes, includeHyperlinksForAssociatedProducts, includeDownloadAttributes, true);
    }

    public async Task<string> FormatAttributesAsync(Product product, string attributesXml, string separator = "<br />", bool includePrices = true, bool includeHyperlinks = true, bool includeGiftCardAttributes = true, bool includeHyperlinksForAssociatedProducts = true, bool includeDownloadAttributes = true)
    {
        return await FormatAttributesAsync(product, attributesXml, null, separator, includePrices, includeHyperlinks, includeGiftCardAttributes, includeHyperlinksForAssociatedProducts, includeDownloadAttributes);
    }

    public async Task<string> FormatAttributesAsync(Product product, string attributesXml, Customer customer = null, string separator = "<br />", bool includePrices = true, bool includeHyperlinks = true, bool includeGiftCardAttributes = true, bool includeHyperlinksForAssociatedProducts = true, bool includeDownloadAttributes = true, bool includeFileUploadAttributes = true)
    {
        var result = new StringBuilder();

        if (string.IsNullOrEmpty(attributesXml))
            return result.ToString();

        var attributes = await _productAttributeParser.ParseProductAttributesAsync(attributesXml);
        var values = await _productAttributeParser.ParseProductAttributeValuesAsync(attributesXml);

        for (var i = 0; i < attributes.Count; i++)
        {
            var attribute = attributes[i];
            var valuesStr = new StringBuilder();

            if (attribute.ShouldHaveValues())
            {
                var attributeValues = values.Where(x => x.ProductAttributeId == attribute.Id).ToList();
                for (var j = 0; j < attributeValues.Count; j++)
                {
                    var value = attributeValues[j];
                    var formattedAttribute = "";

                    if (attribute.AttributeControlType == AttributeControlType.FileUpload)
                    {
                        if (!includeFileUploadAttributes)
                            continue;

                        // TODO: Implement file upload attribute formatting
                    }
                    else
                    {
                        if (includePrices && value.PriceAdjustment > 0)
                        {
                            formattedAttribute = $"{value.Name} [+{value.PriceAdjustment:C}]";
                        }
                        else
                        {
                            formattedAttribute = value.Name;
                        }
                    }

                    if (j > 0)
                        valuesStr.Append(", ");

                    valuesStr.Append(formattedAttribute);
                }
            }
            else
            {
                // No values
                if (attribute.AttributeControlType == AttributeControlType.MultilineTextbox)
                {
                    // TODO: Implement multiline textbox attribute formatting
                }
                else if (attribute.AttributeControlType == AttributeControlType.FileUpload)
                {
                    if (!includeFileUploadAttributes)
                        continue;

                    // TODO: Implement file upload attribute formatting
                }
                else
                {
                    // Other attributes (textbox, datepicker)
                    valuesStr.Append(attribute.Name);
                }
            }

            if (string.IsNullOrEmpty(valuesStr.ToString()))
                continue;

            if (i > 0)
                result.Append(separator);

            result.Append($"{attribute.Name}: {valuesStr}");
        }

        return result.ToString();
    }

    public async Task<string> FormatAttributesAsync(Product product, string attributesXml, string separator = "<br />", bool includePrices = true, bool includeHyperlinks = true, bool includeGiftCardAttributes = true, bool includeHyperlinksForAssociatedProducts = true, bool includeDownloadAttributes = true, bool includeFileUploadAttributes = true)
    {
        return await FormatAttributesAsync(product, attributesXml, null, separator, includePrices, includeHyperlinks, includeGiftCardAttributes, includeHyperlinksForAssociatedProducts, includeDownloadAttributes, includeFileUploadAttributes);
    }
} 
