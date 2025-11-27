using Toss.Application.Vendors.Commands.CreateVendor;
using Toss.Application.Vendors.Commands.LinkVendorProduct;
using Toss.Application.Vendors.Commands.UpdateVendorPricing;
using Toss.Application.Vendors.Queries.GetVendorById;
using Toss.Application.Vendors.Queries.GetVendorProducts;
using Toss.Application.Vendors.Queries.GetVendors;
using Toss.Domain.Constants;

namespace Toss.Web.Endpoints;

/// <summary>
/// Suppliers endpoint - Alias for Vendors endpoints for backward compatibility
/// Frontend uses "/api/suppliers" but backend uses Vendors entity
/// </summary>
public class Suppliers : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.RequireAuthorization(Policies.RequireOwnerOrManager);
        // GET /api/suppliers - List suppliers (alias for vendors)
        group.MapGet(string.Empty, GetSuppliers)
            .WithName("GetSuppliers")
            .WithDescription("Get all suppliers for a shop (alias for vendors)");

        // GET /api/suppliers/{id} - Get supplier by ID (alias for vendor)
        group.MapGet("{id}", GetSupplierById)
            .WithName("GetSupplierById")
            .WithDescription("Get a specific supplier by ID (alias for vendor)");

        // POST /api/suppliers - Create new supplier (alias for vendor)
        group.MapPost(string.Empty, CreateSupplier)
            .WithName("CreateSupplier")
            .WithDescription("Create a new supplier (alias for vendor)");

        // GET /api/suppliers/{id}/products - Get supplier products (alias for vendor products)
        group.MapGet("{id}/products", GetSupplierProducts)
            .WithName("GetSupplierProducts")
            .WithDescription("Get products linked to a supplier (alias for vendor products)");

        // POST /api/suppliers/{id}/products - Link product to supplier (alias for vendor)
        group.MapPost("{id}/products", LinkSupplierProduct)
            .WithName("LinkSupplierProduct")
            .WithDescription("Link a product to a supplier (alias for vendor)");

        // PUT /api/suppliers/products/{productId}/pricing - Update supplier product pricing (alias for vendor)
        group.MapPut("products/{productId}/pricing", UpdateSupplierPricing)
            .WithName("UpdateSupplierPricing")
            .WithDescription("Update supplier product pricing (alias for vendor pricing)");
    }

    // GET /api/suppliers - Alias for GetVendors
    private static async Task<IResult> GetSuppliers(ISender sender, string? searchTerm, bool? activeOnly, int? pageNumber, int? pageSize)
    {
        var query = new GetVendorsQuery
        {
            SearchTerm = searchTerm,
            ActiveOnly = activeOnly,
            PageNumber = pageNumber ?? 1,
            PageSize = pageSize ?? 10
        };

        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    // GET /api/suppliers/{id} - Alias for GetVendorById
    private static async Task<IResult> GetSupplierById(ISender sender, int id)
    {
        var query = new GetVendorByIdQuery { Id = id };
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    // POST /api/suppliers - Alias for CreateVendor
    private static async Task<IResult> CreateSupplier(ISender sender, CreateVendorCommand command)
    {
        var vendorId = await sender.Send(command);
        return Results.Created($"/api/suppliers/{vendorId}", new { id = vendorId });
    }

    // GET /api/suppliers/{id}/products - Alias for GetVendorProducts
    private static async Task<IResult> GetSupplierProducts(ISender sender, int id)
    {
        var query = new GetVendorProductsQuery { VendorId = id };
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    // POST /api/suppliers/{id}/products - Alias for LinkVendorProduct
    private static async Task<IResult> LinkSupplierProduct(ISender sender, int id, LinkVendorProductCommand command)
    {
        // Ensure VendorId from route matches command
        var updatedCommand = command with { VendorId = id };
        var productId = await sender.Send(updatedCommand);
        return Results.Created($"/api/suppliers/{id}/products/{productId}", new { id = productId });
    }

    // PUT /api/suppliers/products/{productId}/pricing - Alias for UpdateVendorPricing
    private static async Task<IResult> UpdateSupplierPricing(ISender sender, int productId, UpdateVendorPricingCommand command)
    {
        // Ensure ProductId from route matches command
        var updatedCommand = command with { VendorProductId = productId };
        await sender.Send(updatedCommand);
        return Results.NoContent();
    }
}

