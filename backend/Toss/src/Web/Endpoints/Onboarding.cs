using Toss.Application.Onboarding.Commands.CompleteOnboarding;
using Toss.Application.Onboarding.Commands.UpdateOnboardingStatus;
using Toss.Application.Onboarding.Queries.GetOnboardingStatus;

namespace Toss.Web.Endpoints;

public class Onboarding : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.MapGet("status", GetOnboardingStatus)
            .WithName("GetOnboardingStatus")
            .RequireAuthorization();

        group.MapPost("update", UpdateOnboardingStatus)
            .WithName("UpdateOnboardingStatus")
            .RequireAuthorization();

        group.MapPost("complete", CompleteOnboarding)
            .WithName("CompleteOnboarding")
            .RequireAuthorization();
    }

    public async Task<IResult> GetOnboardingStatus(
        ISender sender,
        string userId,
        string role)
    {
        var query = new GetOnboardingStatusQuery
        {
            UserId = userId,
            Role = role
        };
        var result = await sender.Send(query);
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    public async Task<IResult> UpdateOnboardingStatus(
        ISender sender,
        UpdateOnboardingStatusCommand command)
    {
        var id = await sender.Send(command);
        return Results.Ok(new { id });
    }

    public async Task<IResult> CompleteOnboarding(
        ISender sender,
        CompleteOnboardingCommand command)
    {
        var result = await sender.Send(command);
        return result ? Results.Ok(new { message = "Onboarding completed successfully" })
                     : Results.BadRequest(new { message = "Failed to complete onboarding" });
    }
}

