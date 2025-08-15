using TossErp.Stock.Application.Common.DTOs;
using TossErp.Stock.Application.Common.Security;
using TossErp.Stock.Domain.Constants;
using TossErp.Stock.Domain.Common;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace TossErp.Stock.API.Endpoints;

/// <summary>
/// Reports endpoint group for comprehensive stock reporting and analytics
/// </summary>
public class Reports : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup(this);

        // Stock Ledger Reports
        group.MapGet(GetStockLedger, "ledger");
        group.MapGet(GetStockLedgerByItem, "ledger/item/{itemId}");
        group.MapGet(GetStockLedgerByWarehouse, "ledger/warehouse/{warehouseId}");

        // Stock Valuation Reports
        group.MapGet(GetStockValuation, "valuation");
        group.MapGet(GetStockValuationByWarehouse, "valuation/warehouse/{warehouseId}");
        group.MapGet(GetStockValuationByCategory, "valuation/category/{category}");

        // Movement Summary Reports
        group.MapGet(GetMovementSummary, "movement-summary");
        group.MapGet(GetMovementSummaryByType, "movement-summary/{movementType}");

        // Business-Specific Reports
        group.MapGet(GetLowStockReport, "low-stock");
        group.MapGet(GetOutOfStockReport, "out-of-stock");
        group.MapGet(GetSlowMovingReport, "slow-moving");
        group.MapGet(GetFastMovingReport, "fast-moving");

        // Aging Reports
        group.MapGet(GetStockAgingReport, "aging");
        group.MapGet(GetStockAgingByWarehouse, "aging/warehouse/{warehouseId}");

        // Performance Reports
        group.MapGet(GetStockTurnoverReport, "turnover");
        group.MapGet(GetStockAccuracyReport, "accuracy");

        // Export Reports
        group.MapGet(ExportStockLedger, "export/ledger");
        group.MapGet(ExportStockValuation, "export/valuation");
        group.MapGet(ExportMovementSummary, "export/movement-summary");
    }

    /// <summary>
    /// Get stock ledger report with filtering and pagination
    /// </summary>
    public static async Task<Results<Ok<StockLedgerReportDto>, BadRequest>> GetStockLedger(
        [FromServices] IStockLedgerEntryRepository repository,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] string? itemCode = null,
        [FromQuery] string? warehouseCode = null,
        [FromQuery] string? movementType = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var entries = await repository.GetAllAsync(cancellationToken);
            
            // Apply filters
            if (fromDate.HasValue)
                entries = entries.Where(e => e.PostingDate >= fromDate.Value);
            
            if (toDate.HasValue)
                entries = entries.Where(e => e.PostingDate <= toDate.Value);
            
            if (!string.IsNullOrWhiteSpace(itemCode))
                entries = entries.Where(e => e.ItemCode == itemCode);
            
            if (!string.IsNullOrWhiteSpace(warehouseCode))
                entries = entries.Where(e => e.WarehouseCode == warehouseCode);
            
            if (!string.IsNullOrWhiteSpace(movementType))
                entries = entries.Where(e => e.VoucherType == movementType);

            // Calculate running balance
            var orderedEntries = entries.OrderBy(e => e.PostingDate).ThenBy(e => e.PostingTime);
            decimal runningBalance = 0;
            var ledgerEntries = new List<StockLedgerEntryDto>();

            foreach (var entry in orderedEntries)
            {
                runningBalance += entry.Qty.Value;
                ledgerEntries.Add(new StockLedgerEntryDto
                {
                    Id = entry.Id,
                    VoucherType = entry.VoucherType,
                    VoucherNo = entry.VoucherNo,
                    PostingDate = entry.PostingDate,
                    PostingTime = entry.PostingTime,
                    ItemCode = entry.ItemCode,
                    ItemName = entry.Item?.ItemName ?? string.Empty,
                    Warehouse = entry.WarehouseCode,
                    Bin = entry.BinName,
                    BatchNo = entry.BatchNo ?? string.Empty,
                    SerialNo = entry.SerialNo ?? string.Empty,
                    UOM = entry.StockUOM ?? string.Empty,
                    Qty = entry.Qty.Value,
                    BalanceQty = runningBalance,
                    IncomingRate = entry.ValuationRate.Value,
                    ValuationRate = entry.ValuationRate.Value,
                    BalanceValue = runningBalance * entry.ValuationRate.Value,
                    StockValue = entry.StockValue.Value,
                    Company = entry.Company ?? string.Empty,
                    FiscalYear = entry.FiscalYear ?? string.Empty,
                    Project = entry.Project ?? string.Empty,
                    CostCenter = entry.CostCenter ?? string.Empty,
                    Remarks = entry.Remarks ?? string.Empty,
                    IsCancelled = entry.IsCancelled,
                    Created = entry.PostingDate,
                    CreatedBy = string.Empty
                });
            }

            // Apply pagination
            var pagedEntries = ledgerEntries
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var report = new StockLedgerReportDto
            {
                FromDate = fromDate,
                ToDate = toDate,
                ItemCode = itemCode,
                WarehouseCode = warehouseCode,
                MovementType = movementType,
                TotalEntries = ledgerEntries.Count,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)ledgerEntries.Count / pageSize),
                Entries = pagedEntries,
                Summary = new StockLedgerSummaryDto
                {
                    TotalIncomingQty = ledgerEntries.Where(e => e.Qty > 0).Sum(e => e.Qty),
                    TotalOutgoingQty = Math.Abs(ledgerEntries.Where(e => e.Qty < 0).Sum(e => e.Qty)),
                    NetMovement = ledgerEntries.Sum(e => e.Qty),
                    TotalValue = ledgerEntries.Sum(e => e.StockValue),
                    AverageValue = ledgerEntries.Any() ? ledgerEntries.Average(e => e.StockValue) : 0
                }
            };

            return TypedResults.Ok(report);
        }
        catch (Exception)
        {
            return TypedResults.BadRequest();
        }
    }

    /// <summary>
    /// Get stock ledger for a specific item
    /// </summary>
    public static async Task<Results<Ok<StockLedgerReportDto>, NotFound, BadRequest>> GetStockLedgerByItem(
        Guid itemId,
        [FromServices] IStockLedgerEntryRepository repository,
        [FromServices] IItemRepository itemRepository,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var item = await itemRepository.GetByIdAsync(itemId, cancellationToken);
            if (item == null)
                return TypedResults.NotFound();

            var entries = await repository.GetByItemAsync(item.ItemCode, cancellationToken);
            
            // Apply date filters
            if (fromDate.HasValue)
                entries = entries.Where(e => e.PostingDate >= fromDate.Value);
            
            if (toDate.HasValue)
                entries = entries.Where(e => e.PostingDate <= toDate.Value);

            // Calculate running balance
            var orderedEntries = entries.OrderBy(e => e.PostingDate).ThenBy(e => e.PostingTime);
            decimal runningBalance = 0;
            var ledgerEntries = new List<StockLedgerEntryDto>();

            foreach (var entry in orderedEntries)
            {
                runningBalance += entry.Qty.Value;
                ledgerEntries.Add(new StockLedgerEntryDto
                {
                    Id = entry.Id,
                    VoucherType = entry.VoucherType,
                    VoucherNo = entry.VoucherNo,
                    PostingDate = entry.PostingDate,
                    PostingTime = entry.PostingTime,
                    ItemCode = entry.ItemCode,
                    ItemName = item.ItemName,
                    Warehouse = entry.WarehouseCode,
                    Bin = entry.BinName,
                    BatchNo = entry.BatchNo ?? string.Empty,
                    SerialNo = entry.SerialNo ?? string.Empty,
                    UOM = entry.StockUOM ?? string.Empty,
                    Qty = entry.Qty.Value,
                    BalanceQty = runningBalance,
                    IncomingRate = entry.ValuationRate.Value,
                    ValuationRate = entry.ValuationRate.Value,
                    BalanceValue = runningBalance * entry.ValuationRate.Value,
                    StockValue = entry.StockValue.Value,
                    Company = entry.Company ?? string.Empty,
                    FiscalYear = entry.FiscalYear ?? string.Empty,
                    Project = entry.Project ?? string.Empty,
                    CostCenter = entry.CostCenter ?? string.Empty,
                    Remarks = entry.Remarks ?? string.Empty,
                    IsCancelled = entry.IsCancelled,
                    Created = entry.PostingDate,
                    CreatedBy = string.Empty
                });
            }

            var report = new StockLedgerReportDto
            {
                FromDate = fromDate,
                ToDate = toDate,
                ItemCode = item.ItemCode,
                TotalEntries = ledgerEntries.Count,
                Entries = ledgerEntries,
                Summary = new StockLedgerSummaryDto
                {
                    TotalIncomingQty = ledgerEntries.Where(e => e.Qty > 0).Sum(e => e.Qty),
                    TotalOutgoingQty = Math.Abs(ledgerEntries.Where(e => e.Qty < 0).Sum(e => e.Qty)),
                    NetMovement = ledgerEntries.Sum(e => e.Qty),
                    TotalValue = ledgerEntries.Sum(e => e.StockValue),
                    AverageValue = ledgerEntries.Any() ? ledgerEntries.Average(e => e.StockValue) : 0
                }
            };

            return TypedResults.Ok(report);
        }
        catch (Exception)
        {
            return TypedResults.BadRequest();
        }
    }

    /// <summary>
    /// Get stock ledger for a specific warehouse
    /// </summary>
    public static async Task<Results<Ok<StockLedgerReportDto>, NotFound, BadRequest>> GetStockLedgerByWarehouse(
        Guid warehouseId,
        [FromServices] IStockLedgerEntryRepository repository,
        [FromServices] IWarehouseRepository warehouseRepository,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var warehouse = await warehouseRepository.GetByIdAsync(warehouseId, cancellationToken);
            if (warehouse == null)
                return TypedResults.NotFound();

            var entries = await repository.GetAllAsync(cancellationToken);
            entries = entries.Where(e => e.WarehouseCode == warehouse.Code);
            
            // Apply date filters
            if (fromDate.HasValue)
                entries = entries.Where(e => e.PostingDate >= fromDate.Value);
            
            if (toDate.HasValue)
                entries = entries.Where(e => e.PostingDate <= toDate.Value);

            // Calculate running balance
            var orderedEntries = entries.OrderBy(e => e.PostingDate).ThenBy(e => e.PostingTime);
            decimal runningBalance = 0;
            var ledgerEntries = new List<StockLedgerEntryDto>();

            foreach (var entry in orderedEntries)
            {
                runningBalance += entry.Qty.Value;
                ledgerEntries.Add(new StockLedgerEntryDto
                {
                    Id = entry.Id,
                    VoucherType = entry.VoucherType,
                    VoucherNo = entry.VoucherNo,
                    PostingDate = entry.PostingDate,
                    PostingTime = entry.PostingTime,
                    ItemCode = entry.ItemCode,
                    ItemName = entry.Item?.ItemName ?? string.Empty,
                    Warehouse = entry.WarehouseCode,
                    Bin = entry.BinName,
                    BatchNo = entry.BatchNo ?? string.Empty,
                    SerialNo = entry.SerialNo ?? string.Empty,
                    UOM = entry.StockUOM ?? string.Empty,
                    Qty = entry.Qty.Value,
                    BalanceQty = runningBalance,
                    IncomingRate = entry.ValuationRate.Value,
                    ValuationRate = entry.ValuationRate.Value,
                    BalanceValue = runningBalance * entry.ValuationRate.Value,
                    StockValue = entry.StockValue.Value,
                    Company = entry.Company ?? string.Empty,
                    FiscalYear = entry.FiscalYear ?? string.Empty,
                    Project = entry.Project ?? string.Empty,
                    CostCenter = entry.CostCenter ?? string.Empty,
                    Remarks = entry.Remarks ?? string.Empty,
                    IsCancelled = entry.IsCancelled,
                    Created = entry.PostingDate,
                    CreatedBy = string.Empty
                });
            }

            var report = new StockLedgerReportDto
            {
                FromDate = fromDate,
                ToDate = toDate,
                WarehouseCode = warehouse.Code,
                TotalEntries = ledgerEntries.Count,
                Entries = ledgerEntries,
                Summary = new StockLedgerSummaryDto
                {
                    TotalIncomingQty = ledgerEntries.Where(e => e.Qty > 0).Sum(e => e.Qty),
                    TotalOutgoingQty = Math.Abs(ledgerEntries.Where(e => e.Qty < 0).Sum(e => e.Qty)),
                    NetMovement = ledgerEntries.Sum(e => e.Qty),
                    TotalValue = ledgerEntries.Sum(e => e.StockValue),
                    AverageValue = ledgerEntries.Any() ? ledgerEntries.Average(e => e.StockValue) : 0
                }
            };

            return TypedResults.Ok(report);
        }
        catch (Exception)
        {
            return TypedResults.BadRequest();
        }
    }

    /// <summary>
    /// Get current stock valuation report
    /// </summary>
    public static async Task<Results<Ok<StockValuationReportDto>, BadRequest>> GetStockValuation(
        [FromServices] IStockLevelRepository stockLevelRepository,
        [FromServices] IItemRepository itemRepository,
        [FromQuery] string? warehouseCode = null,
        [FromQuery] string? category = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var stockLevels = await stockLevelRepository.GetAllAsync(cancellationToken);
            var items = await itemRepository.GetAllAsync(cancellationToken);

            // Apply filters
            if (!string.IsNullOrWhiteSpace(warehouseCode))
            {
                // Filter by warehouse code (assuming stock levels have warehouse info)
                // This would need to be implemented based on your actual data model
            }

            var valuationItems = new List<StockValuationItemDto>();
            decimal totalValue = 0;
            int totalItems = 0;

            foreach (var stockLevel in stockLevels)
            {
                var item = items.FirstOrDefault(i => i.Id == stockLevel.ItemId);
                if (item == null) continue;

                                 if (!string.IsNullOrWhiteSpace(category) && item.ItemGroup != category)
                     continue;

                 var itemValue = stockLevel.Quantity * stockLevel.UnitCost;
                 totalValue += itemValue;
                 totalItems++;

                 valuationItems.Add(new StockValuationItemDto
                 {
                     ItemId = item.Id,
                     ItemCode = item.ItemCode,
                     ItemName = item.ItemName,
                     Category = item.ItemGroup,
                     WarehouseId = stockLevel.WarehouseId,
                     Quantity = stockLevel.Quantity,
                     UnitCost = stockLevel.UnitCost,
                     TotalValue = itemValue,
                     LastUpdated = stockLevel.LastUpdated
                 });
            }

            var report = new StockValuationReportDto
            {
                GeneratedDate = DateTime.UtcNow,
                WarehouseCode = warehouseCode,
                Category = category,
                TotalItems = totalItems,
                TotalValue = totalValue,
                AverageValue = totalItems > 0 ? totalValue / totalItems : 0,
                Items = valuationItems,
                Summary = new StockValuationSummaryDto
                {
                    TotalQuantity = valuationItems.Sum(i => i.Quantity),
                    AverageUnitCost = valuationItems.Any() ? valuationItems.Average(i => i.UnitCost) : 0,
                    HighestValueItem = valuationItems.OrderByDescending(i => i.TotalValue).FirstOrDefault(),
                    LowestValueItem = valuationItems.OrderBy(i => i.TotalValue).FirstOrDefault()
                }
            };

            return TypedResults.Ok(report);
        }
        catch (Exception)
        {
            return TypedResults.BadRequest();
        }
    }

    /// <summary>
    /// Get stock valuation for a specific warehouse
    /// </summary>
    public static async Task<Results<Ok<StockValuationReportDto>, NotFound, BadRequest>> GetStockValuationByWarehouse(
        Guid warehouseId,
        [FromServices] IStockLevelRepository stockLevelRepository,
        [FromServices] IItemRepository itemRepository,
        [FromServices] IWarehouseRepository warehouseRepository,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var warehouse = await warehouseRepository.GetByIdAsync(warehouseId, cancellationToken);
            if (warehouse == null)
                return TypedResults.NotFound();

            var stockLevels = await stockLevelRepository.GetAllAsync(cancellationToken);
            var items = await itemRepository.GetAllAsync(cancellationToken);

            // Filter stock levels by warehouse (assuming stock levels have warehouse info)
            // This would need to be implemented based on your actual data model
            var warehouseStockLevels = stockLevels; // Placeholder - implement actual filtering

            var valuationItems = new List<StockValuationItemDto>();
            decimal totalValue = 0;

            foreach (var stockLevel in warehouseStockLevels)
            {
                                 var item = items.FirstOrDefault(i => i.Id == stockLevel.ItemId);
                 if (item == null) continue;

                 var itemValue = stockLevel.Quantity * stockLevel.UnitCost;
                 totalValue += itemValue;

                 valuationItems.Add(new StockValuationItemDto
                 {
                     ItemId = item.Id,
                     ItemCode = item.ItemCode,
                     ItemName = item.ItemName,
                     Category = item.ItemGroup,
                     WarehouseId = stockLevel.WarehouseId,
                     Quantity = stockLevel.Quantity,
                     UnitCost = stockLevel.UnitCost,
                     TotalValue = itemValue,
                     LastUpdated = stockLevel.LastUpdated
                 });
            }

            var report = new StockValuationReportDto
            {
                GeneratedDate = DateTime.UtcNow,
                WarehouseCode = warehouse.Code,
                WarehouseName = warehouse.Name,
                TotalItems = valuationItems.Count,
                TotalValue = totalValue,
                AverageValue = valuationItems.Any() ? totalValue / valuationItems.Count : 0,
                Items = valuationItems,
                Summary = new StockValuationSummaryDto
                {
                    TotalQuantity = valuationItems.Sum(i => i.Quantity),
                    AverageUnitCost = valuationItems.Any() ? valuationItems.Average(i => i.UnitCost) : 0,
                    HighestValueItem = valuationItems.OrderByDescending(i => i.TotalValue).FirstOrDefault(),
                    LowestValueItem = valuationItems.OrderBy(i => i.TotalValue).FirstOrDefault()
                }
            };

            return TypedResults.Ok(report);
        }
        catch (Exception)
        {
            return TypedResults.BadRequest();
        }
    }

    /// <summary>
    /// Get stock valuation by category
    /// </summary>
    public static async Task<Results<Ok<StockValuationReportDto>, BadRequest>> GetStockValuationByCategory(
        string category,
        [FromServices] IStockLevelRepository stockLevelRepository,
        [FromServices] IItemRepository itemRepository,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var stockLevels = await stockLevelRepository.GetAllAsync(cancellationToken);
            var items = await itemRepository.GetAllAsync(cancellationToken);

                         // Filter items by category
             var categoryItems = items.Where(i => i.ItemGroup == category).ToList();
            var categoryItemIds = categoryItems.Select(i => i.Id).ToHashSet();

            var valuationItems = new List<StockValuationItemDto>();
            decimal totalValue = 0;

            foreach (var stockLevel in stockLevels)
            {
                if (!categoryItemIds.Contains(stockLevel.ItemId)) continue;

                                 var item = categoryItems.First(i => i.Id == stockLevel.ItemId);
                 var itemValue = stockLevel.Quantity * stockLevel.UnitCost;
                 totalValue += itemValue;

                 valuationItems.Add(new StockValuationItemDto
                 {
                     ItemId = item.Id,
                     ItemCode = item.ItemCode,
                     ItemName = item.ItemName,
                     Category = item.ItemGroup,
                     WarehouseId = stockLevel.WarehouseId,
                     Quantity = stockLevel.Quantity,
                     UnitCost = stockLevel.UnitCost,
                     TotalValue = itemValue,
                     LastUpdated = stockLevel.LastUpdated
                 });
            }

            var report = new StockValuationReportDto
            {
                GeneratedDate = DateTime.UtcNow,
                Category = category,
                TotalItems = valuationItems.Count,
                TotalValue = totalValue,
                AverageValue = valuationItems.Any() ? totalValue / valuationItems.Count : 0,
                Items = valuationItems,
                Summary = new StockValuationSummaryDto
                {
                    TotalQuantity = valuationItems.Sum(i => i.Quantity),
                    AverageUnitCost = valuationItems.Any() ? valuationItems.Average(i => i.UnitCost) : 0,
                    HighestValueItem = valuationItems.OrderByDescending(i => i.TotalValue).FirstOrDefault(),
                    LowestValueItem = valuationItems.OrderBy(i => i.TotalValue).FirstOrDefault()
                }
            };

            return TypedResults.Ok(report);
        }
        catch (Exception)
        {
            return TypedResults.BadRequest();
        }
    }

    /// <summary>
    /// Get stock movement summary report
    /// </summary>
    public static async Task<Results<Ok<MovementSummaryReportDto>, BadRequest>> GetMovementSummary(
        [FromServices] IStockMovementRepository movementRepository,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] string? movementType = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var movements = await movementRepository.GetAllAsync(cancellationToken);
            
                         // Apply filters
             if (fromDate.HasValue)
                 movements = movements.Where(m => m.CreatedAt >= fromDate.Value);
             
             if (toDate.HasValue)
                 movements = movements.Where(m => m.CreatedAt <= toDate.Value);
            
            if (!string.IsNullOrWhiteSpace(movementType))
                movements = movements.Where(m => m.MovementType.ToString() == movementType);

            var summary = new MovementSummaryReportDto
            {
                FromDate = fromDate,
                ToDate = toDate,
                MovementType = movementType,
                TotalMovements = movements.Count(),
                TotalQuantity = movements.Sum(m => m.Quantity),
                TotalValue = movements.Sum(m => m.Quantity * (m.UnitCost ?? 0)),
                MovementBreakdown = movements
                    .GroupBy(m => m.MovementType)
                    .Select(g => new MovementTypeSummaryDto
                    {
                        MovementType = g.Key.ToString(),
                        Count = g.Count(),
                        TotalQuantity = g.Sum(m => m.Quantity),
                        TotalValue = g.Sum(m => m.Quantity * (m.UnitCost ?? 0))
                    })
                    .ToList()
            };

            return TypedResults.Ok(summary);
        }
        catch (Exception)
        {
            return TypedResults.BadRequest();
        }
    }

    /// <summary>
    /// Get stock movement summary by type
    /// </summary>
    public static async Task<Results<Ok<MovementSummaryReportDto>, BadRequest>> GetMovementSummaryByType(
        string movementType,
        [FromServices] IStockMovementRepository movementRepository,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var movements = await movementRepository.GetAllAsync(cancellationToken);
            
            // Filter by movement type
            movements = movements.Where(m => m.MovementType.ToString() == movementType);
            
            // Apply date filters
            if (fromDate.HasValue)
                movements = movements.Where(m => m.CreatedAt >= fromDate.Value);
            
            if (toDate.HasValue)
                movements = movements.Where(m => m.CreatedAt <= toDate.Value);

            var summary = new MovementSummaryReportDto
            {
                FromDate = fromDate,
                ToDate = toDate,
                MovementType = movementType,
                TotalMovements = movements.Count(),
                TotalQuantity = movements.Sum(m => m.Quantity),
                TotalValue = movements.Sum(m => m.Quantity * (m.UnitCost ?? 0)),
                MovementBreakdown = new List<MovementTypeSummaryDto>
                {
                    new MovementTypeSummaryDto
                    {
                        MovementType = movementType,
                        Count = movements.Count(),
                        TotalQuantity = movements.Sum(m => m.Quantity),
                        TotalValue = movements.Sum(m => m.Quantity * (m.UnitCost ?? 0))
                    }
                }
            };

            return TypedResults.Ok(summary);
        }
        catch (Exception)
        {
            return TypedResults.BadRequest();
        }
    }

    /// <summary>
    /// Get low stock report
    /// </summary>
    public static async Task<Results<Ok<LowStockReportDto>, BadRequest>> GetLowStockReport(
        [FromServices] IStockLevelRepository stockLevelRepository,
        [FromServices] IItemRepository itemRepository,
        [FromQuery] decimal? threshold = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var stockLevels = await stockLevelRepository.GetAllAsync(cancellationToken);
            var items = await itemRepository.GetAllAsync(cancellationToken);

            var lowStockItems = new List<LowStockItemDto>();
            var defaultThreshold = threshold ?? 10; // Default threshold of 10 units

            foreach (var stockLevel in stockLevels)
            {
                if (stockLevel.Quantity <= defaultThreshold)
                {
                    var item = items.FirstOrDefault(i => i.Id == stockLevel.ItemId);
                    if (item == null) continue;

                    lowStockItems.Add(new LowStockItemDto
                    {
                        ItemId = item.Id,
                        ItemCode = item.ItemCode,
                        ItemName = item.ItemName,
                        Category = item.ItemGroup,
                        WarehouseId = stockLevel.WarehouseId,
                        CurrentQuantity = stockLevel.Quantity,
                        Threshold = defaultThreshold,
                        ReorderQuantity = item.ReOrderLevel ?? 50, // Default reorder quantity
                        LastUpdated = stockLevel.LastUpdated
                    });
                }
            }

            var report = new LowStockReportDto
            {
                GeneratedDate = DateTime.UtcNow,
                Threshold = defaultThreshold,
                TotalItems = lowStockItems.Count,
                TotalValue = lowStockItems.Sum(i => i.CurrentQuantity * i.UnitCost),
                Items = lowStockItems.OrderBy(i => i.CurrentQuantity).ToList()
            };

            return TypedResults.Ok(report);
        }
        catch (Exception)
        {
            return TypedResults.BadRequest();
        }
    }

    /// <summary>
    /// Get out of stock report
    /// </summary>
    public static async Task<Results<Ok<OutOfStockReportDto>, BadRequest>> GetOutOfStockReport(
        [FromServices] IStockLevelRepository stockLevelRepository,
        [FromServices] IItemRepository itemRepository,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var stockLevels = await stockLevelRepository.GetAllAsync(cancellationToken);
            var items = await itemRepository.GetAllAsync(cancellationToken);

            var outOfStockItems = new List<OutOfStockItemDto>();

            foreach (var stockLevel in stockLevels)
            {
                if (stockLevel.Quantity <= 0)
                {
                    var item = items.FirstOrDefault(i => i.Id == stockLevel.ItemId);
                    if (item == null) continue;

                    outOfStockItems.Add(new OutOfStockItemDto
                    {
                        ItemId = item.Id,
                        ItemCode = item.ItemCode,
                        ItemName = item.ItemName,
                        Category = item.ItemGroup,
                        WarehouseId = stockLevel.WarehouseId,
                        LastQuantity = stockLevel.Quantity,
                        LastMovementDate = stockLevel.LastUpdated,
                        ReorderQuantity = item.ReOrderLevel ?? 50
                    });
                }
            }

            var report = new OutOfStockReportDto
            {
                GeneratedDate = DateTime.UtcNow,
                TotalItems = outOfStockItems.Count,
                Items = outOfStockItems.OrderBy(i => i.LastMovementDate).ToList()
            };

            return TypedResults.Ok(report);
        }
        catch (Exception)
        {
            return TypedResults.BadRequest();
        }
    }

    // Placeholder methods for other reports
    public static async Task<Results<Ok<string>, BadRequest>> GetSlowMovingReport(
        [FromServices] IStockMovementRepository movementRepository,
        [FromQuery] int days = 90,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await Task.CompletedTask; // Placeholder
            return TypedResults.Ok("Slow moving report - to be implemented");
        }
        catch (Exception)
        {
            return TypedResults.BadRequest();
        }
    }

    public static async Task<Results<Ok<string>, BadRequest>> GetFastMovingReport(
        [FromServices] IStockMovementRepository movementRepository,
        [FromQuery] int days = 30,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await Task.CompletedTask; // Placeholder
            return TypedResults.Ok("Fast moving report - to be implemented");
        }
        catch (Exception)
        {
            return TypedResults.BadRequest();
        }
    }

    public static async Task<Results<Ok<string>, BadRequest>> GetStockAgingReport(
        [FromServices] IStockLevelRepository stockLevelRepository,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await Task.CompletedTask; // Placeholder
            return TypedResults.Ok("Stock aging report - to be implemented");
        }
        catch (Exception)
        {
            return TypedResults.BadRequest();
        }
    }

    public static async Task<Results<Ok<string>, BadRequest>> GetStockAgingByWarehouse(
        Guid warehouseId,
        [FromServices] IStockLevelRepository stockLevelRepository,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await Task.CompletedTask; // Placeholder
            return TypedResults.Ok("Stock aging by warehouse report - to be implemented");
        }
        catch (Exception)
        {
            return TypedResults.BadRequest();
        }
    }

    public static async Task<Results<Ok<string>, BadRequest>> GetStockTurnoverReport(
        [FromServices] IStockMovementRepository movementRepository,
        [FromQuery] int period = 365,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await Task.CompletedTask; // Placeholder
            return TypedResults.Ok("Stock turnover report - to be implemented");
        }
        catch (Exception)
        {
            return TypedResults.BadRequest();
        }
    }

    public static async Task<Results<Ok<string>, BadRequest>> GetStockAccuracyReport(
        [FromServices] IStockLevelRepository stockLevelRepository,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await Task.CompletedTask; // Placeholder
            return TypedResults.Ok("Stock accuracy report - to be implemented");
        }
        catch (Exception)
        {
            return TypedResults.BadRequest();
        }
    }

    // Export methods
    public static async Task<Results<Ok<string>, BadRequest>> ExportStockLedger(
        [FromServices] IStockLedgerEntryRepository repository,
        [FromQuery] string format = "excel",
        CancellationToken cancellationToken = default)
    {
        try
        {
            await Task.CompletedTask; // Placeholder
            return TypedResults.Ok("Export functionality to be implemented");
        }
        catch (Exception)
        {
            return TypedResults.BadRequest();
        }
    }

    public static async Task<Results<Ok<string>, BadRequest>> ExportStockValuation(
        [FromServices] IStockLevelRepository stockLevelRepository,
        [FromQuery] string format = "excel",
        CancellationToken cancellationToken = default)
    {
        try
        {
            await Task.CompletedTask; // Placeholder
            return TypedResults.Ok("Export functionality to be implemented");
        }
        catch (Exception)
        {
            return TypedResults.BadRequest();
        }
    }

    public static async Task<Results<Ok<string>, BadRequest>> ExportMovementSummary(
        [FromServices] IStockMovementRepository movementRepository,
        [FromQuery] string format = "excel",
        CancellationToken cancellationToken = default)
    {
        try
        {
            await Task.CompletedTask; // Placeholder
            return TypedResults.Ok("Export functionality to be implemented");
        }
        catch (Exception)
        {
            return TypedResults.BadRequest();
        }
    }
}

// DTOs for Reports
public class StockLedgerReportDto
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? ItemCode { get; set; }
    public string? WarehouseCode { get; set; }
    public string? MovementType { get; set; }
    public int TotalEntries { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public List<StockLedgerEntryDto> Entries { get; set; } = new();
    public StockLedgerSummaryDto Summary { get; set; } = new();
}

public class StockLedgerSummaryDto
{
    public decimal TotalIncomingQty { get; set; }
    public decimal TotalOutgoingQty { get; set; }
    public decimal NetMovement { get; set; }
    public decimal TotalValue { get; set; }
    public decimal AverageValue { get; set; }
}

public class StockValuationReportDto
{
    public DateTime GeneratedDate { get; set; }
    public string? WarehouseCode { get; set; }
    public string? WarehouseName { get; set; }
    public string? Category { get; set; }
    public int TotalItems { get; set; }
    public decimal TotalValue { get; set; }
    public decimal AverageValue { get; set; }
    public List<StockValuationItemDto> Items { get; set; } = new();
    public StockValuationSummaryDto Summary { get; set; } = new();
}

public class StockValuationItemDto
{
    public Guid ItemId { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public Guid WarehouseId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalValue { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class StockValuationSummaryDto
{
    public decimal TotalQuantity { get; set; }
    public decimal AverageUnitCost { get; set; }
    public StockValuationItemDto? HighestValueItem { get; set; }
    public StockValuationItemDto? LowestValueItem { get; set; }
}

public class MovementSummaryReportDto
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? MovementType { get; set; }
    public int TotalMovements { get; set; }
    public decimal TotalQuantity { get; set; }
    public decimal TotalValue { get; set; }
    public List<MovementTypeSummaryDto> MovementBreakdown { get; set; } = new();
}

public class MovementTypeSummaryDto
{
    public string MovementType { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal TotalQuantity { get; set; }
    public decimal TotalValue { get; set; }
}

public class LowStockReportDto
{
    public DateTime GeneratedDate { get; set; }
    public decimal Threshold { get; set; }
    public int TotalItems { get; set; }
    public decimal TotalValue { get; set; }
    public List<LowStockItemDto> Items { get; set; } = new();
}

public class LowStockItemDto
{
    public Guid ItemId { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public Guid WarehouseId { get; set; }
    public decimal CurrentQuantity { get; set; }
    public decimal Threshold { get; set; }
    public decimal ReorderQuantity { get; set; }
    public decimal UnitCost { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class OutOfStockReportDto
{
    public DateTime GeneratedDate { get; set; }
    public int TotalItems { get; set; }
    public List<OutOfStockItemDto> Items { get; set; } = new();
}

public class OutOfStockItemDto
{
    public Guid ItemId { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public Guid WarehouseId { get; set; }
    public decimal LastQuantity { get; set; }
    public DateTime LastMovementDate { get; set; }
    public decimal ReorderQuantity { get; set; }
}
