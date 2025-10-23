using Toss.Application.Sales.Commands.CreateSale;
using Toss.Application.Sales.Commands.GenerateReceipt;
using Toss.Application.Sales.Commands.VoidSale;
using Toss.Application.Sales.Queries.GetDailySummary;
using Toss.Application.Sales.Queries.GetSales;

namespace Toss.Web.Endpoints;

public class Sales : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.MapPost(string.Empty, CreateSale)
            .WithName("CreateSale");

        group.MapGet(string.Empty, GetSales)
            .WithName("GetSales");

        group.MapGet("daily-summary", GetDailySummary)
            .WithName("GetDailySummary");
        
        group.MapPost("{id}/void", VoidSale)
            .WithName("VoidSale");
        
        group.MapPost("{id}/receipt", GenerateReceipt)
            .WithName("GenerateReceipt");
    }

    public async Task<IResult> CreateSale(ISender sender, CreateSaleCommand command)
    {
        var id = await sender.Send(command);
        return Results.Created($"/api/sales/{id}", new { id });
    }

    public async Task<IResult> GetSales(
        ISender sender,
        [AsParameters] GetSalesQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> GetDailySummary(
        ISender sender,
        [AsParameters] GetDailySummaryQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> VoidSale(ISender sender, int id, VoidSaleCommand command)
    {
        var result = await sender.Send(command with { SaleId = id });
        return result ? Results.Ok() : Results.BadRequest("Sale cannot be voided");
    }

    public async Task<IResult> GenerateReceipt(ISender sender, int id)
    {
        var result = await sender.Send(new GenerateReceiptCommand { SaleId = id });
        return Results.Ok(result);
    }
}

