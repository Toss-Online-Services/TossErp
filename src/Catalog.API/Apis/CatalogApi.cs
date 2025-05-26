using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Pgvector.EntityFrameworkCore;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using Catalog.Infrastructure.Repositories;

namespace eShop.Catalog.API;

public static class CatalogApi
{
    public static IEndpointRouteBuilder MapCatalogApi(this IEndpointRouteBuilder app)
    {
        // RouteGroupBuilder for catalog endpoints
        var vApi = app.NewVersionedApi("Catalog");
        var api = vApi.MapGroup("api/catalog").HasApiVersion(1, 0).HasApiVersion(2, 0);
        var v1 = vApi.MapGroup("api/catalog").HasApiVersion(1, 0);
        var v2 = vApi.MapGroup("api/catalog").HasApiVersion(2, 0);

        // Product endpoints
        v1.MapGet("/products", GetAllProductsV1)
            .WithName("GetAllProductsV1")
            .WithTags("Products")
            .WithDescription("Get all products with pagination");

        v2.MapGet("/products", GetAllProducts)
            .WithName("GetAllProducts")
            .WithTags("Products")
            .WithDescription("Get all products with filtering and pagination");

        v1.MapGet("/products/{id}", GetProductById)
            .WithName("GetProductById")
            .WithTags("Products")
            .WithDescription("Get a product by its ID");

        v1.MapPost("/products", CreateProduct)
            .WithName("CreateProduct")
            .WithTags("Products")
            .WithDescription("Create a new product");

        v1.MapPut("/products/{id}", UpdateProduct)
            .WithName("UpdateProduct")
            .WithTags("Products")
            .WithDescription("Update an existing product");

        v1.MapDelete("/products/{id}", DeleteProduct)
            .WithName("DeleteProduct")
            .WithTags("Products")
            .WithDescription("Delete a product");

        // Category endpoints
        v1.MapGet("/categories", GetAllCategories)
            .WithName("GetAllCategories")
            .WithTags("Categories")
            .WithDescription("Get all categories");

        v1.MapGet("/categories/{id}", GetCategoryById)
            .WithName("GetCategoryById")
            .WithTags("Categories")
            .WithDescription("Get a category by its ID");

        v1.MapPost("/categories", CreateCategory)
            .WithName("CreateCategory")
            .WithTags("Categories")
            .WithDescription("Create a new category");

        v1.MapPut("/categories/{id}", UpdateCategory)
            .WithName("UpdateCategory")
            .WithTags("Categories")
            .WithDescription("Update an existing category");

        v1.MapDelete("/categories/{id}", DeleteCategory)
            .WithName("DeleteCategory")
            .WithTags("Categories")
            .WithDescription("Delete a category");

        // Product Attribute endpoints
        v1.MapGet("/product-attributes", GetAllProductAttributes)
            .WithName("GetAllProductAttributes")
            .WithTags("ProductAttributes")
            .WithDescription("Get all product attributes");

        v1.MapGet("/product-attributes/{id}", GetProductAttributeById)
            .WithName("GetProductAttributeById")
            .WithTags("ProductAttributes")
            .WithDescription("Get a product attribute by its ID");

        v1.MapPost("/product-attributes", CreateProductAttribute)
            .WithName("CreateProductAttribute")
            .WithTags("ProductAttributes")
            .WithDescription("Create a new product attribute");

        v1.MapPut("/product-attributes/{id}", UpdateProductAttribute)
            .WithName("UpdateProductAttribute")
            .WithTags("ProductAttributes")
            .WithDescription("Update an existing product attribute");

        v1.MapDelete("/product-attributes/{id}", DeleteProductAttribute)
            .WithName("DeleteProductAttribute")
            .WithTags("ProductAttributes")
            .WithDescription("Delete a product attribute");

        // Product Attribute Mapping endpoints
        v1.MapGet("/products/{productId}/attributes", GetProductAttributeMappings)
            .WithName("GetProductAttributeMappings")
            .WithTags("ProductAttributeMappings")
            .WithDescription("Get all attribute mappings for a product");

        v1.MapPost("/products/{productId}/attributes", CreateProductAttributeMapping)
            .WithName("CreateProductAttributeMapping")
            .WithTags("ProductAttributeMappings")
            .WithDescription("Create a new attribute mapping for a product");

        v1.MapPut("/products/{productId}/attributes/{mappingId}", UpdateProductAttributeMapping)
            .WithName("UpdateProductAttributeMapping")
            .WithTags("ProductAttributeMappings")
            .WithDescription("Update an existing attribute mapping");

        v1.MapDelete("/products/{productId}/attributes/{mappingId}", DeleteProductAttributeMapping)
            .WithName("DeleteProductAttributeMapping")
            .WithTags("ProductAttributeMappings")
            .WithDescription("Delete an attribute mapping");

        // Product Attribute Value endpoints
        v1.MapGet("/product-attributes/{attributeId}/values", GetProductAttributeValues)
            .WithName("GetProductAttributeValues")
            .WithTags("ProductAttributeValues")
            .WithDescription("Get all values for a product attribute");

        v1.MapPost("/product-attributes/{attributeId}/values", CreateProductAttributeValue)
            .WithName("CreateProductAttributeValue")
            .WithTags("ProductAttributeValues")
            .WithDescription("Create a new value for a product attribute");

        v1.MapPut("/product-attributes/{attributeId}/values/{valueId}", UpdateProductAttributeValue)
            .WithName("UpdateProductAttributeValue")
            .WithTags("ProductAttributeValues")
            .WithDescription("Update an existing attribute value");

        v1.MapDelete("/product-attributes/{attributeId}/values/{valueId}", DeleteProductAttributeValue)
            .WithName("DeleteProductAttributeValue")
            .WithTags("ProductAttributeValues")
            .WithDescription("Delete an attribute value");

        // Product Picture endpoints
        v1.MapGet("/products/{productId}/pictures", GetProductPictures)
            .WithName("GetProductPictures")
            .WithTags("ProductPictures")
            .WithDescription("Get all pictures for a product");

        v1.MapPost("/products/{productId}/pictures", CreateProductPicture)
            .WithName("CreateProductPicture")
            .WithTags("ProductPictures")
            .WithDescription("Add a new picture to a product");

        v1.MapDelete("/products/{productId}/pictures/{pictureId}", DeleteProductPicture)
            .WithName("DeleteProductPicture")
            .WithTags("ProductPictures")
            .WithDescription("Delete a product picture");

        // Product Review endpoints
        v1.MapGet("/products/{productId}/reviews", GetProductReviews)
            .WithName("GetProductReviews")
            .WithTags("ProductReviews")
            .WithDescription("Get all reviews for a product");

        v1.MapPost("/products/{productId}/reviews", CreateProductReview)
            .WithName("CreateProductReview")
            .WithTags("ProductReviews")
            .WithDescription("Create a new review for a product");

        v1.MapPut("/products/{productId}/reviews/{reviewId}", UpdateProductReview)
            .WithName("UpdateProductReview")
            .WithTags("ProductReviews")
            .WithDescription("Update an existing product review");

        v1.MapDelete("/products/{productId}/reviews/{reviewId}", DeleteProductReview)
            .WithName("DeleteProductReview")
            .WithTags("ProductReviews")
            .WithDescription("Delete a product review");

        // Product Tag endpoints
        v1.MapGet("/product-tags", GetAllProductTags)
            .WithName("GetAllProductTags")
            .WithTags("ProductTags")
            .WithDescription("Get all product tags");

        v1.MapGet("/products/{productId}/tags", GetProductTags)
            .WithName("GetProductTags")
            .WithTags("ProductTags")
            .WithDescription("Get all tags for a product");

        v1.MapPost("/products/{productId}/tags", AddProductTag)
            .WithName("AddProductTag")
            .WithTags("ProductTags")
            .WithDescription("Add a tag to a product");

        v1.MapDelete("/products/{productId}/tags/{tagId}", RemoveProductTag)
            .WithName("RemoveProductTag")
            .WithTags("ProductTags")
            .WithDescription("Remove a tag from a product");

        // Related Product endpoints
        v1.MapGet("/products/{productId}/related", GetRelatedProducts)
            .WithName("GetRelatedProducts")
            .WithTags("RelatedProducts")
            .WithDescription("Get all related products");

        v1.MapPost("/products/{productId}/related", AddRelatedProduct)
            .WithName("AddRelatedProduct")
            .WithTags("RelatedProducts")
            .WithDescription("Add a related product");

        v1.MapDelete("/products/{productId}/related/{relatedProductId}", RemoveRelatedProduct)
            .WithName("RemoveRelatedProduct")
            .WithTags("RelatedProducts")
            .WithDescription("Remove a related product");

        // Cross-sell Product endpoints
        v1.MapGet("/products/{productId}/cross-sells", GetCrossSellProducts)
            .WithName("GetCrossSellProducts")
            .WithTags("CrossSellProducts")
            .WithDescription("Get all cross-sell products");

        v1.MapPost("/products/{productId}/cross-sells", AddCrossSellProduct)
            .WithName("AddCrossSellProduct")
            .WithTags("CrossSellProducts")
            .WithDescription("Add a cross-sell product");

        v1.MapDelete("/products/{productId}/cross-sells/{crossSellProductId}", RemoveCrossSellProduct)
            .WithName("RemoveCrossSellProduct")
            .WithTags("CrossSellProducts")
            .WithDescription("Remove a cross-sell product");

        // Bulk operation endpoints
        v1.MapPost("/products/bulk-edit", BulkEditProducts)
            .WithName("BulkEditProducts")
            .WithTags("BulkOperations")
            .WithDescription("Bulk edit multiple products");

        v1.MapPost("/products/bulk-delete", BulkDeleteProducts)
            .WithName("BulkDeleteProducts")
            .WithTags("BulkOperations")
            .WithDescription("Bulk delete multiple products");

        v1.MapPost("/products/bulk-publish", BulkPublishProducts)
            .WithName("BulkPublishProducts")
            .WithTags("BulkOperations")
            .WithDescription("Bulk publish multiple products");

        v1.MapPost("/products/bulk-unpublish", BulkUnpublishProducts)
            .WithName("BulkUnpublishProducts")
            .WithTags("BulkOperations")
            .WithDescription("Bulk unpublish multiple products");

        v1.MapPost("/products/attribute-combinations", GetProductAttributeCombinations)
            .WithName("GetProductAttributeCombinations")
            .WithTags("ProductAttributeCombinations")
            .WithDescription("Get all attribute combinations for a product");

        v1.MapPost("/products/attribute-combinations", CreateProductAttributeCombination)
            .WithName("CreateProductAttributeCombination")
            .WithTags("ProductAttributeCombinations")
            .WithDescription("Create a new attribute combination for a product");

        v1.MapPut("/products/attribute-combinations/{combinationId}", UpdateProductAttributeCombination)
            .WithName("UpdateProductAttributeCombination")
            .WithTags("ProductAttributeCombinations")
            .WithDescription("Update an existing attribute combination");

        v1.MapDelete("/products/attribute-combinations/{combinationId}", DeleteProductAttributeCombination)
            .WithName("DeleteProductAttributeCombination")
            .WithTags("ProductAttributeCombinations")
            .WithDescription("Delete an attribute combination");

        v1.MapPost("/products/generate-all-combinations", GenerateAllAttributeCombinations)
            .WithName("GenerateAllAttributeCombinations")
            .WithTags("ProductAttributeCombinations")
            .WithDescription("Generate all possible attribute combinations for a product");

        v1.MapPost("/products/tier-prices", GetTierPrices)
            .WithName("GetTierPrices")
            .WithTags("TierPrices")
            .WithDescription("Get all tier prices for a product");

        v1.MapPost("/products/tier-prices", CreateTierPrice)
            .WithName("CreateTierPrice")
            .WithTags("TierPrices")
            .WithDescription("Create a new tier price for a product");

        v1.MapPut("/products/tier-prices/{tierPriceId}", UpdateTierPrice)
            .WithName("UpdateTierPrice")
            .WithTags("TierPrices")
            .WithDescription("Update an existing tier price");

        v1.MapDelete("/products/tier-prices/{tierPriceId}", DeleteTierPrice)
            .WithName("DeleteTierPrice")
            .WithTags("TierPrices")
            .WithDescription("Delete a tier price");

        v1.MapPost("/products/specifications", GetProductSpecifications)
            .WithName("GetProductSpecifications")
            .WithTags("ProductSpecifications")
            .WithDescription("Get all product specifications");

        v1.MapPost("/products/specifications", CreateProductSpecification)
            .WithName("CreateProductSpecification")
            .WithTags("ProductSpecifications")
            .WithDescription("Create a new product specification");

        v1.MapPut("/products/specifications/{specificationId}", UpdateProductSpecification)
            .WithName("UpdateProductSpecification")
            .WithTags("ProductSpecifications")
            .WithDescription("Update an existing product specification");

        v1.MapDelete("/products/specifications/{specificationId}", DeleteProductSpecification)
            .WithName("DeleteProductSpecification")
            .WithTags("ProductSpecifications")
            .WithDescription("Delete a product specification");

        v1.MapPost("/products/videos", GetProductVideos)
            .WithName("GetProductVideos")
            .WithTags("ProductVideos")
            .WithDescription("Get all product videos");

        v1.MapPost("/products/videos", CreateProductVideo)
            .WithName("CreateProductVideo")
            .WithTags("ProductVideos")
            .WithDescription("Create a new product video");

        v1.MapPut("/products/videos/{videoId}", UpdateProductVideo)
            .WithName("UpdateProductVideo")
            .WithTags("ProductVideos")
            .WithDescription("Update an existing product video");

        v1.MapDelete("/products/videos/{videoId}", DeleteProductVideo)
            .WithName("DeleteProductVideo")
            .WithTags("ProductVideos")
            .WithDescription("Delete a product video");

        v1.MapPost("/products/warehouse-inventory", GetProductWarehouseInventory)
            .WithName("GetProductWarehouseInventory")
            .WithTags("ProductWarehouseInventory")
            .WithDescription("Get all warehouse inventory for a product");

        v1.MapPost("/products/warehouse-inventory", CreateProductWarehouseInventory)
            .WithName("CreateProductWarehouseInventory")
            .WithTags("ProductWarehouseInventory")
            .WithDescription("Create a new warehouse inventory for a product");

        v1.MapPut("/products/warehouse-inventory/{inventoryId}", UpdateProductWarehouseInventory)
            .WithName("UpdateProductWarehouseInventory")
            .WithTags("ProductWarehouseInventory")
            .WithDescription("Update an existing warehouse inventory for a product");

        v1.MapDelete("/products/warehouse-inventory/{inventoryId}", DeleteProductWarehouseInventory)
            .WithName("DeleteProductWarehouseInventory")
            .WithTags("ProductWarehouseInventory")
            .WithDescription("Delete a warehouse inventory for a product");

        v1.MapPost("/products/export-to-xml", ExportProductsToXml)
            .WithName("ExportProductsToXml")
            .WithTags("ProductExport")
            .WithDescription("Export products to XML");

        v1.MapPost("/products/export-to-excel", ExportProductsToExcel)
            .WithName("ExportProductsToExcel")
            .WithTags("ProductExport")
            .WithDescription("Export products to Excel");

        v1.MapPost("/products/import-from-excel", ImportProductsFromExcel)
            .WithName("ImportProductsFromExcel")
            .WithTags("ProductExport")
            .WithDescription("Import products from Excel");

        v1.MapPost("/products/export-to-pdf", ExportCatalogToPdf)
            .WithName("ExportCatalogToPdf")
            .WithTags("ProductExport")
            .WithDescription("Export catalog to PDF");

        return app;
    }

    // Product endpoints implementation
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Ok<PaginatedItems<Product>>> GetAllProductsV1(
        [AsParameters] PaginationRequest paginationRequest,
        [AsParameters] CatalogServices services)
    {
        var products = await services.ProductRepository.GetAllAsync();
        var paginatedProducts = new PaginatedItems<Product>(
            paginationRequest.PageIndex,
            paginationRequest.PageSize,
            products.Count(),
            products.Skip(paginationRequest.PageSize * paginationRequest.PageIndex)
                    .Take(paginationRequest.PageSize));

        return TypedResults.Ok(paginatedProducts);
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Ok<PaginatedItems<Product>>> GetAllProducts(
        [AsParameters] PaginationRequest paginationRequest,
        [AsParameters] CatalogServices services,
        [Description("The name of the product to return")] string name,
        [Description("The category of products to return")] int? categoryId,
        [Description("The tag of products to return")] int? tagId)
    {
        var products = await services.ProductRepository.GetAllAsync();
        
        if (!string.IsNullOrEmpty(name))
            products = products.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
        
        if (categoryId.HasValue)
            products = products.Where(p => p.Categories.Any(c => c.Id == categoryId.Value));
        
        if (tagId.HasValue)
            products = products.Where(p => p.ProductTags.Any(t => t.Id == tagId.Value));

        var paginatedProducts = new PaginatedItems<Product>(
            paginationRequest.PageIndex,
            paginationRequest.PageSize,
            products.Count(),
            products.Skip(paginationRequest.PageSize * paginationRequest.PageIndex)
                    .Take(paginationRequest.PageSize));

        return TypedResults.Ok(paginatedProducts);
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Results<Ok<Product>, NotFound>> GetProductById(
        [AsParameters] CatalogServices services,
        [Description("The product id")] int id)
    {
        var product = await services.ProductRepository.GetByIdAsync(id);
        if (product == null)
            return TypedResults.NotFound();

        return TypedResults.Ok(product);
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Created> CreateProduct(
        [AsParameters] CatalogServices services,
        Product product)
    {
        await services.ProductRepository.AddAsync(product);
        return TypedResults.Created($"/api/catalog/products/{product.Id}");
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Results<NoContent, NotFound>> UpdateProduct(
        [AsParameters] CatalogServices services,
        [Description("The product id")] int id,
        Product product)
    {
        var existingProduct = await services.ProductRepository.GetByIdAsync(id);
        if (existingProduct == null)
            return TypedResults.NotFound();

        product.Id = id;
        await services.ProductRepository.UpdateAsync(product);
        return TypedResults.NoContent();
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Results<NoContent, NotFound>> DeleteProduct(
        [AsParameters] CatalogServices services,
        [Description("The product id")] int id)
    {
        var product = await services.ProductRepository.GetByIdAsync(id);
        if (product == null)
            return TypedResults.NotFound();

        await services.ProductRepository.DeleteAsync(id);
        return TypedResults.NoContent();
    }

    // Category endpoints implementation
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Ok<IEnumerable<Category>>> GetAllCategories(
        [AsParameters] CatalogServices services)
    {
        var categories = await services.CategoryRepository.GetAllAsync();
        return TypedResults.Ok(categories);
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Results<Ok<Category>, NotFound>> GetCategoryById(
        [AsParameters] CatalogServices services,
        [Description("The category id")] int id)
    {
        var category = await services.CategoryRepository.GetByIdAsync(id);
        if (category == null)
            return TypedResults.NotFound();

        return TypedResults.Ok(category);
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Created> CreateCategory(
        [AsParameters] CatalogServices services,
        Category category)
    {
        await services.CategoryRepository.AddAsync(category);
        return TypedResults.Created($"/api/catalog/categories/{category.Id}");
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Results<NoContent, NotFound>> UpdateCategory(
        [AsParameters] CatalogServices services,
        [Description("The category id")] int id,
        Category category)
    {
        var existingCategory = await services.CategoryRepository.GetByIdAsync(id);
        if (existingCategory == null)
            return TypedResults.NotFound();

        category.Id = id;
        await services.CategoryRepository.UpdateAsync(category);
        return TypedResults.NoContent();
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Results<NoContent, NotFound>> DeleteCategory(
        [AsParameters] CatalogServices services,
        [Description("The category id")] int id)
    {
        var category = await services.CategoryRepository.GetByIdAsync(id);
        if (category == null)
            return TypedResults.NotFound();

        await services.CategoryRepository.DeleteAsync(id);
        return TypedResults.NoContent();
    }

    // Product Attribute endpoints implementation
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Ok<IEnumerable<ProductAttribute>>> GetAllProductAttributes(
        [AsParameters] CatalogServices services)
    {
        var attributes = await services.ProductAttributeRepository.GetAllAsync();
        return TypedResults.Ok(attributes);
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Results<Ok<ProductAttribute>, NotFound>> GetProductAttributeById(
        [AsParameters] CatalogServices services,
        [Description("The product attribute id")] int id)
    {
        var attribute = await services.ProductAttributeRepository.GetByIdAsync(id);
        if (attribute == null)
            return TypedResults.NotFound();

        return TypedResults.Ok(attribute);
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Created> CreateProductAttribute(
        [AsParameters] CatalogServices services,
        ProductAttribute attribute)
    {
        await services.ProductAttributeRepository.AddAsync(attribute);
        return TypedResults.Created($"/api/catalog/product-attributes/{attribute.Id}");
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Results<NoContent, NotFound>> UpdateProductAttribute(
        [AsParameters] CatalogServices services,
        [Description("The product attribute id")] int id,
        ProductAttribute attribute)
    {
        var existingAttribute = await services.ProductAttributeRepository.GetByIdAsync(id);
        if (existingAttribute == null)
            return TypedResults.NotFound();

        attribute.Id = id;
        await services.ProductAttributeRepository.UpdateAsync(attribute);
        return TypedResults.NoContent();
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Results<NoContent, NotFound>> DeleteProductAttribute(
        [AsParameters] CatalogServices services,
        [Description("The product attribute id")] int id)
    {
        var attribute = await services.ProductAttributeRepository.GetByIdAsync(id);
        if (attribute == null)
            return TypedResults.NotFound();

        await services.ProductAttributeRepository.DeleteAsync(id);
        return TypedResults.NoContent();
    }

    // Product Attribute Mapping endpoints implementation
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Ok<IEnumerable<ProductAttributeMapping>>> GetProductAttributeMappings(
        [AsParameters] CatalogServices services,
        [Description("The product id")] int productId)
    {
        var mappings = await services.ProductAttributeMappingRepository.GetByProductIdAsync(productId);
        return TypedResults.Ok(mappings);
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Created> CreateProductAttributeMapping(
        [AsParameters] CatalogServices services,
        [Description("The product id")] int productId,
        ProductAttributeMapping mapping)
    {
        mapping.ProductId = productId;
        await services.ProductAttributeMappingRepository.AddAsync(mapping);
        return TypedResults.Created($"/api/catalog/products/{productId}/attributes/{mapping.Id}");
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Results<NoContent, NotFound>> UpdateProductAttributeMapping(
        [AsParameters] CatalogServices services,
        [Description("The product id")] int productId,
        [Description("The mapping id")] int mappingId,
        ProductAttributeMapping mapping)
    {
        var existingMapping = await services.ProductAttributeMappingRepository.GetByIdAsync(mappingId);
        if (existingMapping == null || existingMapping.ProductId != productId)
            return TypedResults.NotFound();

        mapping.Id = mappingId;
        mapping.ProductId = productId;
        await services.ProductAttributeMappingRepository.UpdateAsync(mapping);
        return TypedResults.NoContent();
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Results<NoContent, NotFound>> DeleteProductAttributeMapping(
        [AsParameters] CatalogServices services,
        [Description("The product id")] int productId,
        [Description("The mapping id")] int mappingId)
    {
        var mapping = await services.ProductAttributeMappingRepository.GetByIdAsync(mappingId);
        if (mapping == null || mapping.ProductId != productId)
            return TypedResults.NotFound();

        await services.ProductAttributeMappingRepository.DeleteAsync(mappingId);
        return TypedResults.NoContent();
    }

    // Product Attribute Value endpoints implementation
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Ok<IEnumerable<ProductAttributeValue>>> GetProductAttributeValues(
        [AsParameters] CatalogServices services,
        [Description("The attribute id")] int attributeId)
    {
        var values = await services.ProductAttributeValueRepository.GetByProductAttributeMappingIdAsync(attributeId);
        return TypedResults.Ok(values);
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Created> CreateProductAttributeValue(
        [AsParameters] CatalogServices services,
        [Description("The attribute id")] int attributeId,
        ProductAttributeValue value)
    {
        value.ProductAttributeMappingId = attributeId;
        await services.ProductAttributeValueRepository.AddAsync(value);
        return TypedResults.Created($"/api/catalog/product-attributes/{attributeId}/values/{value.Id}");
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Results<NoContent, NotFound>> UpdateProductAttributeValue(
        [AsParameters] CatalogServices services,
        [Description("The attribute id")] int attributeId,
        [Description("The value id")] int valueId,
        ProductAttributeValue value)
    {
        var existingValue = await services.ProductAttributeValueRepository.GetByIdAsync(valueId);
        if (existingValue == null || existingValue.ProductAttributeMappingId != attributeId)
            return TypedResults.NotFound();

        value.Id = valueId;
        value.ProductAttributeMappingId = attributeId;
        await services.ProductAttributeValueRepository.UpdateAsync(value);
        return TypedResults.NoContent();
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Results<NoContent, NotFound>> DeleteProductAttributeValue(
        [AsParameters] CatalogServices services,
        [Description("The attribute id")] int attributeId,
        [Description("The value id")] int valueId)
    {
        var value = await services.ProductAttributeValueRepository.GetByIdAsync(valueId);
        if (value == null || value.ProductAttributeMappingId != attributeId)
            return TypedResults.NotFound();

        await services.ProductAttributeValueRepository.DeleteAsync(valueId);
        return TypedResults.NoContent();
    }

    // Product Picture endpoints implementation
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Ok<IEnumerable<ProductPicture>>> GetProductPictures(
        [AsParameters] CatalogServices services,
        [Description("The product id")] int productId)
    {
        var pictures = await services.ProductPictureRepository.GetByProductIdAsync(productId);
        return TypedResults.Ok(pictures);
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Created> CreateProductPicture(
        [AsParameters] CatalogServices services,
        [Description("The product id")] int productId,
        ProductPicture picture)
    {
        picture.ProductId = productId;
        await services.ProductPictureRepository.AddAsync(picture);
        return TypedResults.Created($"/api/catalog/products/{productId}/pictures/{picture.Id}");
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Results<NoContent, NotFound>> DeleteProductPicture(
        [AsParameters] CatalogServices services,
        [Description("The product id")] int productId,
        [Description("The picture id")] int pictureId)
    {
        var picture = await services.ProductPictureRepository.GetByIdAsync(pictureId);
        if (picture == null || picture.ProductId != productId)
            return TypedResults.NotFound();

        await services.ProductPictureRepository.DeleteAsync(pictureId);
        return TypedResults.NoContent();
    }

    // Product Review endpoints implementation
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Ok<IEnumerable<ProductReview>>> GetProductReviews(
        [AsParameters] CatalogServices services,
        [Description("The product id")] int productId)
    {
        var reviews = await services.ProductReviewRepository.GetByProductIdAsync(productId);
        return TypedResults.Ok(reviews);
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Created> CreateProductReview(
        [AsParameters] CatalogServices services,
        [Description("The product id")] int productId,
        ProductReview review)
    {
        review.ProductId = productId;
        await services.ProductReviewRepository.AddAsync(review);
        return TypedResults.Created($"/api/catalog/products/{productId}/reviews/{review.Id}");
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Results<NoContent, NotFound>> UpdateProductReview(
        [AsParameters] CatalogServices services,
        [Description("The product id")] int productId,
        [Description("The review id")] int reviewId,
        ProductReview review)
    {
        var existingReview = await services.ProductReviewRepository.GetByIdAsync(reviewId);
        if (existingReview == null || existingReview.ProductId != productId)
            return TypedResults.NotFound();

        review.Id = reviewId;
        review.ProductId = productId;
        await services.ProductReviewRepository.UpdateAsync(review);
        return TypedResults.NoContent();
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Results<NoContent, NotFound>> DeleteProductReview(
        [AsParameters] CatalogServices services,
        [Description("The product id")] int productId,
        [Description("The review id")] int reviewId)
    {
        var review = await services.ProductReviewRepository.GetByIdAsync(reviewId);
        if (review == null || review.ProductId != productId)
            return TypedResults.NotFound();

        await services.ProductReviewRepository.DeleteAsync(reviewId);
        return TypedResults.NoContent();
    }

    // Product Tag endpoints implementation
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Ok<IEnumerable<ProductTag>>> GetAllProductTags(
        [AsParameters] CatalogServices services)
    {
        var tags = await services.ProductTagRepository.GetAllAsync();
        return TypedResults.Ok(tags);
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Ok<IEnumerable<ProductTag>>> GetProductTags(
        [AsParameters] CatalogServices services,
        [Description("The product id")] int productId)
    {
        var tags = await services.ProductTagRepository.GetByProductIdAsync(productId);
        return TypedResults.Ok(tags);
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Created> AddProductTag(
        [AsParameters] CatalogServices services,
        [Description("The product id")] int productId,
        ProductTag tag)
    {
        tag.ProductId = productId;
        await services.ProductTagRepository.AddAsync(tag);
        return TypedResults.Created($"/api/catalog/products/{productId}/tags/{tag.Id}");
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Results<NoContent, NotFound>> RemoveProductTag(
        [AsParameters] CatalogServices services,
        [Description("The product id")] int productId,
        [Description("The tag id")] int tagId)
    {
        var tag = await services.ProductTagRepository.GetByIdAsync(tagId);
        if (tag == null || tag.ProductId != productId)
            return TypedResults.NotFound();

        await services.ProductTagRepository.DeleteAsync(tagId);
        return TypedResults.NoContent();
    }

    // Related Product endpoints implementation
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Ok<IEnumerable<RelatedProduct>>> GetRelatedProducts(
        [AsParameters] CatalogServices services,
        [Description("The product id")] int productId)
    {
        var relatedProducts = await services.RelatedProductRepository.GetByProductIdAsync(productId);
        return TypedResults.Ok(relatedProducts);
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Created> AddRelatedProduct(
        [AsParameters] CatalogServices services,
        [Description("The product id")] int productId,
        RelatedProduct relatedProduct)
    {
        relatedProduct.ProductId1 = productId;
        await services.RelatedProductRepository.AddAsync(relatedProduct);
        return TypedResults.Created($"/api/catalog/products/{productId}/related/{relatedProduct.Id}");
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Results<NoContent, NotFound>> RemoveRelatedProduct(
        [AsParameters] CatalogServices services,
        [Description("The product id")] int productId,
        [Description("The related product id")] int relatedProductId)
    {
        var relatedProduct = await services.RelatedProductRepository.GetByIdAsync(relatedProductId);
        if (relatedProduct == null || (relatedProduct.ProductId1 != productId && relatedProduct.ProductId2 != productId))
            return TypedResults.NotFound();

        await services.RelatedProductRepository.DeleteAsync(relatedProductId);
        return TypedResults.NoContent();
    }

    // Cross-sell Product endpoints implementation
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Ok<IEnumerable<CrossSellProduct>>> GetCrossSellProducts(
        [AsParameters] CatalogServices services,
        [Description("The product id")] int productId)
    {
        var crossSellProducts = await services.CrossSellProductRepository.GetByProductIdAsync(productId);
        return TypedResults.Ok(crossSellProducts);
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Created> AddCrossSellProduct(
        [AsParameters] CatalogServices services,
        [Description("The product id")] int productId,
        CrossSellProduct crossSellProduct)
    {
        crossSellProduct.ProductId1 = productId;
        await services.CrossSellProductRepository.AddAsync(crossSellProduct);
        return TypedResults.Created($"/api/catalog/products/{productId}/cross-sells/{crossSellProduct.Id}");
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Results<NoContent, NotFound>> RemoveCrossSellProduct(
        [AsParameters] CatalogServices services,
        [Description("The product id")] int productId,
        [Description("The cross-sell product id")] int crossSellProductId)
    {
        var crossSellProduct = await services.CrossSellProductRepository.GetByIdAsync(crossSellProductId);
        if (crossSellProduct == null || (crossSellProduct.ProductId1 != productId && crossSellProduct.ProductId2 != productId))
            return TypedResults.NotFound();

        await services.CrossSellProductRepository.DeleteAsync(crossSellProductId);
        return TypedResults.NoContent();
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Results<Ok<PaginatedItems<Product>>, BadRequest>> BulkEditProducts(
        [AsParameters] CatalogServices services,
        [AsParameters] PaginationRequest paginationRequest,
        [FromBody] List<BulkEditProductModel> products)
    {
        if (products == null || !products.Any())
            return TypedResults.BadRequest();

        var updatedProducts = new List<Product>();
        foreach (var model in products)
        {
            var product = await services.ProductRepository.GetByIdAsync(model.Id);
            if (product == null) continue;

            // Update product properties
            product.UpdateDetails(
                model.Name ?? product.Name,
                model.Description ?? product.Description,
                model.Price ?? product.Price,
                model.PictureUri ?? product.PictureUri,
                model.CatalogTypeId ?? product.CatalogTypeId,
                model.CatalogBrandId ?? product.CatalogBrandId);

            if (model.AvailableStock.HasValue)
                product.UpdateStock(model.AvailableStock.Value);

            updatedProducts.Add(product);
        }

        await services.ProductRepository.UpdateRangeAsync(updatedProducts);

        var paginatedProducts = new PaginatedItems<Product>(
            paginationRequest.PageIndex,
            paginationRequest.PageSize,
            updatedProducts.Count,
            updatedProducts.Skip(paginationRequest.PageSize * paginationRequest.PageIndex)
                    .Take(paginationRequest.PageSize));

        return TypedResults.Ok(paginatedProducts);
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Results<Ok<PaginatedItems<Product>>, BadRequest>> BulkDeleteProducts(
        [AsParameters] CatalogServices services,
        [AsParameters] PaginationRequest paginationRequest,
        [FromBody] List<int> productIds)
    {
        if (productIds == null || !productIds.Any())
            return TypedResults.BadRequest();

        var products = new List<Product>();
        foreach (var id in productIds)
        {
            var product = await services.ProductRepository.GetByIdAsync(id);
            if (product != null)
                products.Add(product);
        }

        await services.ProductRepository.DeleteRangeAsync(products);

        var paginatedProducts = new PaginatedItems<Product>(
            paginationRequest.PageIndex,
            paginationRequest.PageSize,
            products.Count,
            products.Skip(paginationRequest.PageSize * paginationRequest.PageIndex)
                    .Take(paginationRequest.PageSize));

        return TypedResults.Ok(paginatedProducts);
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Results<Ok<PaginatedItems<Product>>, BadRequest>> BulkPublishProducts(
        [AsParameters] CatalogServices services,
        [AsParameters] PaginationRequest paginationRequest,
        [FromBody] List<int> productIds)
    {
        if (productIds == null || !productIds.Any())
            return TypedResults.BadRequest();

        var products = new List<Product>();
        foreach (var id in productIds)
        {
            var product = await services.ProductRepository.GetByIdAsync(id);
            if (product != null)
            {
                product.Publish();
                products.Add(product);
            }
        }

        await services.ProductRepository.UpdateRangeAsync(products);

        var paginatedProducts = new PaginatedItems<Product>(
            paginationRequest.PageIndex,
            paginationRequest.PageSize,
            products.Count,
            products.Skip(paginationRequest.PageSize * paginationRequest.PageIndex)
                    .Take(paginationRequest.PageSize));

        return TypedResults.Ok(paginatedProducts);
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Results<Ok<PaginatedItems<Product>>, BadRequest>> BulkUnpublishProducts(
        [AsParameters] CatalogServices services,
        [AsParameters] PaginationRequest paginationRequest,
        [FromBody] List<int> productIds)
    {
        if (productIds == null || !productIds.Any())
            return TypedResults.BadRequest();

        var products = new List<Product>();
        foreach (var id in productIds)
        {
            var product = await services.ProductRepository.GetByIdAsync(id);
            if (product != null)
            {
                product.Unpublish();
                products.Add(product);
            }
        }

        await services.ProductRepository.UpdateRangeAsync(products);

        var paginatedProducts = new PaginatedItems<Product>(
            paginationRequest.PageIndex,
            paginationRequest.PageSize,
            products.Count,
            products.Skip(paginationRequest.PageSize * paginationRequest.PageIndex)
                    .Take(paginationRequest.PageSize));

        return TypedResults.Ok(paginatedProducts);
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Results<Ok<PaginatedItems<ProductAttributeCombination>>, BadRequest>> GetProductAttributeCombinations(
        [AsParameters] CatalogServices services,
        [AsParameters] PaginationRequest paginationRequest,
        [Description("The product id")] int productId)
    {
        var combinations = await services.ProductAttributeCombinationRepository.GetByProductAsync(productId);
        
        var paginatedCombinations = new PaginatedItems<ProductAttributeCombination>(
            paginationRequest.PageIndex,
            paginationRequest.PageSize,
            combinations.Count(),
            combinations.Skip(paginationRequest.PageSize * paginationRequest.PageIndex)
                    .Take(paginationRequest.PageSize));

        return TypedResults.Ok(paginatedCombinations);
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Results<Created, BadRequest>> CreateProductAttributeCombination(
        [AsParameters] CatalogServices services,
        [Description("The product id")] int productId,
        ProductAttributeCombination combination)
    {
        if (combination == null)
            return TypedResults.BadRequest();

        combination.ProductId = productId;
        await services.ProductAttributeCombinationRepository.AddAsync(combination);
        return TypedResults.Created($"/api/catalog/products/{productId}/attribute-combinations/{combination.Id}");
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Results<NoContent, NotFound, BadRequest>> UpdateProductAttributeCombination(
        [AsParameters] CatalogServices services,
        [Description("The product id")] int productId,
        [Description("The combination id")] int combinationId,
        ProductAttributeCombination combination)
    {
        if (combination == null)
            return TypedResults.BadRequest();

        var existingCombination = await services.ProductAttributeCombinationRepository.GetAsync(combinationId);
        if (existingCombination == null)
            return TypedResults.NotFound();

        combination.ProductId = productId;
        await services.ProductAttributeCombinationRepository.UpdateAsync(combination);
        return TypedResults.NoContent();
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Results<NoContent, NotFound>> DeleteProductAttributeCombination(
        [AsParameters] CatalogServices services,
        [Description("The product id")] int productId,
        [Description("The combination id")] int combinationId)
    {
        var combination = await services.ProductAttributeCombinationRepository.GetAsync(combinationId);
        if (combination == null)
            return TypedResults.NotFound();

        await services.ProductAttributeCombinationRepository.DeleteAsync(combinationId);
        return TypedResults.NoContent();
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Results<Ok<PaginatedItems<ProductAttributeCombination>>, BadRequest>> GenerateAllAttributeCombinations(
        [AsParameters] CatalogServices services,
        [AsParameters] PaginationRequest paginationRequest,
        [Description("The product id")] int productId)
    {
        var product = await services.ProductRepository.GetByIdAsync(productId);
        if (product == null)
            return TypedResults.BadRequest();

        var attributeMappings = await services.ProductAttributeMappingRepository.GetByProductAsync(productId);
        var combinations = new List<ProductAttributeCombination>();

        // Generate all possible combinations
        foreach (var mapping in attributeMappings)
        {
            var values = await services.ProductAttributeValueRepository.GetByAttributeAsync(mapping.Id);
            foreach (var value in values)
            {
                var combination = new ProductAttributeCombination
                {
                    ProductId = productId,
                    AttributesXml = $"{mapping.Id}:{value.Id}",
                    StockQuantity = 0,
                    AllowOutOfStockOrders = true
                };
                combinations.Add(combination);
            }
        }

        await services.ProductAttributeCombinationRepository.AddRangeAsync(combinations);

        var paginatedCombinations = new PaginatedItems<ProductAttributeCombination>(
            paginationRequest.PageIndex,
            paginationRequest.PageSize,
            combinations.Count,
            combinations.Skip(paginationRequest.PageSize * paginationRequest.PageIndex)
                    .Take(paginationRequest.PageSize));

        return TypedResults.Ok(paginatedCombinations);
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Results<Ok<PaginatedItems<TierPrice>>, BadRequest>> GetTierPrices(
        [AsParameters] CatalogServices services,
        [AsParameters] PaginationRequest paginationRequest,
        [Description("The product id")] int productId)
    {
        var tierPrices = await services.TierPriceRepository.GetByProductAsync(productId);
        
        var paginatedTierPrices = new PaginatedItems<TierPrice>(
            paginationRequest.PageIndex,
            paginationRequest.PageSize,
            tierPrices.Count(),
            tierPrices.Skip(paginationRequest.PageSize * paginationRequest.PageIndex)
                    .Take(paginationRequest.PageSize));

        return TypedResults.Ok(paginatedTierPrices);
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Results<Created, BadRequest>> CreateTierPrice(
        [AsParameters] CatalogServices services,
        [Description("The product id")] int productId,
        TierPrice tierPrice)
    {
        if (tierPrice == null)
            return TypedResults.BadRequest();

        tierPrice.ProductId = productId;
        await services.TierPriceRepository.AddAsync(tierPrice);
        return TypedResults.Created($"/api/catalog/products/{productId}/tier-prices/{tierPrice.Id}");
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Results<NoContent, NotFound, BadRequest>> UpdateTierPrice(
        [AsParameters] CatalogServices services,
        [Description("The product id")] int productId,
        [Description("The tier price id")] int tierPriceId,
        TierPrice tierPrice)
    {
        if (tierPrice == null)
            return TypedResults.BadRequest();

        var existingTierPrice = await services.TierPriceRepository.GetAsync(tierPriceId);
        if (existingTierPrice == null)
            return TypedResults.NotFound();

        tierPrice.ProductId = productId;
        await services.TierPriceRepository.UpdateAsync(tierPrice);
        return TypedResults.NoContent();
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Results<NoContent, NotFound>> DeleteTierPrice(
        [AsParameters] CatalogServices services,
        [Description("The product id")] int productId,
        [Description("The tier price id")] int tierPriceId)
    {
        var tierPrice = await services.TierPriceRepository.GetAsync(tierPriceId);
        if (tierPrice == null)
            return TypedResults.NotFound();

        await services.TierPriceRepository.DeleteAsync(tierPriceId);
        return TypedResults.NoContent();
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Results<Ok<PaginatedItems<ProductSpecificationAttribute>>, BadRequest>> GetProductSpecifications(
        [AsParameters] CatalogServices services,
        [AsParameters] PaginationRequest paginationRequest,
        [Description("The product id")] int productId)
    {
        var specifications = await services.ProductSpecificationAttributeRepository.GetByProductAsync(productId);
        
        var paginatedSpecifications = new PaginatedItems<ProductSpecificationAttribute>(
            paginationRequest.PageIndex,
            paginationRequest.PageSize,
            specifications.Count(),
            specifications.Skip(paginationRequest.PageSize * paginationRequest.PageIndex)
                    .Take(paginationRequest.PageSize));

        return TypedResults.Ok(paginatedSpecifications);
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Results<Created, BadRequest>> CreateProductSpecification(
        [AsParameters] CatalogServices services,
        [Description("The product id")] int productId,
        ProductSpecificationAttribute specification)
    {
        if (specification == null)
            return TypedResults.BadRequest();

        specification.ProductId = productId;
        await services.ProductSpecificationAttributeRepository.AddAsync(specification);
        return TypedResults.Created($"/api/catalog/products/{productId}/specifications/{specification.Id}");
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Results<NoContent, NotFound, BadRequest>> UpdateProductSpecification(
        [AsParameters] CatalogServices services,
        [Description("The product id")] int productId,
        [Description("The specification id")] int specificationId,
        ProductSpecificationAttribute specification)
    {
        if (specification == null)
            return TypedResults.BadRequest();

        var existingSpecification = await services.ProductSpecificationAttributeRepository.GetAsync(specificationId);
        if (existingSpecification == null)
            return TypedResults.NotFound();

        specification.ProductId = productId;
        await services.ProductSpecificationAttributeRepository.UpdateAsync(specification);
        return TypedResults.NoContent();
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Results<NoContent, NotFound>> DeleteProductSpecification(
        [AsParameters] CatalogServices services,
        [Description("The product id")] int productId,
        [Description("The specification id")] int specificationId)
    {
        var specification = await services.ProductSpecificationAttributeRepository.GetAsync(specificationId);
        if (specification == null)
            return TypedResults.NotFound();

        await services.ProductSpecificationAttributeRepository.DeleteAsync(specificationId);
        return TypedResults.NoContent();
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Results<Ok<PaginatedItems<ProductVideo>>, BadRequest>> GetProductVideos(
        [AsParameters] CatalogServices services,
        [AsParameters] PaginationRequest paginationRequest,
        [Description("The product id")] int productId)
    {
        var videos = await services.ProductVideoRepository.GetByProductAsync(productId);
        
        var paginatedVideos = new PaginatedItems<ProductVideo>(
            paginationRequest.PageIndex,
            paginationRequest.PageSize,
            videos.Count(),
            videos.Skip(paginationRequest.PageSize * paginationRequest.PageIndex)
                    .Take(paginationRequest.PageSize));

        return TypedResults.Ok(paginatedVideos);
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Results<Created, BadRequest>> CreateProductVideo(
        [AsParameters] CatalogServices services,
        [Description("The product id")] int productId,
        ProductVideo video)
    {
        if (video == null)
            return TypedResults.BadRequest();

        video.ProductId = productId;
        await services.ProductVideoRepository.AddAsync(video);
        return TypedResults.Created($"/api/catalog/products/{productId}/videos/{video.Id}");
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Results<NoContent, NotFound, BadRequest>> UpdateProductVideo(
        [AsParameters] CatalogServices services,
        [Description("The product id")] int productId,
        [Description("The video id")] int videoId,
        ProductVideo video)
    {
        if (video == null)
            return TypedResults.BadRequest();

        var existingVideo = await services.ProductVideoRepository.GetAsync(videoId);
        if (existingVideo == null)
            return TypedResults.NotFound();

        video.ProductId = productId;
        await services.ProductVideoRepository.UpdateAsync(video);
        return TypedResults.NoContent();
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Results<NoContent, NotFound>> DeleteProductVideo(
        [AsParameters] CatalogServices services,
        [Description("The product id")] int productId,
        [Description("The video id")] int videoId)
    {
        var video = await services.ProductVideoRepository.GetAsync(videoId);
        if (video == null)
            return TypedResults.NotFound();

        await services.ProductVideoRepository.DeleteAsync(videoId);
        return TypedResults.NoContent();
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Results<Ok<PaginatedItems<ProductWarehouseInventory>>, BadRequest>> GetProductWarehouseInventory(
        [AsParameters] CatalogServices services,
        [AsParameters] PaginationRequest paginationRequest,
        [Description("The product id")] int productId)
    {
        var inventory = await services.ProductWarehouseInventoryRepository.GetByProductAsync(productId);
        
        var paginatedInventory = new PaginatedItems<ProductWarehouseInventory>(
            paginationRequest.PageIndex,
            paginationRequest.PageSize,
            inventory.Count(),
            inventory.Skip(paginationRequest.PageSize * paginationRequest.PageIndex)
                    .Take(paginationRequest.PageSize));

        return TypedResults.Ok(paginatedInventory);
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Results<Created, BadRequest>> CreateProductWarehouseInventory(
        [AsParameters] CatalogServices services,
        [Description("The product id")] int productId,
        ProductWarehouseInventory inventory)
    {
        if (inventory == null)
            return TypedResults.BadRequest();

        inventory.ProductId = productId;
        await services.ProductWarehouseInventoryRepository.AddAsync(inventory);
        return TypedResults.Created($"/api/catalog/products/{productId}/warehouse-inventory/{inventory.Id}");
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Results<NoContent, NotFound, BadRequest>> UpdateProductWarehouseInventory(
        [AsParameters] CatalogServices services,
        [Description("The product id")] int productId,
        [Description("The inventory id")] int inventoryId,
        ProductWarehouseInventory inventory)
    {
        if (inventory == null)
            return TypedResults.BadRequest();

        var existingInventory = await services.ProductWarehouseInventoryRepository.GetAsync(inventoryId);
        if (existingInventory == null)
            return TypedResults.NotFound();

        inventory.ProductId = productId;
        await services.ProductWarehouseInventoryRepository.UpdateAsync(inventory);
        return TypedResults.NoContent();
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Results<NoContent, NotFound>> DeleteProductWarehouseInventory(
        [AsParameters] CatalogServices services,
        [Description("The product id")] int productId,
        [Description("The inventory id")] int inventoryId)
    {
        var inventory = await services.ProductWarehouseInventoryRepository.GetAsync(inventoryId);
        if (inventory == null)
            return TypedResults.NotFound();

        await services.ProductWarehouseInventoryRepository.DeleteAsync(inventoryId);
        return TypedResults.NoContent();
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Results<FileContentResult, BadRequest>> ExportProductsToXml(
        [AsParameters] CatalogServices services,
        [Description("The product ids to export")] string productIds)
    {
        if (string.IsNullOrEmpty(productIds))
            return TypedResults.BadRequest();

        var ids = productIds.Split(',').Select(int.Parse).ToList();
        var products = new List<Product>();
        
        foreach (var id in ids)
        {
            var product = await services.ProductRepository.GetByIdAsync(id);
            if (product != null)
                products.Add(product);
        }

        var xml = await services.ExportManager.ExportProductsToXmlAsync(products);
        var fileName = $"products_{DateTime.UtcNow:yyyy-MM-dd-HH-mm-ss}.xml";
        
        return TypedResults.File(
            System.Text.Encoding.UTF8.GetBytes(xml),
            "application/xml",
            fileName);
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Results<FileContentResult, BadRequest>> ExportProductsToExcel(
        [AsParameters] CatalogServices services,
        [Description("The product ids to export")] string productIds)
    {
        if (string.IsNullOrEmpty(productIds))
            return TypedResults.BadRequest();

        var ids = productIds.Split(',').Select(int.Parse).ToList();
        var products = new List<Product>();
        
        foreach (var id in ids)
        {
            var product = await services.ProductRepository.GetByIdAsync(id);
            if (product != null)
                products.Add(product);
        }

        var excel = await services.ExportManager.ExportProductsToExcelAsync(products);
        var fileName = $"products_{DateTime.UtcNow:yyyy-MM-dd-HH-mm-ss}.xlsx";
        
        return TypedResults.File(
            excel,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Results<Ok<int>, BadRequest>> ImportProductsFromExcel(
        [AsParameters] CatalogServices services,
        IFormFile file)
    {
        if (file == null || file.Length == 0)
            return TypedResults.BadRequest();

        using var stream = file.OpenReadStream();
        var importedCount = await services.ImportManager.ImportProductsFromExcelAsync(stream);
        
        return TypedResults.Ok(importedCount);
    }

    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public static async Task<Results<FileContentResult, BadRequest>> ExportCatalogToPdf(
        [AsParameters] CatalogServices services,
        [Description("The product ids to export")] string productIds)
    {
        if (string.IsNullOrEmpty(productIds))
            return TypedResults.BadRequest();

        var ids = productIds.Split(',').Select(int.Parse).ToList();
        var products = new List<Product>();
        
        foreach (var id in ids)
        {
            var product = await services.ProductRepository.GetByIdAsync(id);
            if (product != null)
                products.Add(product);
        }

        var pdf = await services.PdfService.ExportProductsToPdfAsync(products);
        var fileName = $"catalog_{DateTime.UtcNow:yyyy-MM-dd-HH-mm-ss}.pdf";
        
        return TypedResults.File(
            pdf,
            "application/pdf",
            fileName);
    }

    private static string GetImageMimeTypeFromImageFileExtension(string extension) => extension switch
    {
        ".png" => "image/png",
        ".gif" => "image/gif",
        ".jpg" or ".jpeg" => "image/jpeg",
        ".bmp" => "image/bmp",
        ".tiff" => "image/tiff",
        ".wmf" => "image/wmf",
        ".jp2" => "image/jp2",
        ".svg" => "image/svg+xml",
        ".webp" => "image/webp",
        _ => "application/octet-stream",
    };

    public static string GetFullPath(string contentRootPath, string pictureFileName) =>
        Path.Combine(contentRootPath, "Pics", pictureFileName);
}
