using Toss.Application.Tasks.Commands.CreateTask;
using Toss.Application.Tasks.Commands.UpdateTask;
using Toss.Application.Tasks.Commands.UpdateTaskStatus;
using Toss.Application.Tasks.Commands.LinkTask;
using Toss.Application.Tasks.Commands.DeleteTask;
using Toss.Application.Tasks.Queries.GetTasks;
using Toss.Application.Tasks.Queries.GetTaskById;
using Toss.Domain.Constants;

namespace Toss.Web.Endpoints;

public class Tasks : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.RequireAuthorization(Policies.RequireOwnerOrManager);

        // Task CRUD
        group.MapGet(string.Empty, GetTasks)
            .WithName("GetTasks");

        group.MapGet("{id}", GetTaskById)
            .WithName("GetTaskById");

        group.MapPost(string.Empty, CreateTask)
            .WithName("CreateTask");

        group.MapPut("{id}", UpdateTask)
            .WithName("UpdateTask");

        group.MapPut("{id}/status", UpdateTaskStatus)
            .WithName("UpdateTaskStatus");

        group.MapPut("{id}/link", LinkTask)
            .WithName("LinkTask");

        group.MapDelete("{id}", DeleteTask)
            .WithName("DeleteTask");
    }

    public async Task<IResult> GetTasks(ISender sender, [AsParameters] GetTasksQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> GetTaskById(ISender sender, int id)
    {
        var result = await sender.Send(new GetTaskByIdQuery { Id = id });
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    public async Task<IResult> CreateTask(ISender sender, CreateTaskCommand command)
    {
        var id = await sender.Send(command);
        return Results.Created($"/api/tasks/{id}", new { id });
    }

    public async Task<IResult> UpdateTask(ISender sender, int id, UpdateTaskCommand command)
    {
        var result = await sender.Send(command with { TaskId = id });
        return result ? Results.Ok() : Results.NotFound();
    }

    public async Task<IResult> UpdateTaskStatus(ISender sender, int id, UpdateTaskStatusCommand command)
    {
        var result = await sender.Send(command with { TaskId = id });
        return result ? Results.Ok() : Results.NotFound();
    }

    public async Task<IResult> LinkTask(ISender sender, int id, LinkTaskCommand command)
    {
        var result = await sender.Send(command with { TaskId = id });
        return result ? Results.Ok() : Results.NotFound();
    }

    public async Task<IResult> DeleteTask(ISender sender, int id)
    {
        var result = await sender.Send(new DeleteTaskCommand { TaskId = id });
        return result ? Results.Ok() : Results.NotFound();
    }
}

