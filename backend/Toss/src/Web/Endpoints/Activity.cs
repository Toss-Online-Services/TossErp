using Toss.Application.Audit.Queries.GetActivityToday;
using Toss.Application.Audit.Queries.GetEntityTimeline;
using Toss.Domain.Constants;

namespace Toss.Web.Endpoints;

public class Activity : EndpointGroupBase
{
    public override string? GroupName => "activity";

    public override void Map(RouteGroupBuilder group)
    {
        group.RequireAuthorization(Policies.RequireOwnerOrManager);

        group.MapGet("today", GetActivityToday)
            .WithName("GetActivityToday");

        group.MapGet("timeline/{entityType}/{entityId}", GetEntityTimeline)
            .WithName("GetEntityTimeline");
    }

    public async Task<IResult> GetActivityToday(ISender sender, [AsParameters] GetActivityTodayQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> GetEntityTimeline(ISender sender, string entityType, int entityId)
    {
        var result = await sender.Send(new GetEntityTimelineQuery
        {
            EntityType = entityType,
            EntityId = entityId
        });
        return Results.Ok(result);
    }
}

