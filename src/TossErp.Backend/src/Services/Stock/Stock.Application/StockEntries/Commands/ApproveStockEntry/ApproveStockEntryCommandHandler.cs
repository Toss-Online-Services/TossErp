using MediatR;
using TossErp.Stock.Application.Common.DTOs;
using TossErp.Stock.Domain.Entities;
using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Exceptions;
using TossErp.Stock.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace TossErp.Stock.Application.StockEntries.Commands.ApproveStockEntry;

public class ApproveStockEntryCommandHandler : IRequestHandler<ApproveStockEntryCommand, StockEntryDto>
{
    private readonly IStockEntryRepository _stockEntryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<ApproveStockEntryCommandHandler> _logger;

    public ApproveStockEntryCommandHandler(
        IStockEntryRepository stockEntryRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<ApproveStockEntryCommandHandler> logger)
    {
        _stockEntryRepository = stockEntryRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<StockEntryDto> Handle(ApproveStockEntryCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Approving stock entry {StockEntryId}", request.Id);

        var stockEntry = await _stockEntryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (stockEntry == null)
        {
            throw new TossErp.Stock.Domain.Exceptions.NotFoundException($"Stock entry with ID {request.Id} not found.");
        }

        // Validate current status - can only approve draft entries
        if (stockEntry.IsPosted)
        {
            throw new ValidationException($"Stock entry {request.Id} cannot be approved. Current status: Posted");
        }

        // Post the stock entry (this is the approval action)
        stockEntry.Post(_currentUserService.UserId ?? "system");

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully approved stock entry {StockEntryId}", request.Id);

        // Map to DTO and return
        return new StockEntryDto
        {
            Id = stockEntry.Id,
            StockEntryNo = stockEntry.EntryNumber.Value,
            StockEntryType = "Stock Entry",
            Purpose = "Stock Entry Approval",
            PostingDate = stockEntry.EntryDate.Date,
            PostingTime = stockEntry.EntryDate,
            Company = "Default",
            SourceWarehouse = "",
            TargetWarehouse = "",
            SourceCostCenter = "",
            TargetCostCenter = "",
            Project = "",
            ReferenceNo = stockEntry.Reference ?? "",
            ReferenceType = "Stock Entry",
            Status = stockEntry.IsPosted ? "Posted" : "Draft",
            Remarks = stockEntry.Notes ?? "",
            TotalAmount = stockEntry.Details.Sum(d => d.GetTotalValue()),
            TotalQty = stockEntry.Details.Sum(d => d.Quantity.Value),
            IsOpening = false,
            IsRepostItemValuation = false,
            IsRepostItemValuationAllowed = false,
            IsRepostItemValuationDone = false,
            Created = DateTime.UtcNow,
            CreatedBy = string.Empty,
            LastModified = DateTime.UtcNow,
            LastModifiedBy = string.Empty,
            Details = stockEntry.Details.Select(d => new StockEntryDetailDto
            {
                Id = d.Id,
                StockEntryId = d.StockEntryId,
                ItemCode = d.ItemId.ToString(),
                ItemName = string.Empty,
                Description = string.Empty,
                UOM = string.Empty,
                Qty = d.Quantity.Value,
                TransferQty = 0,
                ConsumedQty = 0,
                BasicRate = d.Rate.Value,
                BasicAmount = d.GetTotalValue(),
                AdditionalCost = 0,
                Rate = d.Rate.Value,
                Amount = d.GetTotalValue(),
                SourceWarehouse = d.WarehouseId.ToString(),
                TargetWarehouse = d.WarehouseId.ToString(),
                SourceBin = string.Empty,
                TargetBin = string.Empty,
                BatchNo = d.BatchNo ?? string.Empty,
                SerialNo = d.SerialNo ?? string.Empty,
                CostCenter = string.Empty,
                Project = string.Empty,
                Remarks = d.Remarks ?? string.Empty,
                AllowZeroValuation = false,
                IsFinishedGood = false,
                IsProcessLoss = false,
                IsScrapItem = false,
                Created = DateTime.UtcNow,
                CreatedBy = string.Empty,
                LastModified = DateTime.UtcNow,
                LastModifiedBy = string.Empty
            }).ToList(),
            AdditionalCosts = new List<StockEntryAdditionalCostDto>()
        };
    }
} 



