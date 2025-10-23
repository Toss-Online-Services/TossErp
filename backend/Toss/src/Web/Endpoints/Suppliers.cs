using Toss.Application.Suppliers.Commands.CreateSupplier;
using Toss.Application.Suppliers.Commands.LinkSupplierProduct;
using Toss.Application.Suppliers.Queries.GetSupplierById;
using Toss.Application.Suppliers.Queries.GetSuppliers;

namespace Toss.Web.Endpoints;

public class Suppliers : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.MapPost(string.Empty, CreateSupplier);
        group.MapGet(string.Empty, GetSuppliers);
        group.MapGet("{id}", GetSupplierById);
        group.MapPost("{id}/products", LinkSupplierProduct);
    }

    public async Task<IResult> CreateSupplier(ISender sender, CreateSupplierCommand command)
    {
        var id = await sender.Send(command);
        return Results.Created($"/api/suppliers/{id}", new { id });
    }

    public async Task<IResult> GetSuppliers(
        ISender sender,
        [AsParameters] GetSuppliersQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> GetSupplierById(ISender sender, int id)
    {
        var result = await sender.Send(new GetSupplierByIdQuery { Id = id });
        return Results.Ok(result);
    }

    public async Task<IResult> LinkSupplierProduct(ISender sender, int id, LinkSupplierProductCommand command)
    {
        var linkId = await sender.Send(command with { SupplierId = id });
        return Results.Created($"/api/suppliers/{id}/products/{linkId}", new { id = linkId });
    }
}

