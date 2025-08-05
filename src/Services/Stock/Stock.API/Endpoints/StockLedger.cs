using TossErp.Stock.Application.Common.DTOs;
using TossErp.Stock.Application.Common.Security;
using TossErp.Stock.Domain.Constants;
using TossErp.Stock.Domain.Common;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace TossErp.Stock.API.Endpoints;

/// <summary>
/// Stock Ledger endpoint group for managing stock ledger entries and audit trail
/// </summary>
public class StockLedger : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup(nameof(StockLedger));

        group.MapGet("", GetStockLedgerEntries)
            .WithName("GetStockLedgerEntries")
            .WithSummary("Get all stock ledger entries")
            .WithDescription("Retrieves paginated stock ledger entries");

        group.MapGet("{id}", GetStockLedgerEntryById)
            .WithName("GetStockLedgerEntryById")
            .WithSummary("Get stock ledger entry by ID")
            .WithDescription("Retrieves a specific stock ledger entry by its ID");

        group.MapGet("item/{itemId}", GetStockLedgerEntriesByItem)
            .WithName("GetStockLedgerEntriesByItem")
            .WithSummary("Get stock ledger entries by item")
            .WithDescription("Retrieves all stock ledger entries for a specific item");
    }

    public static async Task<Results<Ok<List<StockLedgerEntryDto>>, NotFound>> GetStockLedgerEntries(
        [FromServices] IStockLedgerEntryRepository repository,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var entries = await repository.GetAllAsync(cancellationToken);
        
        var pagedEntries = entries
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(sle => new StockLedgerEntryDto
            {
                Id = sle.Id,
                VoucherType = sle.VoucherType,
                VoucherNo = sle.VoucherNo,
                PostingDate = sle.PostingDate,
                PostingTime = sle.PostingTime,
                ItemCode = sle.ItemCode,
                ItemName = sle.Item?.ItemName ?? string.Empty,
                Warehouse = sle.WarehouseCode,
                Bin = sle.BinName,
                BatchNo = sle.BatchNo ?? string.Empty,
                SerialNo = sle.SerialNo ?? string.Empty,
                UOM = sle.StockUOM ?? string.Empty,
                Qty = sle.Qty.Value,
                BalanceQty = sle.Qty.Value, // Simplified - should calculate running balance
                IncomingRate = sle.ValuationRate.Value,
                ValuationRate = sle.ValuationRate.Value,
                BalanceValue = sle.StockValue.Value,
                StockValue = sle.StockValue.Value,
                Company = sle.Company ?? string.Empty,
                FiscalYear = sle.FiscalYear ?? string.Empty,
                Project = sle.Project ?? string.Empty,
                CostCenter = sle.CostCenter ?? string.Empty,
                Remarks = sle.Remarks ?? string.Empty,
                IsCancelled = sle.IsCancelled,
                Created = DateTime.UtcNow, // Simplified
                CreatedBy = "System"
            })
            .ToList();

        return TypedResults.Ok(pagedEntries);
    }

    public static async Task<Results<Ok<StockLedgerEntryDto>, NotFound>> GetStockLedgerEntryById(
        Guid id,
        [FromServices] IStockLedgerEntryRepository repository,
        CancellationToken cancellationToken = default)
    {
        var entry = await repository.GetByIdAsync(id, cancellationToken);
        
        if (entry == null)
            return TypedResults.NotFound();

        var dto = new StockLedgerEntryDto
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
            BalanceQty = entry.Qty.Value,
            IncomingRate = entry.ValuationRate.Value,
            ValuationRate = entry.ValuationRate.Value,
            BalanceValue = entry.StockValue.Value,
            StockValue = entry.StockValue.Value,
            Company = entry.Company ?? string.Empty,
            FiscalYear = entry.FiscalYear ?? string.Empty,
            Project = entry.Project ?? string.Empty,
            CostCenter = entry.CostCenter ?? string.Empty,
            Remarks = entry.Remarks ?? string.Empty,
            IsCancelled = entry.IsCancelled,
            Created = DateTime.UtcNow,
            CreatedBy = "System"
        };

        return TypedResults.Ok(dto);
    }

    public static async Task<Results<Ok<List<StockLedgerEntryDto>>, NotFound>> GetStockLedgerEntriesByItem(
        Guid itemId,
        [FromServices] IStockLedgerEntryRepository repository,
        [FromServices] IItemRepository itemRepository,
        CancellationToken cancellationToken = default)
    {
        var item = await itemRepository.GetByIdAsync(itemId, cancellationToken);
        if (item == null)
            return TypedResults.NotFound();

        var entries = await repository.GetByItemAsync(item.ItemCode, cancellationToken);
        
        var dtos = entries.Select(sle => new StockLedgerEntryDto
        {
            Id = sle.Id,
            VoucherType = sle.VoucherType,
            VoucherNo = sle.VoucherNo,
            PostingDate = sle.PostingDate,
            PostingTime = sle.PostingTime,
            ItemCode = sle.ItemCode,
            ItemName = sle.Item?.ItemName ?? string.Empty,
            Warehouse = sle.WarehouseCode,
            Bin = sle.BinName,
            BatchNo = sle.BatchNo ?? string.Empty,
            SerialNo = sle.SerialNo ?? string.Empty,
            UOM = sle.StockUOM ?? string.Empty,
            Qty = sle.Qty.Value,
            BalanceQty = sle.Qty.Value,
            IncomingRate = sle.ValuationRate.Value,
            ValuationRate = sle.ValuationRate.Value,
            BalanceValue = sle.StockValue.Value,
            StockValue = sle.StockValue.Value,
            Company = sle.Company ?? string.Empty,
            FiscalYear = sle.FiscalYear ?? string.Empty,
            Project = sle.Project ?? string.Empty,
            CostCenter = sle.CostCenter ?? string.Empty,
            Remarks = sle.Remarks ?? string.Empty,
            IsCancelled = sle.IsCancelled,
            Created = DateTime.UtcNow,
            CreatedBy = "System"
        }).ToList();

        return TypedResults.Ok(dtos);
    }
} 
