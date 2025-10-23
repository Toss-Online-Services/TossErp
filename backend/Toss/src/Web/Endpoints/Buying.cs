using Toss.Application.Buying.Commands.ApprovePurchaseOrder;
using Toss.Application.Buying.Commands.CreatePurchaseOrder;
using Toss.Application.Buying.Queries.GetPurchaseOrderById;

namespace Toss.Web.Endpoints;

public class Buying : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.MapPost("purchase-orders", CreatePurchaseOrder);
        group.MapGet("purchase-orders/{id}", GetPurchaseOrderById);
        group.MapPost("purchase-orders/{id}/approve", ApprovePurchaseOrder);
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
}

