using Toss.Application.Quality.Commands.CreateChecklist;
using Toss.Application.Quality.Commands.RecordChecklistRun;
using Toss.Application.Quality.Commands.LogIncident;
using Toss.Application.Quality.Commands.AssignActionItem;
using Toss.Application.Quality.Commands.UpdateActionItemStatus;
using Toss.Application.Quality.Queries.GetChecklists;
using Toss.Application.Quality.Queries.GetChecklistById;
using Toss.Application.Quality.Queries.GetChecklistRuns;
using Toss.Application.Quality.Queries.GetIncidents;
using Toss.Application.Quality.Queries.GetActionItems;
using Toss.Domain.Constants;

namespace Toss.Web.Endpoints;

public class Quality : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.RequireAuthorization(Policies.RequireOwnerOrManager);

        // Checklists
        group.MapGet("checklists", GetChecklists)
            .WithName("GetChecklists");

        group.MapGet("checklists/{id}", GetChecklistById)
            .WithName("GetChecklistById");

        group.MapPost("checklists", CreateChecklist)
            .WithName("CreateChecklist");

        // Checklist Runs
        group.MapPost("checklists/runs", RecordChecklistRun)
            .WithName("RecordChecklistRun");

        group.MapGet("checklists/runs", GetChecklistRuns)
            .WithName("GetChecklistRuns");

        // Incidents
        group.MapPost("incidents", LogIncident)
            .WithName("LogIncident");

        group.MapGet("incidents", GetIncidents)
            .WithName("GetIncidents");

        // Action Items
        group.MapPost("action-items", AssignActionItem)
            .WithName("AssignActionItem");

        group.MapGet("action-items", GetActionItems)
            .WithName("GetActionItems");

        group.MapPut("action-items/{id}/status", UpdateActionItemStatus)
            .WithName("UpdateActionItemStatus");
    }

    public async Task<IResult> GetChecklists(ISender sender, [AsParameters] GetChecklistsQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> GetChecklistById(ISender sender, int id)
    {
        var result = await sender.Send(new GetChecklistByIdQuery { Id = id });
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    public async Task<IResult> CreateChecklist(ISender sender, CreateChecklistCommand command)
    {
        var id = await sender.Send(command);
        return Results.Created($"/api/quality/checklists/{id}", new { id });
    }

    public async Task<IResult> RecordChecklistRun(ISender sender, RecordChecklistRunCommand command)
    {
        var id = await sender.Send(command);
        return Results.Created($"/api/quality/checklists/runs/{id}", new { id });
    }

    public async Task<IResult> GetChecklistRuns(ISender sender, [AsParameters] GetChecklistRunsQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> LogIncident(ISender sender, LogIncidentCommand command)
    {
        var id = await sender.Send(command);
        return Results.Created($"/api/quality/incidents/{id}", new { id });
    }

    public async Task<IResult> GetIncidents(ISender sender, [AsParameters] GetIncidentsQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> AssignActionItem(ISender sender, AssignActionItemCommand command)
    {
        var id = await sender.Send(command);
        return Results.Created($"/api/quality/action-items/{id}", new { id });
    }

    public async Task<IResult> GetActionItems(ISender sender, [AsParameters] GetActionItemsQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> UpdateActionItemStatus(ISender sender, int id, UpdateActionItemStatusCommand command)
    {
        var result = await sender.Send(command with { ActionItemId = id });
        return result ? Results.Ok() : Results.NotFound();
    }
}

