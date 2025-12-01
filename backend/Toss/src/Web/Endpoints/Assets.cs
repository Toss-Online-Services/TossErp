using Toss.Application.Assets.Commands.CreateAsset;
using Toss.Application.Assets.Commands.UpdateAsset;
using Toss.Application.Assets.Commands.DeleteAsset;
using Toss.Application.Assets.Commands.AddMaintenanceLog;
using Toss.Application.Assets.Queries.GetAssets;
using Toss.Application.Assets.Queries.GetAssetById;
using Toss.Application.Assets.Queries.GetAssetMaintenanceLogs;
using Toss.Domain.Constants;

namespace Toss.Web.Endpoints;

public class Assets : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.RequireAuthorization(Policies.RequireOwnerOrManager);

        // Asset CRUD
        group.MapGet(string.Empty, GetAssets)
            .WithName("GetAssets");

        group.MapGet("{id}", GetAssetById)
            .WithName("GetAssetById");

        group.MapPost(string.Empty, CreateAsset)
            .WithName("CreateAsset");

        group.MapPut("{id}", UpdateAsset)
            .WithName("UpdateAsset");

        group.MapDelete("{id}", DeleteAsset)
            .WithName("DeleteAsset");

        // Maintenance logs
        group.MapPost("{id}/maintenance", AddMaintenanceLog)
            .WithName("AddMaintenanceLog");

        group.MapGet("{id}/maintenance", GetMaintenanceLogs)
            .WithName("GetMaintenanceLogs");
    }

    public async Task<IResult> GetAssets(ISender sender, [AsParameters] GetAssetsQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> GetAssetById(ISender sender, int id)
    {
        var result = await sender.Send(new GetAssetByIdQuery(id));
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    public async Task<IResult> CreateAsset(ISender sender, CreateAssetCommand command)
    {
        var id = await sender.Send(command);
        return Results.Created($"/api/assets/{id}", new { id });
    }

    public async Task<IResult> UpdateAsset(ISender sender, int id, UpdateAssetCommand command)
    {
        await sender.Send(command with { Id = id });
        return Results.Ok();
    }

    public async Task<IResult> DeleteAsset(ISender sender, int id)
    {
        await sender.Send(new DeleteAssetCommand(id));
        return Results.Ok();
    }

    public async Task<IResult> AddMaintenanceLog(ISender sender, int id, AddMaintenanceLogCommand command)
    {
        var maintenanceLogId = await sender.Send(command with { AssetId = id });
        return Results.Created($"/api/assets/{id}/maintenance/{maintenanceLogId}", new { id = maintenanceLogId });
    }

    public async Task<IResult> GetMaintenanceLogs(ISender sender, int id, [AsParameters] GetAssetMaintenanceLogsQuery query)
    {
        var result = await sender.Send(query with { AssetId = id });
        return Results.Ok(result);
    }
}

