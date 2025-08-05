using MediatR;
using TossErp.Stock.Application.Common.DTOs;
using TossErp.Stock.Domain.Entities;
using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Exceptions;
using TossErp.Stock.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace TossErp.Stock.Application.StockEntries.Commands.SubmitStockEntry;

public class SubmitStockEntryCommandHandler : IRequestHandler<SubmitStockEntryCommand, StockEntryDto>
{
    private readonly IStockEntryRepository _stockEntryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<SubmitStockEntryCommandHandler> _logger;

    public SubmitStockEntryCommandHandler(
        IStockEntryRepository stockEntryRepository, 
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<SubmitStockEntryCommandHandler> logger)
    {
        _stockEntryRepository = stockEntryRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<StockEntryDto> Handle(SubmitStockEntryCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Submitting stock entry {StockEntryId}", request.Id);

        var stockEntry = await _stockEntryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (stockEntry == null)
        {
            throw new TossErp.Stock.Domain.Exceptions.NotFoundException($"Stock entry with ID {request.Id} not found.");
        }

        // Validate current status - can only submit draft entries
        if (stockEntry.IsPosted)
        {
            throw new ValidationException($"Stock entry {request.Id} cannot be submitted. Current status: Posted");
        }

        // Validate stock entry has details
        if (!stockEntry.Details.Any())
        {
            throw new ValidationException($"Stock entry {request.Id} cannot be submitted without details.");
        }

        // Validate all details have required information
        foreach (var detail in stockEntry.Details)
        {
            if (detail.Quantity.Value <= 0)
            {
                throw new ValidationException($"Detail {detail.Id} has invalid quantity: {detail.Quantity.Value}");
            }

            if (detail.Rate.Value < 0)
            {
                throw new ValidationException($"Detail {detail.Id} has invalid rate: {detail.Rate.Value}");
            }
        }

        // Post the stock entry (this is the submission action)
        stockEntry.Post(_currentUserService.UserId ?? "system");

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully submitted stock entry {StockEntryId}", request.Id);

        // Map to DTO and return
        return new StockEntryDto
        {
            Id = stockEntry.Id,
            StockEntryNo = stockEntry.EntryNumber.Value,
            StockEntryType = "Stock Entry",
            Purpose = "Stock Entry Submission",
            PostingDate = stockEntry.EntryDate.Date,
            PostingTime = stockEntry.EntryDate,
            Company = stockEntry.Company,
            SourceWarehouse = "",
            TargetWarehouse = "", // Will need to get from first detail's warehouse
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
            Created = DateTime.UtcNow, // Use current time since aggregate doesn't expose Created
            CreatedBy = string.Empty, // Use empty string since aggregate doesn't expose CreatedBy
            LastModified = DateTime.UtcNow, // Use current time since aggregate doesn't expose LastModified
            LastModifiedBy = string.Empty, // Use empty string since aggregate doesn't expose LastModifiedBy
            Details = stockEntry.Details.Select(d => new StockEntryDetailDto
            {
                Id = d.Id,
                StockEntryId = d.StockEntryId,
                ItemCode = d.ItemId.ToString(), // Use ItemId as string since we don't have ItemCode
                ItemName = string.Empty, // Will need to get from Item aggregate
                Description = string.Empty, // Will need to get from Item aggregate
                UOM = string.Empty, // Will need to get from Item aggregate
                Qty = d.Quantity.Value,
                TransferQty = 0,
                ConsumedQty = 0,
                BasicRate = d.Rate.Value,
                BasicAmount = d.GetTotalValue(),
                AdditionalCost = 0,
                Rate = d.Rate.Value,
                Amount = d.GetTotalValue(),
                SourceWarehouse = d.WarehouseId.ToString(), // Use WarehouseId as string
                TargetWarehouse = d.WarehouseId.ToString(), // Use WarehouseId as string
                SourceBin = d.BinId?.ToString() ?? string.Empty,
                TargetBin = d.BinId?.ToString() ?? string.Empty,
                BatchNo = d.BatchNo ?? string.Empty,
                SerialNo = d.SerialNo ?? string.Empty,
                CostCenter = string.Empty,
                Project = string.Empty,
                Remarks = d.Remarks ?? string.Empty,
                AllowZeroValuation = false,
                IsFinishedGood = false,
                IsProcessLoss = false,
                IsScrapItem = false,
                Created = DateTime.UtcNow, // Use current time since aggregate doesn't expose Created
                CreatedBy = string.Empty, // Use empty string since aggregate doesn't expose CreatedBy
                LastModified = DateTime.UtcNow, // Use current time since aggregate doesn't expose LastModified
                LastModifiedBy = string.Empty // Use empty string since aggregate doesn't expose LastModifiedBy
            }).ToList(),
            AdditionalCosts = new List<StockEntryAdditionalCostDto>()
        };
    }
} 
