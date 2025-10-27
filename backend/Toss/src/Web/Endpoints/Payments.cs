using Toss.Application.Payments.Commands.GeneratePayLink;
using Toss.Application.Payments.Commands.GeneratePaymentQR;
using Toss.Application.Payments.Commands.InitiateAirtelPayment;
using Toss.Application.Payments.Commands.InitiateMpesaPayment;
using Toss.Application.Payments.Commands.InitiateMtnPayment;
using Toss.Application.Payments.Commands.RecordPayment;
using Toss.Application.Payments.Queries.GetPaymentById;
using Toss.Application.Payments.Queries.GetPayments;
using Toss.Application.Payments.Queries.GetPaymentStatus;

namespace Toss.Web.Endpoints;

public class Payments : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.MapPost("pay-links", GeneratePayLink);
        group.MapPost("record", RecordPayment);
        group.MapGet(string.Empty, GetPayments);
        group.MapGet("{id}", GetPaymentById);

        // Mobile Money Providers
        group.MapPost("mpesa/initiate", InitiateMpesaPayment);
        group.MapPost("airtel/initiate", InitiateAirtelPayment);
        group.MapPost("mtn/initiate", InitiateMtnPayment);
        group.MapGet("{provider}/status/{transactionId}", GetPaymentStatus);

        // QR Code Generation
        group.MapPost("qr/generate", GeneratePaymentQR);
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

    public async Task<IResult> GetPaymentById(ISender sender, int id)
    {
        var result = await sender.Send(new GetPaymentByIdQuery { Id = id });
        return Results.Ok(result);
    }

    public async Task<IResult> InitiateMpesaPayment(ISender sender, InitiateMpesaPaymentCommand command)
    {
        var result = await sender.Send(command);
        return Results.Ok(result);
    }

    public async Task<IResult> InitiateAirtelPayment(ISender sender, InitiateAirtelPaymentCommand command)
    {
        var result = await sender.Send(command);
        return Results.Ok(result);
    }

    public async Task<IResult> InitiateMtnPayment(ISender sender, InitiateMtnPaymentCommand command)
    {
        var result = await sender.Send(command);
        return Results.Ok(result);
    }

    public async Task<IResult> GetPaymentStatus(ISender sender, string provider, string transactionId)
    {
        var query = new GetPaymentStatusQuery
        {
            Provider = provider,
            TransactionId = transactionId
        };
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> GeneratePaymentQR(ISender sender, GeneratePaymentQRCommand command)
    {
        var result = await sender.Send(command);
        return Results.Ok(result);
    }
}

