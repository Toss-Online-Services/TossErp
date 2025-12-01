using Toss.Application.Businesses.Commands.UpdateBusinessSettings;
using Toss.Application.Businesses.Queries.GetBusinessSettings;
using Toss.Domain.Constants;

namespace Toss.Web.Endpoints;

public class BusinessSettings : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.RequireAuthorization(Policies.RequireOwnerOrManager);

        group.MapGet(string.Empty, GetBusinessSettings)
            .WithName("GetBusinessSettings");

        group.MapPut(string.Empty, UpdateBusinessSettings)
            .WithName("UpdateBusinessSettings");
    }

    public async Task<IResult> GetBusinessSettings(ISender sender)
    {
        var result = await sender.Send(new GetBusinessSettingsQuery());
        return Results.Ok(result);
    }

    public async Task<IResult> UpdateBusinessSettings(ISender sender, UpdateBusinessSettingsCommand command)
    {
        var result = await sender.Send(command);
        return result ? Results.Ok() : Results.BadRequest();
    }
}

