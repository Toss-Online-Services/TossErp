using Toss.Application.CRM.Commands.CreateCustomer;
using Toss.Application.CRM.Commands.CreateCustomerInteraction;
using Toss.Application.CRM.Commands.DeleteCustomer;
using Toss.Application.CRM.Commands.UpdateCustomer;
using Toss.Application.CRM.Queries.GetCustomerInteractions;
using Toss.Application.CRM.Queries.GetCustomerProfile;
using Toss.Application.CRM.Queries.GetCustomers;
using Toss.Application.CRM.Queries.SearchCustomers;
using Toss.Domain.Constants;

namespace Toss.Web.Endpoints;

public class CRM : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.RequireAuthorization(Policies.RequireStaffOrAbove);
        group.MapPost("customers", CreateCustomer);
        group.MapGet("customers", GetCustomers);
        group.MapGet("customers/search", SearchCustomers);
        group.MapGet("customers/{id}", GetCustomerProfile);
        group.MapPut("customers/{id}", UpdateCustomer);
        group.MapDelete("customers/{id}", DeleteCustomer);
        group.MapPost("customers/{customerId}/interactions", CreateInteraction);
        group.MapGet("customers/{customerId}/interactions", GetInteractions);
    }

    public async Task<IResult> CreateCustomer(ISender sender, CreateCustomerCommand command)
    {
        var id = await sender.Send(command);
        return Results.Created($"/api/crm/customers/{id}", new { id });
    }

    public async Task<IResult> GetCustomers(ISender sender, [AsParameters] GetCustomersQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> SearchCustomers(ISender sender, string searchTerm, int? storeId)
    {
        var query = new SearchCustomersQuery
        {
            SearchTerm = searchTerm,
            StoreId = storeId
        };
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> GetCustomerProfile(ISender sender, int id)
    {
        var result = await sender.Send(new GetCustomerProfileQuery { Id = id });
        return Results.Ok(result);
    }

    public async Task<IResult> UpdateCustomer(ISender sender, int id, UpdateCustomerCommand command)
    {
        var updatedId = await sender.Send(command with { Id = id });
        return Results.Ok(new { id = updatedId });
    }

    public async Task<IResult> DeleteCustomer(ISender sender, int id)
    {
        var deleted = await sender.Send(new DeleteCustomerCommand { Id = id });
        return deleted ? Results.NoContent() : Results.NotFound();
    }

    public async Task<IResult> CreateInteraction(ISender sender, int customerId, CreateCustomerInteractionCommand command)
    {
        var id = await sender.Send(command with { CustomerId = customerId });
        return Results.Created($"/api/crm/customers/{customerId}/interactions/{id}", new { id });
    }

    public async Task<IResult> GetInteractions(ISender sender, int customerId, [AsParameters] GetCustomerInteractionsQuery query)
    {
        var result = await sender.Send(query with { CustomerId = customerId });
        return Results.Ok(result);
    }
}

