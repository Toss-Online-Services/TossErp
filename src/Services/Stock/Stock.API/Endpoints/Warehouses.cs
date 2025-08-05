using TossErp.Stock.Application.Common.DTOs;
using TossErp.Stock.Application.Warehouses.Commands.CreateWarehouse;
using TossErp.Stock.Application.Warehouses.Commands.UpdateWarehouse;
using TossErp.Stock.Application.Warehouses.Commands.DeleteWarehouse;
using TossErp.Stock.Application.Warehouses.Queries.GetWarehouses;
using TossErp.Stock.Application.Warehouses.Queries.GetWarehouseById;
using TossErp.Stock.Application.Warehouses.Queries.GetWarehouseByCode;
using TossErp.Stock.Application.Warehouses.Queries.GetWarehouseByName;
using TossErp.Stock.Application.Warehouses.Queries.GetWarehousesByLocation;
using TossErp.Stock.Application.Warehouses.Queries.GetWarehousesByType;
using TossErp.Stock.Application.Warehouses.Queries.GetWarehousesByStatus;
using TossErp.Stock.Application.Warehouses.Queries.GetWarehousesByCapacity;
using TossErp.Stock.Application.Warehouses.Queries.GetWarehousesByUtilization;
using TossErp.Stock.Application.Warehouses.Queries.GetWarehousesByValue;
using TossErp.Stock.Application.Warehouses.Queries.GetWarehousesByItem;
using TossErp.Stock.Application.Warehouses.Queries.GetWarehousesByBin;
using TossErp.Stock.Application.Warehouses.Queries.GetWarehousesByMovement;
using TossErp.Stock.Application.Warehouses.Queries.GetWarehouseAnalytics;
using TossErp.Stock.Application.Warehouses.Queries.GetWarehouseReport;
using Microsoft.AspNetCore.Http.HttpResults;
using MediatR;

namespace TossErp.Stock.API.Endpoints;

/// <summary>
/// Warehouses endpoint group for managing warehouse operations
/// </summary>
public class Warehouses : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup("/warehouses");
        
        // Basic CRUD operations
        group.MapGet(GetWarehouses, "");
        group.MapGet(GetWarehouse, "{id}");
        group.MapPost(CreateWarehouse, "");
        group.MapPut(UpdateWarehouse, "{id}");
        group.MapDelete(DeleteWarehouse, "{id}");
            
        // Warehouse-specific operations
        group.MapGet(GetWarehouseByCode, "code/{code}");
        group.MapGet(GetWarehouseByName, "name/{name}");
        group.MapGet(GetWarehouseByLocation, "location/{location}");
        group.MapGet(GetWarehouseByType, "type/{type}");
        group.MapGet(GetWarehouseByStatus, "status/{status}");
        group.MapGet(GetWarehouseByCapacity, "capacity");
        group.MapGet(GetWarehouseByUtilization, "utilization");
        group.MapGet(GetWarehouseByValue, "value");
        group.MapGet(GetWarehouseByItems, "items/{itemId}");
        group.MapGet(GetWarehouseByBins, "bins/{binId}");
        group.MapGet(GetWarehouseByMovements, "movements");
        group.MapGet(GetWarehouseByAnalytics, "analytics");
        group.MapGet(GetWarehouseByReport, "report");
    }

    /// <summary>
    /// Get all warehouses with optional filtering and pagination
    /// </summary>
    public async Task<Results<Ok<List<WarehouseDto>>, BadRequest>> GetWarehouses(
        ISender sender, 
        [AsParameters] GetWarehousesQuery query)
    {
        try
        {
            var warehouses = await sender.Send(query);
            return TypedResults.Ok(warehouses);
        }
        catch (Exception)
        {
            return TypedResults.BadRequest();
        }
    }

    /// <summary>
    /// Get a specific warehouse by ID
    /// </summary>
    public async Task<Results<Ok<WarehouseDto>, NotFound>> GetWarehouse(
        ISender sender, 
        Guid id)
    {
        try
        {
            var warehouse = await sender.Send(new GetWarehouseByIdQuery(id));
            
            if (warehouse == null)
                return TypedResults.NotFound();
                
            return TypedResults.Ok(warehouse);
        }
        catch (Exception)
        {
            return TypedResults.NotFound();
        }
    }

    /// <summary>
    /// Create a new warehouse
    /// </summary>
    public async Task<Results<Created<WarehouseDto>, BadRequest>> CreateWarehouse(
        ISender sender, 
        CreateWarehouseCommand command)
    {
        try
        {
            var warehouse = await sender.Send(command);
            return TypedResults.Created($"/warehouses/{warehouse.Id}", warehouse);
        }
        catch (Exception)
        {
            return TypedResults.BadRequest();
        }
    }

    /// <summary>
    /// Update an existing warehouse
    /// </summary>
    public async Task<Results<NoContent, BadRequest, NotFound>> UpdateWarehouse(
        ISender sender, 
        Guid id, 
        UpdateWarehouseCommand command)
    {
        if (id != command.Id) 
            return TypedResults.BadRequest();
            
        try
        {
            await sender.Send(command);
            return TypedResults.NoContent();
        }
        catch (Exception)
        {
            return TypedResults.NotFound();
        }
    }

    /// <summary>
    /// Delete a warehouse
    /// </summary>
    public async Task<Results<NoContent, NotFound>> DeleteWarehouse(
        ISender sender, 
        Guid id)
    {
        try
        {
            await sender.Send(new DeleteWarehouseCommand(id));
            return TypedResults.NoContent();
        }
        catch (Exception)
        {
            return TypedResults.NotFound();
        }
    }

    /// <summary>
    /// Get warehouse by code
    /// </summary>
    public async Task<Results<Ok<WarehouseDto>, NotFound>> GetWarehouseByCode(
        ISender sender,
        string code)
    {
        try
        {
            var warehouse = await sender.Send(new GetWarehouseByCodeQuery(code));
            
            if (warehouse == null)
                return TypedResults.NotFound();
                
            return TypedResults.Ok(warehouse);
        }
        catch (Exception)
        {
            return TypedResults.NotFound();
        }
    }

    /// <summary>
    /// Get warehouse by name
    /// </summary>
    public async Task<Results<Ok<WarehouseDto>, NotFound>> GetWarehouseByName(
        ISender sender,
        string name)
    {
        try
        {
            var warehouse = await sender.Send(new GetWarehouseByNameQuery(name));
            
            if (warehouse == null)
                return TypedResults.NotFound();
                
            return TypedResults.Ok(warehouse);
        }
        catch (Exception)
        {
            return TypedResults.NotFound();
        }
    }

    /// <summary>
    /// Get warehouse by location
    /// </summary>
    public async Task<Results<Ok<List<WarehouseDto>>, BadRequest>> GetWarehouseByLocation(
        ISender sender,
        string location)
    {
        try
        {
            var warehouses = await sender.Send(new GetWarehousesByLocationQuery(location));
            return TypedResults.Ok(warehouses);
        }
        catch (Exception)
        {
            return TypedResults.BadRequest();
        }
    }

    /// <summary>
    /// Get warehouse by type
    /// </summary>
    public async Task<Results<Ok<List<WarehouseDto>>, BadRequest>> GetWarehouseByType(
        ISender sender,
        string type)
    {
        try
        {
            var warehouses = await sender.Send(new GetWarehousesByTypeQuery(type));
            return TypedResults.Ok(warehouses);
        }
        catch (Exception)
        {
            return TypedResults.BadRequest();
        }
    }

    /// <summary>
    /// Get warehouse by status
    /// </summary>
    public async Task<Results<Ok<List<WarehouseDto>>, BadRequest>> GetWarehouseByStatus(
        ISender sender,
        string status)
    {
        try
        {
            var warehouses = await sender.Send(new GetWarehousesByStatusQuery(status));
            return TypedResults.Ok(warehouses);
        }
        catch (Exception)
        {
            return TypedResults.BadRequest();
        }
    }

    /// <summary>
    /// Get warehouse by capacity
    /// </summary>
    public async Task<Results<Ok<List<WarehouseDto>>, BadRequest>> GetWarehouseByCapacity(
        ISender sender,
        decimal? minCapacity = null,
        decimal? maxCapacity = null)
    {
        try
        {
            var warehouses = await sender.Send(new GetWarehousesByCapacityQuery(minCapacity, maxCapacity));
            return TypedResults.Ok(warehouses);
        }
        catch (Exception)
        {
            return TypedResults.BadRequest();
        }
    }

    /// <summary>
    /// Get warehouse by utilization
    /// </summary>
    public async Task<Results<Ok<List<WarehouseDto>>, BadRequest>> GetWarehouseByUtilization(
        ISender sender,
        decimal? minUtilization = null,
        decimal? maxUtilization = null)
    {
        try
        {
            var warehouses = await sender.Send(new GetWarehousesByUtilizationQuery(minUtilization, maxUtilization));
            return TypedResults.Ok(warehouses);
        }
        catch (Exception)
        {
            return TypedResults.BadRequest();
        }
    }

    /// <summary>
    /// Get warehouse by value
    /// </summary>
    public async Task<Results<Ok<List<WarehouseDto>>, BadRequest>> GetWarehouseByValue(
        ISender sender,
        decimal? minValue = null,
        decimal? maxValue = null)
    {
        try
        {
            var warehouses = await sender.Send(new GetWarehousesByValueQuery(minValue, maxValue));
            return TypedResults.Ok(warehouses);
        }
        catch (Exception)
        {
            return TypedResults.BadRequest();
        }
    }

    /// <summary>
    /// Get warehouse by items
    /// </summary>
    public async Task<Results<Ok<List<WarehouseDto>>, BadRequest>> GetWarehouseByItems(
        ISender sender,
        Guid itemId)
    {
        try
        {
            var warehouses = await sender.Send(new GetWarehousesByItemQuery(itemId));
            return TypedResults.Ok(warehouses);
        }
        catch (Exception)
        {
            return TypedResults.BadRequest();
        }
    }

    /// <summary>
    /// Get warehouse by bins
    /// </summary>
    public async Task<Results<Ok<List<WarehouseDto>>, BadRequest>> GetWarehouseByBins(
        ISender sender,
        Guid binId)
    {
        try
        {
            var warehouses = await sender.Send(new GetWarehousesByBinQuery(binId));
            return TypedResults.Ok(warehouses);
        }
        catch (Exception)
        {
            return TypedResults.BadRequest();
        }
    }

    /// <summary>
    /// Get warehouse by movements
    /// </summary>
    public async Task<Results<Ok<List<WarehouseDto>>, BadRequest>> GetWarehouseByMovements(
        ISender sender,
        Guid movementId)
    {
        try
        {
            var warehouses = await sender.Send(new GetWarehousesByMovementQuery(movementId));
            return TypedResults.Ok(warehouses);
        }
        catch (Exception)
        {
            return TypedResults.BadRequest();
        }
    }

    /// <summary>
    /// Get warehouse analytics
    /// </summary>
    public async Task<Results<Ok<WarehouseAnalyticsDto>, BadRequest>> GetWarehouseByAnalytics(
        ISender sender,
        Guid warehouseId)
    {
        try
        {
            var analytics = await sender.Send(new GetWarehouseAnalyticsQuery(warehouseId));
            return TypedResults.Ok(analytics);
        }
        catch (Exception)
        {
            return TypedResults.BadRequest();
        }
    }

    /// <summary>
    /// Get warehouse report
    /// </summary>
    public async Task<Results<Ok<WarehouseReportDto>, BadRequest>> GetWarehouseByReport(
        ISender sender,
        Guid? warehouseId = null,
        string? reportType = null,
        DateTime? fromDate = null,
        DateTime? toDate = null)
    {
        try
        {
            var report = await sender.Send(new GetWarehouseReportQuery(warehouseId, reportType, fromDate, toDate));
            return TypedResults.Ok(report);
        }
        catch (Exception)
        {
            return TypedResults.BadRequest();
        }
    }
} 
