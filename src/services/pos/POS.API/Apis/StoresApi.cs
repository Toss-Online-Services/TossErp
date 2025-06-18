using Microsoft.AspNetCore.Http.HttpResults;
using POS.API.Application.Models;
using POS.API.Services;

namespace POS.API.Apis;

public static class StoresApi
{
    public static RouteGroupBuilder MapStoresApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("");

        api.MapGet("/", GetStoresAsync);
        api.MapGet("/{id:guid}", GetStoreAsync);

        return api;
    }

    public static async Task<Ok<IEnumerable<StoreDto>>> GetStoresAsync(
        [AsParameters] POSServices services,
        [FromQuery] bool? isActive = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var stores = await services.Queries.GetStoresAsync(isActive, page, pageSize);
        return TypedResults.Ok(stores);
    }

    public static async Task<Results<Ok<StoreDto>, NotFound>> GetStoreAsync(
        Guid id,
        [AsParameters] POSServices services)
    {
        try
        {
            var store = await services.Queries.GetStoreAsync(id);
            if (store == null)
            {
                return TypedResults.NotFound();
            }
            return TypedResults.Ok(store);
        }
        catch
        {
            return TypedResults.NotFound();
        }
    }
} 
