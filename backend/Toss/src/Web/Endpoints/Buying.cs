using Toss.Application.Buying.Commands.CreatePurchaseOrder;

namespace Toss.Web.Endpoints;

public class Buying : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.MapPost("purchase-orders", CreatePurchaseOrder);
    }

    public async Task<IResult> CreatePurchaseOrder(ISender sender, CreatePurchaseOrderCommand command)
    {
        var id = await sender.Send(command);
        return Results.Created($"/api/buying/purchase-orders/{id}", new { id });
    }
}

