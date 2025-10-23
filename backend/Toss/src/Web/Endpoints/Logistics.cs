using Toss.Application.Logistics.Commands.AssignDriver;
using Toss.Application.Logistics.Commands.CreateSharedDeliveryRun;
using Toss.Application.Logistics.Commands.UpdateDeliveryStatus;
using Toss.Application.Logistics.Queries.GetSharedRuns;

namespace Toss.Web.Endpoints;

public class Logistics : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.MapPost("delivery-runs", CreateSharedDeliveryRun);
        group.MapGet("delivery-runs", GetSharedRuns);
        group.MapPost("delivery-runs/{id}/status", UpdateDeliveryStatus);
        group.MapPost("delivery-runs/{id}/assign-driver", AssignDriver);
    }

    public async Task<IResult> CreateSharedDeliveryRun(ISender sender, CreateSharedDeliveryRunCommand command)
    {
        var id = await sender.Send(command);
        return Results.Created($"/api/logistics/delivery-runs/{id}", new { id });
    }

    public async Task<IResult> GetSharedRuns(
        ISender sender,
        [AsParameters] GetSharedRunsQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> UpdateDeliveryStatus(ISender sender, int id, UpdateDeliveryStatusCommand command)
    {
        var result = await sender.Send(command with { DeliveryRunId = id });
        return result ? Results.Ok() : Results.BadRequest("Status update failed");
    }

    public async Task<IResult> AssignDriver(ISender sender, int id, AssignDriverCommand command)
    {
        var result = await sender.Send(command with { DeliveryRunId = id });
        return result ? Results.Ok() : Results.BadRequest("Driver assignment failed");
    }
}

