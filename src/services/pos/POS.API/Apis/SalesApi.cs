using Microsoft.AspNetCore.Http.HttpResults;
using POS.API.Application.Models;
using POS.API.Services;

namespace POS.API.Apis;

public static class SalesApi
{
    public static RouteGroupBuilder MapSalesApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("");

        api.MapGet("/", GetSalesAsync);
        api.MapGet("/{id:guid}", GetSaleAsync);

        return api;
    }

    public static async Task<Ok<IEnumerable<SaleDto>>> GetSalesAsync(
        [AsParameters] POSServices services,
        [FromQuery] Guid? customerId = null,
        [FromQuery] Guid? storeId = null,
        [FromQuery] Guid? staffId = null,
        [FromQuery] string? paymentStatus = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var sales = await services.Queries.GetSalesAsync(customerId, storeId, staffId, paymentStatus, fromDate, toDate, page, pageSize);
        return TypedResults.Ok(sales);
    }

    public static async Task<Results<Ok<SaleDto>, NotFound>> GetSaleAsync(
        Guid id,
        [AsParameters] POSServices services)
    {
        try
        {
            var sale = await services.Queries.GetSaleAsync(id);
            if (sale == null)
            {
                return TypedResults.NotFound();
            }
            return TypedResults.Ok(sale);
        }
        catch
        {
            return TypedResults.NotFound();
        }
    }
} 
