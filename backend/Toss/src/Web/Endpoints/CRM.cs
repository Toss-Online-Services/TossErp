using Toss.Application.CRM.Commands.CreateCustomer;

namespace Toss.Web.Endpoints;

public class CRM : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.MapPost("customers", CreateCustomer);
    }

    public async Task<IResult> CreateCustomer(ISender sender, CreateCustomerCommand command)
    {
        var id = await sender.Send(command);
        return Results.Created($"/api/crm/customers/{id}", new { id });
    }
}

