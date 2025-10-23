using Toss.Application.Payments.Commands.GeneratePayLink;
using Toss.Application.Payments.Commands.RecordPayment;
using Toss.Application.Payments.Queries.GetPayments;

namespace Toss.Web.Endpoints;

public class Payments : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.MapPost("pay-links", GeneratePayLink);
        group.MapPost("record", RecordPayment);
        group.MapGet(string.Empty, GetPayments);
    }

    public async Task<IResult> GeneratePayLink(ISender sender, GeneratePayLinkCommand command)
    {
        var result = await sender.Send(command);
        return Results.Created($"/api/payments/pay-links/{result.Id}", result);
    }

    public async Task<IResult> RecordPayment(ISender sender, RecordPaymentCommand command)
    {
        var id = await sender.Send(command);
        return Results.Created($"/api/payments/{id}", new { id });
    }

    public async Task<IResult> GetPayments(ISender sender, [AsParameters] GetPaymentsQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }
}

