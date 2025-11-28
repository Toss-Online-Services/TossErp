using Toss.Application.CRM.Commands.CreateCustomer;
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
}

