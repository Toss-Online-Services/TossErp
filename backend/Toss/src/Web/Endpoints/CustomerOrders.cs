using Toss.Application.CustomerOrders.Commands.CreateCustomerOrder;
using Toss.Application.CustomerOrders.Commands.UpdateCustomerOrderStatus;
using Toss.Application.CustomerOrders.Commands.CancelCustomerOrder;
using Toss.Application.CustomerOrders.Queries.GetCustomerOrders;

namespace Toss.Web.Endpoints;

public class CustomerOrders : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.MapPost(string.Empty, CreateOrder)
            .WithName("CreateCustomerOrder")
            .WithSummary("Create a new customer order");

        group.MapGet(string.Empty, GetOrders)
            .WithName("GetCustomerOrders")
            .WithSummary("Get customer orders");

        group.MapPost("{id}/status", UpdateOrderStatus)
            .WithName("UpdateCustomerOrderStatus")
            .WithSummary("Update order status");

        group.MapPost("{id}/cancel", CancelOrder)
            .WithName("CancelCustomerOrder")
            .WithSummary("Cancel a customer order");
    }

    public async Task<IResult> CreateOrder(ISender sender, CreateCustomerOrderCommand command)
    {
        var id = await sender.Send(command);
        return Results.Created($"/api/customer-orders/{id}", new { id });
    }

    public async Task<IResult> GetOrders(
        ISender sender,
        [AsParameters] GetCustomerOrdersQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> UpdateOrderStatus(ISender sender, int id, UpdateCustomerOrderStatusCommand command)
    {
        var result = await sender.Send(command with { OrderId = id });
        return result ? Results.Ok() : Results.BadRequest("Status update failed");
    }

    public async Task<IResult> CancelOrder(ISender sender, int id, CancelCustomerOrderCommand command)
    {
        var result = await sender.Send(command with { OrderId = id });
        return result ? Results.Ok() : Results.BadRequest("Cancellation failed");
    }
}

