using Toss.Application.Settings.Commands.UpdateShopSettings;
using Toss.Application.Settings.Queries.GetShopSettings;
using Toss.Domain.Constants;

namespace Toss.Web.Endpoints;

public class Settings : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.RequireAuthorization(Policies.RequireOwnerOrManager);
        group.MapGet("shop/{shopId}", GetShopSettings);
        group.MapPut("shop/{shopId}", UpdateShopSettings);
    }

    public async Task<IResult> GetShopSettings(ISender sender, int shopId)
    {
        var result = await sender.Send(new GetShopSettingsQuery { ShopId = shopId });
        return Results.Ok(result);
    }

    public async Task<IResult> UpdateShopSettings(ISender sender, int shopId, UpdateShopSettingsCommand command)
    {
        var result = await sender.Send(command with { ShopId = shopId });
        return result ? Results.Ok() : Results.BadRequest("Settings update failed");
    }
}

