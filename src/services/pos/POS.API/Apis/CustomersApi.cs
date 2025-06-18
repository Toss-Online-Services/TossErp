using Microsoft.AspNetCore.Http.HttpResults;
using POS.API.Application.Models;
using POS.API.Services;

namespace POS.API.Apis;

public static class CustomersApi
{
    public static RouteGroupBuilder MapCustomersApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("");

        api.MapGet("/", GetCustomersAsync);
        api.MapGet("/{id:guid}", GetCustomerAsync);

        return api;
    }

    public static async Task<Ok<IEnumerable<CustomerDto>>> GetCustomersAsync(
        [AsParameters] POSServices services,
        [FromQuery] string? searchTerm = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var customers = await services.Queries.GetCustomersAsync(searchTerm, isActive, page, pageSize);
        return TypedResults.Ok(customers);
    }

    public static async Task<Results<Ok<CustomerDto>, NotFound>> GetCustomerAsync(
        Guid id,
        [AsParameters] POSServices services)
    {
        try
        {
            var customer = await services.Queries.GetCustomerAsync(id);
            if (customer == null)
            {
                return TypedResults.NotFound();
            }
            return TypedResults.Ok(customer);
        }
        catch
        {
            return TypedResults.NotFound();
        }
    }
} 
