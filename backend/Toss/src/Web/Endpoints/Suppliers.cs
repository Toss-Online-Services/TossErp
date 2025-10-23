using Toss.Application.Suppliers.Commands.CreateSupplier;
using Toss.Application.Suppliers.Queries.GetSuppliers;

namespace Toss.Web.Endpoints;

public class Suppliers : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.MapPost(string.Empty, CreateSupplier);
        group.MapGet(string.Empty, GetSuppliers);
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
}

