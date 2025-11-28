using Toss.Application.Accounting.Commands.CreateAccount;
using Toss.Application.Accounting.Commands.RecordCashIn;
using Toss.Application.Accounting.Commands.RecordCashOut;
using Toss.Application.Accounting.Commands.RecordTransfer;
using Toss.Application.Accounting.Queries.GetAccounts;
using Toss.Application.Accounting.Queries.GetCashbookEntries;
using Toss.Domain.Constants;

namespace Toss.Web.Endpoints;

public class Accounting : EndpointGroupBase
{
    public override string? GroupName => "accounting";

    public override void Map(RouteGroupBuilder group)
    {
        group.RequireAuthorization(Policies.RequireOwnerOrManager);

        // Account management
        group.MapPost("accounts", CreateAccount);
        group.MapGet("accounts", GetAccounts);

        // Cashbook operations
        group.MapPost("cashbook/in", RecordCashIn);
        group.MapPost("cashbook/out", RecordCashOut);
        group.MapPost("cashbook/transfer", RecordTransfer);
        group.MapGet("cashbook/entries", GetCashbookEntries);
    }

    public async Task<IResult> CreateAccount(ISender sender, CreateAccountCommand command)
    {
        var id = await sender.Send(command);
        return Results.Created($"/api/accounting/accounts/{id}", new { id });
    }

    public async Task<IResult> GetAccounts(ISender sender, [AsParameters] GetAccountsQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> RecordCashIn(ISender sender, RecordCashInCommand command)
    {
        var id = await sender.Send(command);
        return Results.Created($"/api/accounting/cashbook/entries/{id}", new { id });
    }

    public async Task<IResult> RecordCashOut(ISender sender, RecordCashOutCommand command)
    {
        var id = await sender.Send(command);
        return Results.Created($"/api/accounting/cashbook/entries/{id}", new { id });
    }

    public async Task<IResult> RecordTransfer(ISender sender, RecordTransferCommand command)
    {
        var id = await sender.Send(command);
        return Results.Created($"/api/accounting/cashbook/entries/{id}", new { id });
    }

    public async Task<IResult> GetCashbookEntries(ISender sender, [AsParameters] GetCashbookEntriesQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }
}

