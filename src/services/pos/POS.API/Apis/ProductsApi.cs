using Microsoft.AspNetCore.Http.HttpResults;
using POS.API.Application.Commands;
using POS.API.Application.Models;
using POS.API.Services;

namespace POS.API.Apis;

public static class ProductsApi
{
    public static RouteGroupBuilder MapProductsApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("");

        api.MapGet("/", GetProductsAsync);
        api.MapGet("/{id:guid}", GetProductAsync);
        api.MapPost("/", CreateProductAsync);
        api.MapPut("/{id:guid}", UpdateProductAsync);
        api.MapDelete("/{id:guid}", DeleteProductAsync);

        return api;
    }

    public static async Task<Ok<IEnumerable<ProductDto>>> GetProductsAsync(
        [AsParameters] POSServices services,
        [FromQuery] string? searchTerm = null,
        [FromQuery] Guid? categoryId = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var products = await services.Queries.GetProductsAsync(searchTerm, categoryId, isActive, page, pageSize);
        return TypedResults.Ok(products);
    }

    public static async Task<Results<Ok<ProductDto>, NotFound>> GetProductAsync(
        Guid id,
        [AsParameters] POSServices services)
    {
        try
        {
            var product = await services.Queries.GetProductAsync(id);
            if (product == null)
            {
                return TypedResults.NotFound();
            }
            return TypedResults.Ok(product);
        }
        catch
        {
            return TypedResults.NotFound();
        }
    }

    public static async Task<Results<Created<Guid>, BadRequest<string>>> CreateProductAsync(
        CreateProductCommand command,
        [AsParameters] POSServices services)
    {
        services.Logger.LogInformation(
            "Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
            command.GetType().Name,
            nameof(command.Name),
            command.Name,
            command);

        try
        {
            var productId = await services.Mediator.Send(command);
            services.Logger.LogInformation("CreateProductCommand succeeded - ProductId: {ProductId}", productId);
            return TypedResults.Created($"/api/products/{productId}", productId);
        }
        catch (Exception ex)
        {
            services.Logger.LogError(ex, "CreateProductCommand failed");
            return TypedResults.BadRequest(ex.Message);
        }
    }

    public static async Task<Results<Ok, BadRequest<string>, NotFound>> UpdateProductAsync(
        Guid id,
        UpdateProductCommand command,
        [AsParameters] POSServices services)
    {
        if (id != command.Id)
        {
            return TypedResults.BadRequest("ID mismatch");
        }

        services.Logger.LogInformation(
            "Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
            command.GetType().Name,
            nameof(command.Id),
            command.Id,
            command);

        try
        {
            var result = await services.Mediator.Send(command);
            if (!result)
            {
                return TypedResults.NotFound();
            }

            services.Logger.LogInformation("UpdateProductCommand succeeded - ProductId: {ProductId}", command.Id);
            return TypedResults.Ok();
        }
        catch (Exception ex)
        {
            services.Logger.LogError(ex, "UpdateProductCommand failed");
            return TypedResults.BadRequest(ex.Message);
        }
    }

    public static async Task<Results<NoContent, BadRequest<string>, NotFound>> DeleteProductAsync(
        Guid id,
        [AsParameters] POSServices services)
    {
        var command = new DeleteProductCommand(id);

        services.Logger.LogInformation(
            "Sending command: {CommandName} - {IdProperty}: {CommandId}",
            command.GetType().Name,
            nameof(command.Id),
            command.Id);

        try
        {
            var result = await services.Mediator.Send(command);
            if (!result)
            {
                return TypedResults.NotFound();
            }

            services.Logger.LogInformation("DeleteProductCommand succeeded - ProductId: {ProductId}", id);
            return TypedResults.NoContent();
        }
        catch (Exception ex)
        {
            services.Logger.LogError(ex, "DeleteProductCommand failed");
            return TypedResults.BadRequest(ex.Message);
        }
    }
} 
