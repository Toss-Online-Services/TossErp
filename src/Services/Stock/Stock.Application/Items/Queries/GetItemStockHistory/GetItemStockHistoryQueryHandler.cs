using System;
using MediatR;
using TossErp.Stock.Application.Common.DTOs;
using TossErp.Stock.Application.Common.Interfaces;
using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Aggregates.ItemAggregate;
using Microsoft.Extensions.Logging;

namespace TossErp.Stock.Application.Items.Queries.GetItemStockHistory;

/// <summary>
/// Handler for retrieving item stock history
/// </summary>
public class GetItemStockHistoryQueryHandler : IRequestHandler<GetItemStockHistoryQuery, List<StockLedgerEntryDto>>
{
    private readonly IItemRepository _itemRepository;
    private readonly IStockLedgerEntryRepository _stockLedgerRepository;
    private readonly ILogger<GetItemStockHistoryQueryHandler> _logger;

    public GetItemStockHistoryQueryHandler(
        IItemRepository itemRepository,
        IStockLedgerEntryRepository stockLedgerRepository,
        ILogger<GetItemStockHistoryQueryHandler> logger)
    {
        _itemRepository = itemRepository;
        _stockLedgerRepository = stockLedgerRepository;
        _logger = logger;
    }

    public async Task<List<StockLedgerEntryDto>> Handle(GetItemStockHistoryQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting stock history for item {ItemId}", request.ItemId);

        var item = await _itemRepository.GetByIdAsync(request.ItemId, cancellationToken);
        if (item == null)
        {
            _logger.LogWarning("Item with ID {ItemId} not found", request.ItemId);
            return new List<StockLedgerEntryDto>();
        }

        // Get all stock ledger entries for this item
        var stockLedgerEntries = await _stockLedgerRepository.GetByItemAsync(item.ItemCode, cancellationToken);
        
        // Map to DTOs and sort by posting date/time (most recent first)
        var result = stockLedgerEntries
            .OrderByDescending(sle => sle.PostingDate)
            .ThenByDescending(sle => sle.PostingTime)
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
                BalanceQty = CalculateRunningBalance(stockLedgerEntries, sle),
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
                Created = DateTime.UtcNow, // Since StockLedgerEntry doesn't have audit properties
                CreatedBy = "System",
                LastModified = null,
                LastModifiedBy = null
            })
            .ToList();

        _logger.LogInformation("Retrieved {Count} stock history entries for item {ItemId}", result.Count, request.ItemId);
        return result;
    }

    private decimal CalculateRunningBalance(IEnumerable<Domain.Entities.StockLedgerEntry> allEntries, Domain.Entities.StockLedgerEntry currentEntry)
    {
        // Calculate running balance up to current entry
        return allEntries
            .Where(sle => sle.PostingDate < currentEntry.PostingDate || 
                         (sle.PostingDate == currentEntry.PostingDate && sle.PostingTime <= currentEntry.PostingTime))
            .Sum(sle => sle.Qty.Value);
    }
}
