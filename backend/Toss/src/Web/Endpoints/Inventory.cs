using Toss.Application.Inventory.Commands.CreateProduct;
using Toss.Application.Inventory.Queries.GetLowStockAlerts;
using Toss.Application.Inventory.Queries.GetProducts;
using Toss.Application.Inventory.Queries.GetStockLevels;

namespace Toss.Web.Endpoints;

public class Inventory : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.MapPost("products", CreateProduct);
        group.MapGet("products", GetProducts);
        group.MapGet("stock-levels", GetStockLevels);
        group.MapGet("low-stock-alerts", GetLowStockAlerts);
    }

    public async Task<IResult> CreateProduct(ISender sender, CreateProductCommand command)
    {
        var id = await sender.Send(command);
        return Results.Created($"/api/inventory/products/{id}", new { id });
    }

    public async Task<IResult> GetProducts(
        ISender sender,
        [AsParameters] GetProductsQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> GetStockLevels(
        ISender sender,
        [AsParameters] GetStockLevelsQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> GetLowStockAlerts(
        ISender sender,
        [AsParameters] GetLowStockAlertsQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }
}

