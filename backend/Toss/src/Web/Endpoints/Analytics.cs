using Toss.Application.Analytics.Queries.GetAnalyticsSummary;
using Toss.Application.Analytics.Queries.GetWeeklyAnalytics;
using Toss.Domain.Constants;

namespace Toss.Web.Endpoints;

public class Analytics : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.RequireAuthorization(Policies.RequireOwnerOrManager);

        group.MapGet("summary", GetAnalyticsSummary)
            .WithName("GetAnalyticsSummary");

        group.MapGet("weekly", GetWeeklyAnalytics)
            .WithName("GetWeeklyAnalytics");
    }

    public async Task<IResult> GetAnalyticsSummary(ISender sender, [AsParameters] GetAnalyticsSummaryQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> GetWeeklyAnalytics(ISender sender, [AsParameters] GetWeeklyAnalyticsQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }
}

