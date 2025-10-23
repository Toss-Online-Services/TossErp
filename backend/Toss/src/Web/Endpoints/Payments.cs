using Toss.Application.Payments.Commands.GeneratePayLink;

namespace Toss.Web.Endpoints;

public class Payments : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.MapPost("pay-links", GeneratePayLink);
    }

    public async Task<IResult> GeneratePayLink(ISender sender, GeneratePayLinkCommand command)
    {
        var result = await sender.Send(command);
        return Results.Created($"/api/payments/pay-links/{result.Id}", result);
    }
}

