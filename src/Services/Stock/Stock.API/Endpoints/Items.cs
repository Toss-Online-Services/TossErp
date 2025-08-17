using TossErp.Stock.Application.Items.Commands.CreateItem;
using TossErp.Stock.Application.Items.Commands.UpdateItem;
using TossErp.Stock.Application.Items.Commands.DeleteItem;
using TossErp.Stock.Application.Items.Queries.GetItems;
using TossErp.Stock.Application.Items.Queries.GetItemById;
using TossErp.Stock.Application.Items.Queries.GetItemByBarcode;
using TossErp.Stock.Application.Items.Queries.GetItemBySku;
using TossErp.Stock.Application.Items.Queries.GetItemStockHistory;
using TossErp.Stock.Application.Common.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;

namespace TossErp.Stock.API.Endpoints;

/// <summary>
/// Items endpoint group for managing inventory items/products
/// </summary>
public class Items : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup(this);
        
        // Basic CRUD operations
        group.MapGet(GetItems, "");
        group.MapGet(GetItem, "{id}");
        group.MapPost(CreateItem, "");
        group.MapPut(UpdateItem, "{id}");
        group.MapDelete(DeleteItem, "{id}");
            
        // Stock-specific operations
        group.MapGet(GetLowStockItems, "low-stock");
        group.MapGet(GetOutOfStockItems, "out-of-stock");
        group.MapGet(GetStockItems, "stock-items");
        group.MapGet(GetStockOverview, "overview");
        group.MapGet(GetCategories, "categories");
        group.MapGet(GetItemStockHistory, "{id}/stock-history");
        group.MapGet(GetItemBySku, "sku/{sku}");
        group.MapGet(GetItemByBarcode, "barcode/{barcode}");
    }

    /// <summary>
    /// Get all items with optional filtering and pagination
    /// </summary>
    public async Task<Results<Ok<GetItemsResponse>, BadRequest>> GetItems(
        ISender sender, 
        [AsParameters] GetItemsQuery query)
    {
        try
        {
            var response = await sender.Send(query);
            return TypedResults.Ok(response);
        }
        catch (Exception)
        {
            return TypedResults.BadRequest();
        }
    }

    /// <summary>
    /// Get a specific item by ID
    /// </summary>
    public async Task<Results<Ok<ItemDto>, NotFound>> GetItem(
        ISender sender, 
        Guid id)
    {
        var item = await sender.Send(new GetItemByIdQuery { Id = id });
        
        if (item == null)
            return TypedResults.NotFound();
            
        return TypedResults.Ok(item);
    }

    /// <summary>
    /// Create a new item
    /// </summary>
    public async Task<Results<Created<ItemDto>, BadRequest>> CreateItem(
        ISender sender, 
        CreateItemCommand command)
    {
        try
        {
            var item = await sender.Send(command);
            return TypedResults.Created($"/api/Items/{item.Id}", item);
        }
        catch (Exception)
        {
            return TypedResults.BadRequest();
        }
    }

    /// <summary>
    /// Update an existing item
    /// </summary>
    public async Task<Results<NoContent, BadRequest, NotFound>> UpdateItem(
        ISender sender, 
        Guid id, 
        UpdateItemCommand command)
    {
        if (id != command.Id) 
            return TypedResults.BadRequest();
            
        try
        {
            await sender.Send(command);
            return TypedResults.NoContent();
        }
        catch (Exception)
        {
            return TypedResults.NotFound();
        }
    }

    /// <summary>
    /// Delete an item
    /// </summary>
    public async Task<Results<NoContent, NotFound>> DeleteItem(
        ISender sender, 
        Guid id)
    {
        try
        {
            await sender.Send(new DeleteItemCommand { Id = id });
            return TypedResults.NoContent();
        }
        catch (Exception)
        {
            return TypedResults.NotFound();
        }
    }

    /// <summary>
    /// Get items with low stock (below reorder level)
    /// </summary>
    public async Task<Results<Ok<GetItemsResponse>, BadRequest>> GetLowStockItems(
        ISender sender,
        [AsParameters] GetItemsQuery query)
    {
        try
        {
            var lowStockQuery = query with { LowStockOnly = true };
            var response = await sender.Send(lowStockQuery);
            return TypedResults.Ok(response);
        }
        catch (Exception)
        {
            return TypedResults.BadRequest();
        }
    }

    /// <summary>
    /// Get out of stock items
    /// </summary>
    public async Task<Results<Ok<GetItemsResponse>, BadRequest>> GetOutOfStockItems(
        ISender sender,
        [AsParameters] GetItemsQuery query)
    {
        try
        {
            var outOfStockQuery = query with { OutOfStockOnly = true };
            var response = await sender.Send(outOfStockQuery);
            return TypedResults.Ok(response);
        }
        catch (Exception)
        {
            return TypedResults.BadRequest();
        }
    }

    /// <summary>
    /// Get stock items only (exclude services)
    /// </summary>
    public async Task<Results<Ok<GetItemsResponse>, BadRequest>> GetStockItems(
        ISender sender,
        [AsParameters] GetItemsQuery query)
    {
        try
        {
            var stockItemsQuery = query with { StockItemsOnly = true };
            var response = await sender.Send(stockItemsQuery);
            return TypedResults.Ok(response);
        }
        catch (Exception)
        {
            return TypedResults.BadRequest();
        }
    }

    /// <summary>
    /// Get stock history for a specific item
    /// </summary>
    public async Task<Results<Ok<List<StockLedgerEntryDto>>, NotFound, BadRequest>> GetItemStockHistory(
        ISender sender,
        Guid itemId)
    {
        try
        {
            var query = new GetItemStockHistoryQuery { ItemId = itemId };
            var history = await sender.Send(query);
            return TypedResults.Ok(history);
        }
        catch (Exception)
        {
            return TypedResults.BadRequest();
        }
    }

    /// <summary>
    /// Get item by SKU
    /// </summary>
    public async Task<Results<Ok<ItemDto>, NotFound, BadRequest>> GetItemBySku(
        ISender sender,
        string sku)
    {
        try
        {
            var query = new GetItemBySkuQuery { Sku = sku };
            var item = await sender.Send(query);
            
            if (item == null)
                return TypedResults.NotFound();
                
            return TypedResults.Ok(item);
        }
        catch (Exception)
        {
            return TypedResults.BadRequest();
        }
    }

    /// <summary>
    /// Get item by barcode
    /// </summary>
    public async Task<Results<Ok<ItemDto>, NotFound, BadRequest>> GetItemByBarcode(
        ISender sender,
        string barcode)
    {
        try
        {
            var query = new GetItemByBarcodeQuery { Barcode = barcode };
            var item = await sender.Send(query);
            
            if (item == null)
                return TypedResults.NotFound();
                
            return TypedResults.Ok(item);
        }
        catch (Exception)
        {
            return TypedResults.BadRequest();
        }
    }

    /// <summary>
    /// Get stock overview with summary statistics
    /// </summary>
    public async Task<Results<Ok<StockOverviewDto>, BadRequest>> GetStockOverview(
        ISender sender)
    {
        try
        {
            // For now, we'll calculate this from existing queries
            var allItemsQuery = new GetItemsQuery { PageSize = int.MaxValue };
            var allItems = await sender.Send(allItemsQuery);
            
            var lowStockQuery = new GetItemsQuery { LowStockOnly = true, PageSize = int.MaxValue };
            var lowStockItems = await sender.Send(lowStockQuery);
            
            var outOfStockQuery = new GetItemsQuery { OutOfStockOnly = true, PageSize = int.MaxValue };
            var outOfStockItems = await sender.Send(outOfStockQuery);

            var overview = new StockOverviewDto
            {
                TotalItems = allItems.TotalCount,
                LowStockItems = lowStockItems.TotalCount,
                OutOfStockItems = outOfStockItems.TotalCount,
                TotalValue = allItems.Items.Sum(i => i.Price * i.QuantityOnHand),
                TotalCategories = allItems.Items.Select(i => i.Category).Distinct().Count(),
                CategorySummary = allItems.Items
                    .GroupBy(i => i.Category)
                    .Select(g => new CategorySummaryDto
                    {
                        Category = g.Key,
                        ItemCount = g.Count(),
                        TotalValue = g.Sum(i => i.Price * i.QuantityOnHand)
                    })
                    .ToList()
            };
            
            return TypedResults.Ok(overview);
        }
        catch (Exception)
        {
            return TypedResults.BadRequest();
        }
    }

    /// <summary>
    /// Get all available categories
    /// </summary>
    public async Task<Results<Ok<List<string>>, BadRequest>> GetCategories(
        ISender sender)
    {
        try
        {
            var query = new GetItemsQuery { PageSize = int.MaxValue };
            var items = await sender.Send(query);
            
            var categories = items.Items
                .Select(i => i.Category)
                .Where(c => !string.IsNullOrEmpty(c))
                .Distinct()
                .OrderBy(c => c)
                .ToList();
            
            return TypedResults.Ok(categories);
        }
        catch (Exception)
        {
            return TypedResults.BadRequest();
        }
    }
} 
