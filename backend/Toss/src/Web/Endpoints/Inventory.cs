using Toss.Application.Inventory.Commands.AdjustStock;
using Toss.Application.Inventory.Commands.CreateProduct;
using Toss.Application.Inventory.Commands.UpdateProduct;
using Toss.Application.Inventory.Commands.DeleteProduct;
using Toss.Application.Inventory.Queries.GetCategories;
using Toss.Application.Inventory.Queries.GetLowStockAlerts;
using Toss.Application.Inventory.Queries.GetProductByBarcode;
using Toss.Application.Inventory.Queries.GetProductById;
using Toss.Application.Inventory.Queries.GetProductBySku;
using Toss.Application.Inventory.Queries.GetProducts;
using Toss.Application.Inventory.Queries.GetStockLevels;
using Toss.Application.Inventory.Queries.GetStockMovementHistory;
using Toss.Application.Inventory.Queries.SearchProducts;
using Toss.Application.Inventory.Queries.GetLowStockItems;
using Toss.Domain.Constants;

namespace Toss.Web.Endpoints;

public class Inventory : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.RequireAuthorization(Policies.RequireOwnerOrManager);
        group.MapPost("products", CreateProduct);
        group.MapPut("products/{id}", UpdateProduct);
        group.MapDelete("products/{id}", DeleteProduct);
        group.MapGet("products", GetProducts);
        group.MapGet("products/{id}", GetProductById);
        group.MapGet("products/by-sku", GetProductBySku);
        group.MapGet("products/by-barcode", GetProductByBarcode);
        group.MapGet("categories", GetCategories);
        group.MapGet("stock-levels", GetStockLevels);
        group.MapGet("low-stock-alerts", GetLowStockAlerts);
        group.MapPost("stock/adjust", AdjustStock);
        group.MapGet("stock/movements", GetStockMovementHistory);
        group.MapPost("products/search", SearchProducts);
        group.MapGet("low-stock-items", GetLowStockItems);
    }

    public async Task<IResult> SearchProducts(ISender sender, SearchProductsQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> GetLowStockItems(ISender sender, int shopId, int threshold = 10)
    {
        var query = new GetLowStockItemsQuery { ShopId = shopId, Threshold = threshold };
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> CreateProduct(ISender sender, CreateProductCommand command)                                                                      
    {
        var id = await sender.Send(command);
        return Results.Created($"/api/inventory/products/{id}", new { id });    
    }

    public async Task<IResult> UpdateProduct(ISender sender, int id, UpdateProductCommand command)
    {
        var result = await sender.Send(command with { Id = id });
        return result ? Results.Ok(new { message = "Product updated successfully" })
                     : Results.NotFound(new { message = "Product not found" });
    }

    public async Task<IResult> DeleteProduct(ISender sender, int id)
    {
        var result = await sender.Send(new DeleteProductCommand { Id = id });
        return result ? Results.Ok(new { message = "Product deleted successfully" })
                     : Results.NotFound(new { message = "Product not found" });
    }

    public async Task<IResult> GetProducts(
        ISender sender,
        [AsParameters] GetProductsQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> GetProductById(ISender sender, int id)
    {
        var result = await sender.Send(new GetProductByIdQuery { Id = id });
        return Results.Ok(result);
    }

    public async Task<IResult> GetProductBySku(ISender sender, string sku, int shopId)
    {
        var query = new GetProductBySkuQuery { Sku = sku, ShopId = shopId };
        var result = await sender.Send(query);

        if (result == null)
            return Results.NotFound(new { message = $"Product with SKU '{sku}' not found in shop {shopId}" });

        return Results.Ok(result);
    }

    public async Task<IResult> GetProductByBarcode(ISender sender, string barcode, int shopId)
    {
        var query = new GetProductByBarcodeQuery { Barcode = barcode, ShopId = shopId };
        var result = await sender.Send(query);

        if (result == null)
            return Results.NotFound(new { message = $"Product with barcode '{barcode}' not found in shop {shopId}" });

        return Results.Ok(result);
    }

    public async Task<IResult> GetCategories(ISender sender, int shopId)
    {
        var query = new GetCategoriesQuery { ShopId = shopId };
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

    public async Task<IResult> AdjustStock(ISender sender, AdjustStockCommand command)
    {
        var id = await sender.Send(command);
        return Results.Ok(new { id });
    }

    public async Task<IResult> GetStockMovementHistory(
        ISender sender,
        [AsParameters] GetStockMovementHistoryQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }
}

