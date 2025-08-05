// using TossErp.Stock.Application.Common.DTOs;
// using TossErp.Stock.Application.SerialNumbers.Queries.GetSerialNumbers;
// using TossErp.Stock.Application.SerialNumbers.Queries.GetSerialNumberById;
// using TossErp.Stock.Application.SerialNumbers.Commands.CreateSerialNumber;
// using TossErp.Stock.Application.SerialNumbers.Commands.UpdateSerialNumber;
// using TossErp.Stock.Application.SerialNumbers.Commands.DeleteSerialNumber;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TossErp.Stock.Application.Common.Security;
using TossErp.Stock.Domain.Constants;

namespace TossErp.Stock.API.Endpoints;

/// <summary>
/// Serial Numbers endpoint group for managing serial number tracking and traceability
/// </summary>
public class SerialNumbers : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup(nameof(SerialNumbers));

        // Temporarily disabled due to domain model alignment issues
        // TODO: Re-enable after fixing domain entity, DTO, and handler alignment
        
        // group.MapGet(GetSerialNumbers, "");
        // group.MapGet(GetSerialNumberById, "{id}");
        // group.MapPost(CreateSerialNumber, "");
        // group.MapPut(UpdateSerialNumber, "{id}");
        // group.MapDelete(DeleteSerialNumber, "{id}");
        
        // Placeholder endpoint to confirm the group is working
        group.MapGet("/ping", () => "SerialNumbers endpoint group is working");
    }

    // Temporarily commented out until domain model issues are resolved
    /*
    public static async Task<Results<Ok<PaginatedResult<SerialNoDto>>, NotFound>> GetSerialNumbers(
        [AsParameters] GetSerialNumbersQuery query,
        [FromServices] IGetSerialNumbersQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(query, cancellationToken);
        return result != null ? TypedResults.Ok(result) : TypedResults.NotFound();
    }

    public static async Task<Results<Ok<SerialNoDto>, NotFound>> GetSerialNumberById(
        Guid id,
        [FromServices] IGetSerialNumberByIdQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(new GetSerialNumberByIdQuery { Id = id }, cancellationToken);
        return result != null ? TypedResults.Ok(result) : TypedResults.NotFound();
    }

    public static async Task<Results<Created<SerialNoDto>, BadRequest>> CreateSerialNumber(
        CreateSerialNumberCommand command,
        [FromServices] ICreateSerialNumberCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(command, cancellationToken);
        return TypedResults.Created($"/api/serialnumbers/{result.Id}", result);
    }

    public static async Task<Results<Ok<SerialNoDto>, NotFound, BadRequest>> UpdateSerialNumber(
        Guid id,
        UpdateSerialNumberCommand command,
        [FromServices] IUpdateSerialNumberCommandHandler handler,
        CancellationToken cancellationToken)
    {
        command.Id = id;
        var result = await handler.Handle(command, cancellationToken);
        return result != null ? TypedResults.Ok(result) : TypedResults.NotFound();
    }

    public static async Task<Results<NoContent, NotFound>> DeleteSerialNumber(
        Guid id,
        [FromServices] IDeleteSerialNumberCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(new DeleteSerialNumberCommand { Id = id }, cancellationToken);
        return result ? TypedResults.NoContent() : TypedResults.NotFound();
    }
    */
}

// Placeholder DTOs for analytics and reports
public class SerialNumberAnalyticsDto
{
    public int TotalSerialNumbers { get; set; }
    public int ActiveSerialNumbers { get; set; }
    public int ExpiredSerialNumbers { get; set; }
    public decimal TotalValue { get; set; }
}

public class SerialNumberReportDto
{
    public string ReportType { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
    public List<object> SerialNumbers { get; set; } = new(); // Changed from SerialNoDto to object
} 
