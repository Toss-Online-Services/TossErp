using Toss.Application.Logistics.Commands.CreateSharedDeliveryRun;

namespace Toss.Web.Endpoints;

public class Logistics : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.MapPost("delivery-runs", CreateSharedDeliveryRun);
    }

    public async Task<IResult> CreateSharedDeliveryRun(ISender sender, CreateSharedDeliveryRunCommand command)
    {
        var id = await sender.Send(command);
        return Results.Created($"/api/logistics/delivery-runs/{id}", new { id });
    }
}

