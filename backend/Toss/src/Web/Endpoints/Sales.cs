using Toss.Application.Sales.Commands.CreateSale;
using Toss.Application.Sales.Commands.GenerateReceipt;
using Toss.Application.Sales.Commands.VoidSale;
using Toss.Application.Sales.Commands.UpdateSaleStatus;
using Toss.Application.Sales.Commands.ProcessRefund;
using Toss.Application.Sales.Queries.GetDailySummary;
using Toss.Application.Sales.Queries.GetSaleById;
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

        group.MapGet("{id}", GetSaleById)
            .WithName("GetSaleById");

        group.MapGet("daily-summary", GetDailySummary)
            .WithName("GetDailySummary");
        
        group.MapPost("{id}/void", VoidSale)
            .WithName("VoidSale");
        
        group.MapPost("{id}/receipt", GenerateReceipt)
            .WithName("GenerateReceipt");
        
        group.MapPost("{id}/status", UpdateSaleStatus)
            .WithName("UpdateSaleStatus");
        
        group.MapPost("{id}/refund", ProcessRefund)
            .WithName("ProcessRefund");
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

    public async Task<IResult> GetSaleById(ISender sender, int id)
    {
        var result = await sender.Send(new GetSaleByIdQuery { Id = id });
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

    public async Task<IResult> UpdateSaleStatus(ISender sender, int id, UpdateSaleStatusCommand command)
    {
        var result = await sender.Send(command with { SaleId = id });
        return result ? Results.Ok() : Results.BadRequest("Status update failed");
    }

    public async Task<IResult> ProcessRefund(ISender sender, int id, ProcessRefundCommand command)
    {
        var refundId = await sender.Send(command with { SaleId = id });
        return Results.Ok(new { refundId });
    }
}

