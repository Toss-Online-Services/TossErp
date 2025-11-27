using Toss.Application.Dashboard.Queries.GetCashFlowSummary;
using Toss.Application.Dashboard.Queries.GetDashboardSummary;
using Toss.Application.Dashboard.Queries.GetSalesTrends;
using Toss.Application.Dashboard.Queries.GetTopProducts;
using Toss.Application.Dashboard.Queries.GetOrderStatusDistribution;
using Toss.Application.Dashboard.Queries.GetCategorySales;
using Toss.Domain.Constants;

namespace Toss.Web.Endpoints;

public class Dashboard : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.RequireAuthorization(Policies.RequireStaffOrAbove);
        group.MapGet("summary", GetDashboardSummary);
        group.MapGet("sales-trends", GetSalesTrends);
        group.MapGet("top-products", GetTopProducts);
        group.MapGet("cash-flow", GetCashFlowSummary);
        group.MapGet("order-status-distribution", GetOrderStatusDistribution);
        group.MapGet("category-sales", GetCategorySales);
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

    public async Task<IResult> GetOrderStatusDistribution(
        ISender sender,
        [AsParameters] GetOrderStatusDistributionQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> GetCategorySales(
        ISender sender,
        [AsParameters] GetCategorySalesQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }
}

