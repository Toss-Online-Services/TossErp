using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TossErp.Domain.Entities.Manufacturing;
using TossErp.Infrastructure.Data;

namespace TossErp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ManufacturingController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ManufacturingController> _logger;

    public ManufacturingController(ApplicationDbContext context, ILogger<ManufacturingController> logger)
    {
        _context = context;
        _logger = logger;
    }

    #region Bill of Materials (BOM)

    /// <summary>
    /// Get all Bills of Materials
    /// </summary>
    [HttpGet("boms")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BillOfMaterials>>> GetBoms(
        [FromQuery] BomStatus? status = null,
        [FromQuery] int? productId = null)
    {
        var query = _context.BillsOfMaterials
            .Include(b => b.Items)
            .Include(b => b.Operations)
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(b => b.Status == status.Value);

        if (productId.HasValue)
            query = query.Where(b => b.ProductId == productId.Value);

        var boms = await query
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

        return Ok(boms);
    }

    /// <summary>
    /// Get BOM by ID
    /// </summary>
    [HttpGet("boms/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BillOfMaterials>> GetBom(int id)
    {
        var bom = await _context.BillsOfMaterials
            .Include(b => b.Items)
            .Include(b => b.Operations)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (bom == null)
            return NotFound(new { error = $"BOM {id} not found" });

        return Ok(bom);
    }

    /// <summary>
    /// Create new BOM
    /// </summary>
    [HttpPost("boms")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BillOfMaterials>> CreateBom([FromBody] CreateBomRequest request)
    {
        var bom = new BillOfMaterials
        {
            BomNumber = request.BomNumber,
            ProductId = request.ProductId,
            ProductName = request.ProductName,
            ProductSku = request.ProductSku,
            Type = request.Type,
            Quantity = request.Quantity,
            UnitOfMeasure = request.UnitOfMeasure,
            Description = request.Description,
            EstimatedProductionTime = request.EstimatedProductionTime,
            CreatedAt = DateTime.UtcNow
        };

        // Add items
        foreach (var itemRequest in request.Items)
        {
            bom.Items.Add(new BomItem
            {
                Sequence = itemRequest.Sequence,
                ComponentId = itemRequest.ComponentId,
                ComponentName = itemRequest.ComponentName,
                ComponentSku = itemRequest.ComponentSku,
                Quantity = itemRequest.Quantity,
                UnitOfMeasure = itemRequest.UnitOfMeasure,
                UnitCost = itemRequest.UnitCost,
                WastagePercentage = itemRequest.WastagePercentage,
                SupplyType = itemRequest.SupplyType,
                IsOptional = itemRequest.IsOptional,
                Description = itemRequest.Description,
                CreatedAt = DateTime.UtcNow
            });
        }

        // Add operations
        foreach (var opRequest in request.Operations)
        {
            bom.Operations.Add(new BomOperation
            {
                Sequence = opRequest.Sequence,
                OperationCode = opRequest.OperationCode,
                OperationName = opRequest.OperationName,
                Type = opRequest.Type,
                SetupTime = opRequest.SetupTime,
                RunTimePerUnit = opRequest.RunTimePerUnit,
                LaborRate = opRequest.LaborRate,
                MachineRate = opRequest.MachineRate,
                OverheadRate = opRequest.OverheadRate,
                Description = opRequest.Description,
                WorkInstructions = opRequest.WorkInstructions,
                CreatedAt = DateTime.UtcNow
            });
        }

        // Calculate costs
        bom.CalculateCosts();

        _context.BillsOfMaterials.Add(bom);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created BOM {BomNumber} for product {ProductName}", bom.BomNumber, bom.ProductName);

        return CreatedAtAction(nameof(GetBom), new { id = bom.Id }, bom);
    }

    /// <summary>
    /// Approve BOM
    /// </summary>
    [HttpPost("boms/{id}/approve")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BillOfMaterials>> ApproveBom(int id, [FromBody] ApproveBomRequest request)
    {
        var bom = await _context.BillsOfMaterials.FindAsync(id);
        if (bom == null)
            return NotFound(new { error = $"BOM {id} not found" });

        try
        {
            bom.Approve(request.ApprovedBy, request.ApprovedByName);
            await _context.SaveChangesAsync();

            _logger.LogInformation("BOM {BomNumber} approved by {ApprovedByName}", bom.BomNumber, request.ApprovedByName);

            return Ok(bom);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Activate BOM
    /// </summary>
    [HttpPost("boms/{id}/activate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BillOfMaterials>> ActivateBom(int id)
    {
        var bom = await _context.BillsOfMaterials.FindAsync(id);
        if (bom == null)
            return NotFound(new { error = $"BOM {id} not found" });

        try
        {
            bom.Activate();
            await _context.SaveChangesAsync();

            _logger.LogInformation("BOM {BomNumber} activated", bom.BomNumber);

            return Ok(bom);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    #endregion

    #region Work Orders

    /// <summary>
    /// Get all work orders
    /// </summary>
    [HttpGet("work-orders")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<WorkOrder>>> GetWorkOrders(
        [FromQuery] WorkOrderStatus? status = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var query = _context.WorkOrders
            .Include(w => w.Operations)
            .Include(w => w.Materials)
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(w => w.Status == status.Value);

        if (startDate.HasValue)
            query = query.Where(w => w.PlannedStartDate >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(w => w.PlannedEndDate <= endDate.Value);

        var workOrders = await query
            .OrderByDescending(w => w.Priority)
            .ThenBy(w => w.PlannedStartDate)
            .ToListAsync();

        return Ok(workOrders);
    }

    /// <summary>
    /// Get work order by ID
    /// </summary>
    [HttpGet("work-orders/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkOrder>> GetWorkOrder(int id)
    {
        var workOrder = await _context.WorkOrders
            .Include(w => w.Operations)
            .Include(w => w.Materials)
            .Include(w => w.ProductionEntries)
            .FirstOrDefaultAsync(w => w.Id == id);

        if (workOrder == null)
            return NotFound(new { error = $"Work order {id} not found" });

        return Ok(workOrder);
    }

    /// <summary>
    /// Create new work order
    /// </summary>
    [HttpPost("work-orders")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<WorkOrder>> CreateWorkOrder([FromBody] CreateWorkOrderRequest request)
    {
        // Get BOM to copy items and operations
        BillOfMaterials? bom = null;
        if (request.BomId.HasValue)
        {
            bom = await _context.BillsOfMaterials
                .Include(b => b.Items)
                .Include(b => b.Operations)
                .FirstOrDefaultAsync(b => b.Id == request.BomId.Value);

            if (bom == null)
                return BadRequest(new { error = $"BOM {request.BomId} not found" });

            if (bom.Status != BomStatus.Active)
                return BadRequest(new { error = "BOM must be active to create work order" });
        }

        var workOrder = new WorkOrder
        {
            WorkOrderNumber = request.WorkOrderNumber,
            Type = request.Type,
            ProductId = request.ProductId,
            ProductName = request.ProductName,
            ProductSku = request.ProductSku,
            BomId = request.BomId,
            BomNumber = bom?.BomNumber,
            QuantityOrdered = request.QuantityOrdered,
            UnitOfMeasure = request.UnitOfMeasure,
            PlannedStartDate = request.PlannedStartDate,
            PlannedEndDate = request.PlannedEndDate,
            Priority = request.Priority,
            WarehouseId = request.WarehouseId,
            WarehouseName = request.WarehouseName,
            Description = request.Description,
            SpecialInstructions = request.SpecialInstructions,
            CreatedAt = DateTime.UtcNow
        };

        // Copy materials from BOM
        if (bom != null)
        {
            foreach (var bomItem in bom.Items)
            {
                workOrder.Materials.Add(new WorkOrderMaterial
                {
                    Sequence = bomItem.Sequence,
                    ComponentId = bomItem.ComponentId,
                    ComponentName = bomItem.ComponentName,
                    ComponentSku = bomItem.ComponentSku,
                    QuantityRequired = bomItem.EffectiveQuantity * request.QuantityOrdered,
                    UnitOfMeasure = bomItem.UnitOfMeasure,
                    UnitCost = bomItem.UnitCost,
                    BomItemId = bomItem.Id,
                    CreatedAt = DateTime.UtcNow
                });
            }

            // Copy operations from BOM
            foreach (var bomOp in bom.Operations)
            {
                var workOrderOp = new WorkOrderOperation
                {
                    Sequence = bomOp.Sequence,
                    OperationCode = bomOp.OperationCode,
                    OperationName = bomOp.OperationName,
                    WorkCenterId = bomOp.WorkCenterId,
                    WorkCenterName = bomOp.WorkCenterName,
                    MachineId = bomOp.MachineId,
                    MachineName = bomOp.MachineName,
                    EstimatedSetupTime = bomOp.SetupTime,
                    EstimatedRunTime = bomOp.RunTimePerUnit * request.QuantityOrdered,
                    WorkInstructions = bomOp.WorkInstructions,
                    RequiresInspection = bomOp.RequiresQualityCheck,
                    CreatedAt = DateTime.UtcNow
                };
                
                bomOp.CalculateCosts(request.QuantityOrdered);
                workOrderOp.EstimatedCost = bomOp.TotalCost;
                
                workOrder.Operations.Add(workOrderOp);
            }

            // Set estimated costs from BOM
            workOrder.EstimatedMaterialCost = bom.MaterialCost * request.QuantityOrdered;
            workOrder.EstimatedLaborCost = bom.LaborCost * request.QuantityOrdered;
            workOrder.EstimatedOverheadCost = bom.OverheadCost * request.QuantityOrdered;
            workOrder.EstimatedTotalCost = bom.TotalCost * request.QuantityOrdered;
        }

        _context.WorkOrders.Add(workOrder);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created work order {WorkOrderNumber} for {Quantity} units of {ProductName}",
            workOrder.WorkOrderNumber, workOrder.QuantityOrdered, workOrder.ProductName);

        return CreatedAtAction(nameof(GetWorkOrder), new { id = workOrder.Id }, workOrder);
    }

    /// <summary>
    /// Release work order to shop floor
    /// </summary>
    [HttpPost("work-orders/{id}/release")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkOrder>> ReleaseWorkOrder(int id)
    {
        var workOrder = await _context.WorkOrders.FindAsync(id);
        if (workOrder == null)
            return NotFound(new { error = $"Work order {id} not found" });

        try
        {
            workOrder.Release();
            await _context.SaveChangesAsync();

            _logger.LogInformation("Work order {WorkOrderNumber} released to shop floor", workOrder.WorkOrderNumber);

            return Ok(workOrder);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Start work order production
    /// </summary>
    [HttpPost("work-orders/{id}/start")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkOrder>> StartWorkOrder(int id)
    {
        var workOrder = await _context.WorkOrders.FindAsync(id);
        if (workOrder == null)
            return NotFound(new { error = $"Work order {id} not found" });

        try
        {
            workOrder.Start();
            await _context.SaveChangesAsync();

            _logger.LogInformation("Work order {WorkOrderNumber} started production", workOrder.WorkOrderNumber);

            return Ok(workOrder);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Complete work order
    /// </summary>
    [HttpPost("work-orders/{id}/complete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkOrder>> CompleteWorkOrder(int id)
    {
        var workOrder = await _context.WorkOrders.FindAsync(id);
        if (workOrder == null)
            return NotFound(new { error = $"Work order {id} not found" });

        try
        {
            workOrder.Complete();
            await _context.SaveChangesAsync();

            _logger.LogInformation("Work order {WorkOrderNumber} completed. Produced {Quantity} units",
                workOrder.WorkOrderNumber, workOrder.QuantityProduced);

            return Ok(workOrder);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Cancel work order
    /// </summary>
    [HttpPost("work-orders/{id}/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkOrder>> CancelWorkOrder(int id, [FromBody] CancelWorkOrderRequest request)
    {
        var workOrder = await _context.WorkOrders.FindAsync(id);
        if (workOrder == null)
            return NotFound(new { error = $"Work order {id} not found" });

        try
        {
            workOrder.Cancel(request.Reason);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Work order {WorkOrderNumber} cancelled: {Reason}",
                workOrder.WorkOrderNumber, request.Reason);

            return Ok(workOrder);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    #endregion

    #region Production Entries

    /// <summary>
    /// Record production output
    /// </summary>
    [HttpPost("production-entries")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductionEntry>> CreateProductionEntry([FromBody] CreateProductionEntryRequest request)
    {
        var workOrder = await _context.WorkOrders.FindAsync(request.WorkOrderId);
        if (workOrder == null)
            return BadRequest(new { error = $"Work order {request.WorkOrderId} not found" });

        if (workOrder.Status != WorkOrderStatus.InProgress)
            return BadRequest(new { error = "Work order must be in progress to record production" });

        var entry = new ProductionEntry
        {
            WorkOrderId = request.WorkOrderId,
            EntryNumber = request.EntryNumber,
            QuantityProduced = request.QuantityProduced,
            QuantityRejected = request.QuantityRejected,
            QuantityScrapped = request.QuantityScrapped,
            UnitOfMeasure = request.UnitOfMeasure,
            ProductionDate = request.ProductionDate,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            ActualHours = request.ActualHours,
            WorkCenterId = request.WorkCenterId,
            WorkCenterName = request.WorkCenterName,
            OperatorId = request.OperatorId,
            OperatorName = request.OperatorName,
            SupervisorId = request.SupervisorId,
            SupervisorName = request.SupervisorName,
            DefectCodes = request.DefectCodes,
            DefectDescription = request.DefectDescription,
            MaterialCost = request.MaterialCost,
            LaborCost = request.LaborCost,
            OverheadCost = request.OverheadCost,
            TotalCost = request.MaterialCost + request.LaborCost + request.OverheadCost,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow
        };

        // Update work order quantities
        workOrder.QuantityProduced += entry.QuantityProduced;
        workOrder.QuantityRejected += entry.QuantityRejected;
        workOrder.ActualMaterialCost += entry.MaterialCost;
        workOrder.ActualLaborCost += entry.LaborCost;
        workOrder.ActualOverheadCost += entry.OverheadCost;
        workOrder.ActualTotalCost += entry.TotalCost;

        _context.ProductionEntries.Add(entry);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Recorded production entry {EntryNumber} for work order {WorkOrderNumber}: {Quantity} units produced",
            entry.EntryNumber, workOrder.WorkOrderNumber, entry.QuantityProduced);

        return CreatedAtAction(nameof(GetProductionEntry), new { id = entry.Id }, entry);
    }

    /// <summary>
    /// Get production entry by ID
    /// </summary>
    [HttpGet("production-entries/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductionEntry>> GetProductionEntry(int id)
    {
        var entry = await _context.ProductionEntries
            .Include(p => p.WorkOrder)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (entry == null)
            return NotFound(new { error = $"Production entry {id} not found" });

        return Ok(entry);
    }

    /// <summary>
    /// Get production summary
    /// </summary>
    [HttpGet("production/summary")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetProductionSummary(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var start = startDate ?? DateTime.Today;
        var end = endDate ?? DateTime.Today.AddDays(1);

        var workOrders = await _context.WorkOrders
            .Where(w => w.ActualStartDate >= start && w.ActualStartDate < end)
            .ToListAsync();

        var productionEntries = await _context.ProductionEntries
            .Where(p => p.ProductionDate >= start && p.ProductionDate < end)
            .ToListAsync();

        var summary = new
        {
            period = new { startDate = start, endDate = end },
            workOrders = new
            {
                total = workOrders.Count,
                inProgress = workOrders.Count(w => w.Status == WorkOrderStatus.InProgress),
                completed = workOrders.Count(w => w.Status == WorkOrderStatus.Completed),
                onHold = workOrders.Count(w => w.Status == WorkOrderStatus.OnHold)
            },
            production = new
            {
                totalEntries = productionEntries.Count,
                totalProduced = productionEntries.Sum(p => p.QuantityProduced),
                totalRejected = productionEntries.Sum(p => p.QuantityRejected),
                totalScrapped = productionEntries.Sum(p => p.QuantityScrapped),
                totalMaterialCost = productionEntries.Sum(p => p.MaterialCost),
                totalLaborCost = productionEntries.Sum(p => p.LaborCost),
                totalOverheadCost = productionEntries.Sum(p => p.OverheadCost),
                totalCost = productionEntries.Sum(p => p.TotalCost)
            },
            efficiency = productionEntries.Any()
                ? new
                {
                    qualityRate = (productionEntries.Sum(p => p.QuantityProduced) /
                                  (productionEntries.Sum(p => p.QuantityProduced) + productionEntries.Sum(p => p.QuantityRejected))) * 100,
                    scrapRate = (productionEntries.Sum(p => p.QuantityScrapped) /
                                (productionEntries.Sum(p => p.QuantityProduced) + productionEntries.Sum(p => p.QuantityRejected) + productionEntries.Sum(p => p.QuantityScrapped))) * 100
                }
                : null
        };

        return Ok(summary);
    }

    #endregion
}

// Request DTOs
public record CreateBomRequest(
    string BomNumber,
    int ProductId,
    string ProductName,
    string? ProductSku,
    BomType Type,
    decimal Quantity,
    string? UnitOfMeasure,
    string? Description,
    decimal EstimatedProductionTime,
    List<BomItemRequest> Items,
    List<BomOperationRequest> Operations
);

public record BomItemRequest(
    int Sequence,
    int ComponentId,
    string ComponentName,
    string? ComponentSku,
    decimal Quantity,
    string? UnitOfMeasure,
    decimal UnitCost,
    decimal? WastagePercentage,
    ItemSupplyType SupplyType,
    bool IsOptional,
    string? Description
);

public record BomOperationRequest(
    int Sequence,
    string OperationCode,
    string OperationName,
    OperationType Type,
    decimal SetupTime,
    decimal RunTimePerUnit,
    decimal LaborRate,
    decimal MachineRate,
    decimal OverheadRate,
    string? Description,
    string? WorkInstructions
);

public record ApproveBomRequest(int ApprovedBy, string ApprovedByName);

public record CreateWorkOrderRequest(
    string WorkOrderNumber,
    WorkOrderType Type,
    int ProductId,
    string ProductName,
    string? ProductSku,
    int? BomId,
    decimal QuantityOrdered,
    string? UnitOfMeasure,
    DateTime PlannedStartDate,
    DateTime PlannedEndDate,
    int Priority,
    int? WarehouseId,
    string? WarehouseName,
    string? Description,
    string? SpecialInstructions
);

public record CancelWorkOrderRequest(string Reason);

public record CreateProductionEntryRequest(
    int WorkOrderId,
    string EntryNumber,
    decimal QuantityProduced,
    decimal QuantityRejected,
    decimal QuantityScrapped,
    string? UnitOfMeasure,
    DateTime ProductionDate,
    DateTime? StartTime,
    DateTime? EndTime,
    decimal? ActualHours,
    int? WorkCenterId,
    string? WorkCenterName,
    int? OperatorId,
    string? OperatorName,
    int? SupervisorId,
    string? SupervisorName,
    string? DefectCodes,
    string? DefectDescription,
    decimal MaterialCost,
    decimal LaborCost,
    decimal OverheadCost,
    string? Notes
);
