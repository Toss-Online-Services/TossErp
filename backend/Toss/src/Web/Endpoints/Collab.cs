using Toss.Application.Collaborations.Commands.SubmitFeedback;
using Toss.Application.Collaborations.Commands.SubmitOffer;
using Microsoft.AspNetCore.RateLimiting;

namespace Toss.Web.Endpoints;

public class Collab : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        // Public endpoints - no authorization required, but rate limited
        group.MapPost("feedback", SubmitFeedback)
            .WithName("SubmitFeedback")
            .AllowAnonymous()
            .RequireRateLimiting("PublicLimiter")
            .DisableAntiforgery(); // Public endpoint, no antiforgery needed

        group.MapPost("offer", SubmitOffer)
            .WithName("SubmitOffer")
            .AllowAnonymous()
            .RequireRateLimiting("PublicLimiter")
            .DisableAntiforgery(); // Public endpoint, no antiforgery needed
    }

    public async Task<IResult> SubmitFeedback(ISender sender, SubmitFeedbackCommand command)
    {
        try
        {
            var id = await sender.Send(command);
            return Results.Created($"/api/collab/feedback/{id}", new { id });
        }
        catch (Toss.Application.Common.Exceptions.ForbiddenAccessException)
        {
            return Results.Forbid();
        }
        catch (Toss.Application.Common.Exceptions.ValidationException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    public async Task<IResult> SubmitOffer(ISender sender, SubmitOfferCommand command)
    {
        try
        {
            var id = await sender.Send(command);
            return Results.Created($"/api/collab/offer/{id}", new { id });
        }
        catch (Toss.Application.Common.Exceptions.ForbiddenAccessException)
        {
            return Results.Forbid();
        }
        catch (Toss.Application.Common.Exceptions.ValidationException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }
}

