using Toss.Application.Accounting.Queries.GetProfitAndLoss;
using Toss.Application.Accounting.Queries.GetCashflowSummary;
using Toss.Application.Accounting.Queries.GetDebtors;
using Toss.Application.Accounting.Queries.GetCreditors;
using Toss.Domain.Constants;

namespace Toss.Web.Endpoints;

public class Reports : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.RequireAuthorization(Policies.RequireOwnerOrManager);

        // Financial reports
        group.MapGet("pnl", GetProfitAndLoss)
            .WithName("GetProfitAndLoss");

        group.MapGet("cashflow", GetCashflowSummary)
            .WithName("GetCashflowSummary");

        // Accounts receivable/payable
        group.MapGet("debtors", GetDebtors)
            .WithName("GetDebtors");

        group.MapGet("creditors", GetCreditors)
            .WithName("GetCreditors");
    }

    public async Task<IResult> GetProfitAndLoss(ISender sender, [AsParameters] GetProfitAndLossQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> GetCashflowSummary(ISender sender, [AsParameters] GetCashflowSummaryQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> GetDebtors(ISender sender, [AsParameters] GetDebtorsQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> GetCreditors(ISender sender, [AsParameters] GetCreditorsQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }
}

