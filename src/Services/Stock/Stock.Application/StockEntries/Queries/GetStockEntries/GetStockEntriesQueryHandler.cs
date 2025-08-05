using MediatR;
using TossErp.Stock.Application.Common.DTOs;
using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Aggregates.StockEntryAggregate;
using Microsoft.Extensions.Logging;

namespace TossErp.Stock.Application.StockEntries.Queries.GetStockEntries;

public class GetStockEntriesQueryHandler : IRequestHandler<GetStockEntriesQuery, List<StockEntryDto>>
{
    private readonly IRepository<StockEntryAggregate> _repository;

    public GetStockEntriesQueryHandler(IRepository<StockEntryAggregate> repository)
    {
        _repository = repository;
    }

    public async Task<List<StockEntryDto>> Handle(GetStockEntriesQuery request, CancellationToken cancellationToken)
    {
        var stockEntries = await _repository.GetAllAsync(cancellationToken);
        return stockEntries.Select(entry => new StockEntryDto
        {
            Id = entry.Id,
            StockEntryNo = entry.EntryNumber.Value,
            StockEntryType = "Stock Entry", // Default type
            Purpose = "Stock Movement", // Default purpose
            PostingDate = entry.EntryDate.Date,
            PostingTime = entry.EntryDate,
            Company = entry.Company,
            SourceWarehouse = "", // Not available in aggregate
            TargetWarehouse = "", // Not available in aggregate
            SourceCostCenter = "", // Not available in aggregate
            TargetCostCenter = "", // Not available in aggregate
            Project = "", // Not available in aggregate
            ReferenceNo = entry.Reference ?? "",
            ReferenceType = "", // Not available in aggregate
            Status = entry.IsPosted ? "Posted" : "Draft",
            Remarks = entry.Notes ?? "",
            TotalAmount = entry.GetTotalValue(),
            TotalQty = entry.GetTotalQuantity(),
            IsOpening = false, // Not available in aggregate
            IsRepostItemValuation = false, // Not available in aggregate
            IsRepostItemValuationAllowed = false, // Not available in aggregate
            IsRepostItemValuationDone = false, // Not available in aggregate
            Created = DateTime.UtcNow, // Use current time since aggregate doesn't expose Created
            CreatedBy = "", // Use empty string since aggregate doesn't expose CreatedBy
            LastModified = DateTime.UtcNow, // Use current time since aggregate doesn't expose LastModified
            LastModifiedBy = "", // Use empty string since aggregate doesn't expose LastModifiedBy
            Details = entry.Details.Select(d => new StockEntryDetailDto
            {
                Id = d.Id,
                StockEntryId = d.StockEntryId,
                ItemCode = "", // Need to get from item
                ItemName = "", // Need to get from item
                Description = "",
                UOM = "",
                Qty = d.Quantity.Value,
                TransferQty = 0,
                ConsumedQty = 0,
                BasicRate = d.Rate.Value,
                BasicAmount = d.GetTotalValue(),
                AdditionalCost = 0,
                Rate = d.Rate.Value,
                Amount = d.GetTotalValue(),
                SourceWarehouse = "",
                TargetWarehouse = "",
                SourceBin = "",
                TargetBin = "",
                BatchNo = d.BatchNo ?? "",
                SerialNo = d.SerialNo ?? "",
                CostCenter = "",
                Project = "",
                Remarks = d.Remarks ?? "",
                AllowZeroValuation = false,
                IsFinishedGood = false,
                IsProcessLoss = false,
                IsScrapItem = false,
                Created = DateTime.UtcNow,
                CreatedBy = "",
                LastModified = DateTime.UtcNow,
                LastModifiedBy = ""
            }).ToList(),
            AdditionalCosts = entry.AdditionalCosts.Select(ac => new StockEntryAdditionalCostDto
            {
                ExpenseAccount = "", // Not available in aggregate
                Description = "", // Not available in aggregate
                Amount = 0, // Not available in aggregate
                CostCenter = "", // Not available in aggregate
                Project = "" // Not available in aggregate
            }).ToList()
        }).ToList();
    }
} 
