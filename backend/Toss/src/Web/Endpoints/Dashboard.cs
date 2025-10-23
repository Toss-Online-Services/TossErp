using Toss.Application.Dashboard.Queries.GetDashboardSummary;

namespace Toss.Web.Endpoints;

public class Dashboard : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.MapGet("summary", GetDashboardSummary);
    }

    public async Task<IResult> GetDashboardSummary(
        ISender sender,
        [AsParameters] GetDashboardSummaryQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }
}

