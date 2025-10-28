using Toss.Application.Buying.Commands.ApprovePurchaseOrder;
using Toss.Application.Buying.Commands.CreatePurchaseOrder;
using Toss.Application.Buying.Commands.UpdatePurchaseOrderStatus;
using Toss.Application.Buying.Commands.ReceiveGoods;
using Toss.Application.Buying.Queries.GetPurchaseOrderById;
using Toss.Application.Buying.Queries.GetPurchaseOrders;

namespace Toss.Web.Endpoints;

public class Buying : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.MapGet("purchase-orders", GetPurchaseOrders);
        group.MapPost("purchase-orders", CreatePurchaseOrder);
        group.MapGet("purchase-orders/{id}", GetPurchaseOrderById);
        group.MapPost("purchase-orders/{id}/approve", ApprovePurchaseOrder);
        group.MapPost("purchase-orders/{id}/status", UpdatePurchaseOrderStatus);
        group.MapPost("purchase-orders/{id}/receive", ReceiveGoods);
    }

    public async Task<IResult> GetPurchaseOrders(ISender sender, int? shopId, string? status, int? skip, int? take)
    {
        var query = new GetPurchaseOrdersQuery
        {
            ShopId = shopId,
            Status = status,
            Skip = skip ?? 0,
            Take = take ?? 50
        };
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> CreatePurchaseOrder(ISender sender, CreatePurchaseOrderCommand command)
    {
        var id = await sender.Send(command);
        return Results.Created($"/api/buying/purchase-orders/{id}", new { id });
    }

    public async Task<IResult> GetPurchaseOrderById(ISender sender, int id)
    {
        var result = await sender.Send(new GetPurchaseOrderByIdQuery { Id = id });
        return Results.Ok(result);
    }

    public async Task<IResult> ApprovePurchaseOrder(ISender sender, int id, ApprovePurchaseOrderCommand command)
    {
        var result = await sender.Send(command with { PurchaseOrderId = id });
        return result ? Results.Ok() : Results.BadRequest("PO cannot be approved");
    }

    public async Task<IResult> UpdatePurchaseOrderStatus(ISender sender, int id, UpdatePurchaseOrderStatusCommand command)
    {
        var result = await sender.Send(command with { PurchaseOrderId = id });
        return result ? Results.Ok() : Results.BadRequest("Status update failed");
    }

    public async Task<IResult> ReceiveGoods(ISender sender, int id, ReceiveGoodsCommand command)
    {
        var result = await sender.Send(command with { PurchaseOrderId = id });
        return result ? Results.Ok() : Results.BadRequest("Goods receipt failed");
    }
}

