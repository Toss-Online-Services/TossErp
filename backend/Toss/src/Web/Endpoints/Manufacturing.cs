using Toss.Application.Manufacturing.Commands.CreateBillOfMaterials;
using Toss.Application.Manufacturing.Commands.CreateProductionOrder;
using Toss.Application.Manufacturing.Commands.CompleteProductionOrder;
using Toss.Application.Manufacturing.Queries.GetBillOfMaterials;
using Toss.Application.Manufacturing.Queries.GetProductionOrders;
using Toss.Domain.Constants;

namespace Toss.Web.Endpoints;

public class Manufacturing : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.RequireAuthorization(Policies.RequireOwnerOrManager);

        // BOM endpoints
        group.MapPost("bom", CreateBom)
            .WithName("CreateBillOfMaterials");

        group.MapGet("bom", GetBoms)
            .WithName("GetBillOfMaterials");

        group.MapGet("bom/{id}", GetBomById)
            .WithName("GetBillOfMaterialsById");

        // Production Order endpoints
        group.MapPost("orders", CreateProductionOrder)
            .WithName("CreateProductionOrder");

        group.MapGet("orders", GetProductionOrders)
            .WithName("GetProductionOrders");

        group.MapGet("orders/{id}", GetProductionOrderById)
            .WithName("GetProductionOrderById");

        group.MapPost("orders/{id}/complete", CompleteProductionOrder)
            .WithName("CompleteProductionOrder");
    }

    public async Task<IResult> CreateBom(ISender sender, CreateBillOfMaterialsCommand command)
    {
        var id = await sender.Send(command);
        return Results.Created($"/api/manufacturing/bom/{id}", new { id });
    }

    public async Task<IResult> GetBoms(ISender sender, [AsParameters] GetBillOfMaterialsQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> GetBomById(ISender sender, int id)
    {
        var result = await sender.Send(new GetBillOfMaterialsByIdQuery { Id = id });
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    public async Task<IResult> CreateProductionOrder(ISender sender, CreateProductionOrderCommand command)
    {
        var id = await sender.Send(command);
        return Results.Created($"/api/manufacturing/orders/{id}", new { id });
    }

    public async Task<IResult> GetProductionOrders(ISender sender, [AsParameters] GetProductionOrdersQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> GetProductionOrderById(ISender sender, int id)
    {
        var result = await sender.Send(new GetProductionOrderByIdQuery { Id = id });
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    public async Task<IResult> CompleteProductionOrder(ISender sender, int id, CompleteProductionOrderCommand command)
    {
        var result = await sender.Send(command with { ProductionOrderId = id });
        return Results.Ok(result);
    }
}

