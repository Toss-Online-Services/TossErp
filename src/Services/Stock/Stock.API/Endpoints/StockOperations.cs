using TossErp.Stock.Application.Commands.IssueStock;
using TossErp.Stock.Application.Commands.ReceiveStock;
using TossErp.Stock.Application.Commands.TransferStock;
using TossErp.Stock.Application.Commands.AdjustStock;
using Microsoft.AspNetCore.Http.HttpResults;

namespace TossErp.Stock.API.Endpoints;

/// <summary>
/// Stock Operations endpoint group for managing stock movements
/// </summary>
public class StockOperations : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup(this);
        
        // Stock movement operations
        group.MapPost(IssueStock, "issue");
        group.MapPost(ReceiveStock, "receive");
        group.MapPost(TransferStock, "transfer");
        group.MapPost(AdjustStock, "adjust");
    }

    /// <summary>
    /// Issue stock from a specific stock level
    /// </summary>
    public async Task<Results<Ok<bool>, BadRequest>> IssueStock(
        ISender sender, 
        IssueStockCommand command)
    {
        try
        {
            var result = await sender.Send(command);
            return TypedResults.Ok(result);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest();
        }
    }

    /// <summary>
    /// Receive stock into a specific stock level
    /// </summary>
    public async Task<Results<Ok<bool>, BadRequest>> ReceiveStock(
        ISender sender, 
        ReceiveStockCommand command)
    {
        try
        {
            var result = await sender.Send(command);
            return TypedResults.Ok(result);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest();
        }
    }

    /// <summary>
    /// Transfer stock between stock levels
    /// </summary>
    public async Task<Results<Ok<bool>, BadRequest>> TransferStock(
        ISender sender, 
        TransferStockCommand command)
    {
        try
        {
            var result = await sender.Send(command);
            return TypedResults.Ok(result);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest();
        }
    }

    /// <summary>
    /// Adjust stock levels (for stock takes, corrections, etc.)
    /// </summary>
    public async Task<Results<Ok<bool>, BadRequest>> AdjustStock(
        ISender sender, 
        AdjustStockCommand command)
    {
        try
        {
            var result = await sender.Send(command);
            return TypedResults.Ok(result);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest();
        }
    }
}
