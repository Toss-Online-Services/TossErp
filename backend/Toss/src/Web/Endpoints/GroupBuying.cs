using Toss.Application.GroupBuying.Commands.ConfirmPool;
using Toss.Application.GroupBuying.Commands.CreatePool;
using Toss.Application.GroupBuying.Commands.JoinPool;
using Toss.Application.GroupBuying.Queries.GetActivePools;
using Toss.Application.GroupBuying.Queries.GetMyParticipations;
using Toss.Application.GroupBuying.Queries.GetPoolById;

namespace Toss.Web.Endpoints;

public class GroupBuying : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.MapPost("pools", CreatePool);
        group.MapGet("pools/active", GetActivePools);
        group.MapGet("pools/{id}", GetPoolById);
        group.MapPost("pools/{poolId}/join", JoinPool);
        group.MapPost("pools/{poolId}/confirm", ConfirmPool);
        group.MapGet("participations", GetMyParticipations);
    }

    public async Task<IResult> CreatePool(ISender sender, CreatePoolCommand command)
    {
        var id = await sender.Send(command);
        return Results.Created($"/api/group-buying/pools/{id}", new { id });
    }

    public async Task<IResult> GetActivePools(
        ISender sender,
        [AsParameters] GetActivePoolsQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> GetPoolById(ISender sender, int id)
    {
        var result = await sender.Send(new GetPoolByIdQuery { Id = id });
        return Results.Ok(result);
    }

    public async Task<IResult> JoinPool(ISender sender, int poolId, JoinPoolCommand command)
    {
        var commandWithId = command with { GroupBuyPoolId = poolId };
        var id = await sender.Send(commandWithId);
        return Results.Created($"/api/group-buying/participations/{id}", new { id });
    }

    public async Task<IResult> ConfirmPool(ISender sender, int poolId)
    {
        var result = await sender.Send(new ConfirmPoolCommand { PoolId = poolId });
        return result.Succeeded ? Results.Ok(result) : Results.BadRequest(result);
    }

    public async Task<IResult> GetMyParticipations(
        ISender sender,
        [AsParameters] GetMyParticipationsQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }
}

