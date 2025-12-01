using Toss.Application.Projects.Commands.CreateProject;
using Toss.Application.Projects.Commands.UpdateProjectStatus;
using Toss.Application.Projects.Commands.AddProjectMaterial;
using Toss.Application.Projects.Commands.AddLabourEntry;
using Toss.Application.Projects.Commands.LinkProjectInvoice;
using Toss.Application.Projects.Queries.GetProjects;
using Toss.Application.Projects.Queries.GetProjectById;
using Toss.Domain.Constants;

namespace Toss.Web.Endpoints;

public class Projects : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.RequireAuthorization(Policies.RequireOwnerOrManager);

        // Project CRUD
        group.MapGet(string.Empty, GetProjects)
            .WithName("GetProjects");

        group.MapGet("{id}", GetProjectById)
            .WithName("GetProjectById");

        group.MapPost(string.Empty, CreateProject)
            .WithName("CreateProject");

        group.MapPut("{id}/status", UpdateProjectStatus)
            .WithName("UpdateProjectStatus");

        // Project materials
        group.MapPost("{id}/materials", AddProjectMaterial)
            .WithName("AddProjectMaterial");

        // Project labour
        group.MapPost("{id}/labour", AddLabourEntry)
            .WithName("AddLabourEntry");

        // Link invoice
        group.MapPut("{id}/invoice", LinkProjectInvoice)
            .WithName("LinkProjectInvoice");
    }

    public async Task<IResult> GetProjects(ISender sender, [AsParameters] GetProjectsQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> GetProjectById(ISender sender, int id)
    {
        var result = await sender.Send(new GetProjectByIdQuery { Id = id });
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    public async Task<IResult> CreateProject(ISender sender, CreateProjectCommand command)
    {
        var id = await sender.Send(command);
        return Results.Created($"/api/projects/{id}", new { id });
    }

    public async Task<IResult> UpdateProjectStatus(ISender sender, int id, UpdateProjectStatusCommand command)
    {
        var result = await sender.Send(command with { ProjectId = id });
        return result ? Results.Ok() : Results.NotFound();
    }

    public async Task<IResult> AddProjectMaterial(ISender sender, int id, AddProjectMaterialCommand command)
    {
        var materialId = await sender.Send(command with { ProjectId = id });
        return Results.Created($"/api/projects/{id}/materials/{materialId}", new { id = materialId });
    }

    public async Task<IResult> AddLabourEntry(ISender sender, int id, AddLabourEntryCommand command)
    {
        var labourId = await sender.Send(command with { ProjectId = id });
        return Results.Created($"/api/projects/{id}/labour/{labourId}", new { id = labourId });
    }

    public async Task<IResult> LinkProjectInvoice(ISender sender, int id, LinkProjectInvoiceCommand command)
    {
        var result = await sender.Send(command with { ProjectId = id });
        return result ? Results.Ok() : Results.NotFound();
    }
}

