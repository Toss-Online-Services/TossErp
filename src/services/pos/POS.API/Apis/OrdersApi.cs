using Microsoft.AspNetCore.Http.HttpResults;
using POS.API.Application.Models;
using POS.API.Services;

namespace POS.API.Apis;

public static class OrdersApi
{
    public static RouteGroupBuilder MapOrdersApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("");

        api.MapGet("/", GetOrdersAsync);
        api.MapGet("/{id:guid}", GetOrderAsync);

        return api;
    }

    public static async Task<Ok<IEnumerable<OrderDto>>> GetOrdersAsync(
        [AsParameters] POSServices services,
        [FromQuery] Guid? customerId = null,
        [FromQuery] string? status = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var orders = await services.Queries.GetOrdersAsync(customerId, status, fromDate, toDate, page, pageSize);
        return TypedResults.Ok(orders);
    }

    public static async Task<Results<Ok<OrderDto>, NotFound>> GetOrderAsync(
        Guid id,
        [AsParameters] POSServices services)
    {
        try
        {
            var order = await services.Queries.GetOrderAsync(id);
            if (order == null)
            {
                return TypedResults.NotFound();
            }
            return TypedResults.Ok(order);
        }
        catch
        {
            return TypedResults.NotFound();
        }
    }
} 
