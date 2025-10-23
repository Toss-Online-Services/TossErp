using Toss.Application.Dashboard.Queries.GetCashFlowSummary;
using Toss.Application.Dashboard.Queries.GetDashboardSummary;
using Toss.Application.Dashboard.Queries.GetSalesTrends;
using Toss.Application.Dashboard.Queries.GetTopProducts;

namespace Toss.Web.Endpoints;

public class Dashboard : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.MapGet("summary", GetDashboardSummary);
        group.MapGet("sales-trends", GetSalesTrends);
        group.MapGet("top-products", GetTopProducts);
        group.MapGet("cash-flow", GetCashFlowSummary);
    }

    public async Task<IResult> GetDashboardSummary(
        ISender sender,
        [AsParameters] GetDashboardSummaryQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> GetSalesTrends(
        ISender sender,
        [AsParameters] GetSalesTrendsQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> GetTopProducts(
        ISender sender,
        [AsParameters] GetTopProductsQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> GetCashFlowSummary(
        ISender sender,
        [AsParameters] GetCashFlowSummaryQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }
}

