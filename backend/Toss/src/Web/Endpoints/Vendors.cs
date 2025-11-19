using Toss.Application.Vendors.Commands.CreateVendor;
using Toss.Application.Vendors.Commands.LinkVendorProduct;
using Toss.Application.Vendors.Commands.UpdateVendorPricing;
using Toss.Application.Vendors.Queries.GetVendorById;
using Toss.Application.Vendors.Queries.GetVendorProducts;
using Toss.Application.Vendors.Queries.GetVendors;

namespace Toss.Web.Endpoints;

public class Vendors : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.MapPost(string.Empty, CreateVendor);
        group.MapGet(string.Empty, GetVendors);
        group.MapGet("{id}", GetVendorById);
        group.MapPost("{id}/products", LinkVendorProduct);
        group.MapGet("{id}/products", GetVendorProducts);
        group.MapPut("products/{productId}/pricing", UpdateVendorPricing);
    }

    public async Task<IResult> CreateVendor(ISender sender, CreateVendorCommand command)
    {
        var id = await sender.Send(command);
        return Results.Created($"/api/vendors/{id}", new { id });
    }

    public async Task<IResult> GetVendors(
        ISender sender,
        [AsParameters] GetVendorsQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> GetVendorById(ISender sender, int id)
    {
        var result = await sender.Send(new GetVendorByIdQuery { Id = id });
        return Results.Ok(result);
    }

    public async Task<IResult> LinkVendorProduct(ISender sender, int id, LinkVendorProductCommand command)
    {
        var linkId = await sender.Send(command with { VendorId = id });
        return Results.Created($"/api/vendors/{id}/products/{linkId}", new { id = linkId });
    }

    public async Task<IResult> GetVendorProducts(ISender sender, int id, [AsParameters] GetVendorProductsQuery query)
    {
        var result = await sender.Send(query with { VendorId = id });
        return Results.Ok(result);
    }

    public async Task<IResult> UpdateVendorPricing(ISender sender, int productId, UpdateVendorPricingCommand command)
    {
        var result = await sender.Send(command with { VendorProductId = productId });
        return Results.Ok(new { id = result });
    }
}

