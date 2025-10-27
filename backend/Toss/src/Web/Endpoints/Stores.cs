using Toss.Application.Stores.Commands.CreateStore;
using Toss.Application.Stores.Commands.UpdateStore;
using Toss.Application.Stores.Commands.DeleteStore;
using Toss.Application.Stores.Queries.GetStores;
using Toss.Application.Stores.Queries.GetStoreById;

namespace Toss.Web.Endpoints;

public class Stores : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.MapGet(string.Empty, GetStores)
            .WithName("GetStores");

        group.MapGet("{id}", GetStoreById)
            .WithName("GetStoreById");

        group.MapPost(string.Empty, CreateStore)
            .WithName("CreateStore");

        group.MapPut("{id}", UpdateStore)
            .WithName("UpdateStore");

        group.MapDelete("{id}", DeleteStore)
            .WithName("DeleteStore");
    }

    public async Task<IResult> GetStores(ISender sender, string? searchTerm, bool? activeOnly, int? pageNumber, int? pageSize)
    {
        var query = new GetStoresQuery
        {
            SearchTerm = searchTerm,
            ActiveOnly = activeOnly,
            PageNumber = pageNumber ?? 1,
            PageSize = pageSize ?? 50
        };

        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> GetStoreById(ISender sender, int id)
    {
        var result = await sender.Send(new GetStoreByIdQuery { Id = id });
        return Results.Ok(result);
    }

    public async Task<IResult> CreateStore(ISender sender, CreateStoreCommand command)
    {
        var result = await sender.Send(command);
        return Results.Created($"/api/stores/{result}", new { id = result });
    }

    public async Task<IResult> UpdateStore(ISender sender, int id, UpdateStoreCommand command)
    {
        var result = await sender.Send(command with { Id = id });
        return result ? Results.Ok() : Results.BadRequest("Store update failed");
    }

    public async Task<IResult> DeleteStore(ISender sender, int id)
    {
        var result = await sender.Send(new DeleteStoreCommand { Id = id });
        return result ? Results.Ok() : Results.BadRequest("Store deletion failed");
    }
}

