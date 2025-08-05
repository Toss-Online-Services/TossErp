using MediatR;
using TossErp.Stock.Application.Common.DTOs;
using TossErp.Stock.Domain.Entities;
using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Exceptions;
using TossErp.Stock.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace TossErp.Stock.Application.StockEntries.Commands.RejectStockEntry;

public class RejectStockEntryCommandHandler : IRequestHandler<RejectStockEntryCommand, StockEntryDto>
{
    private readonly IStockEntryRepository _stockEntryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<RejectStockEntryCommandHandler> _logger;

    public RejectStockEntryCommandHandler(
        IStockEntryRepository stockEntryRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<RejectStockEntryCommandHandler> logger)
    {
        _stockEntryRepository = stockEntryRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<StockEntryDto> Handle(RejectStockEntryCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Rejecting stock entry {StockEntryId}", request.Id);

        var stockEntry = await _stockEntryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (stockEntry == null)
        {
            throw new TossErp.Stock.Domain.Exceptions.NotFoundException($"Stock entry with ID {request.Id} not found.");
        }

        // Validate current status - can only reject draft entries
        if (stockEntry.IsPosted)
        {
            throw new ValidationException($"Stock entry {request.Id} cannot be rejected. Current status: Posted");
        }

        // Add rejection notes to the stock entry
        var rejectionNotes = $"REJECTED by {request.RejectedBy ?? _currentUserService.UserId ?? "system"}. Reason: {request.RejectionReason ?? "No reason provided"}";
        var updatedNotes = string.IsNullOrEmpty(stockEntry.Notes) ? rejectionNotes : $"{stockEntry.Notes}\n{rejectionNotes}";
        stockEntry.Update(stockEntry.Reference, updatedNotes);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully rejected stock entry {StockEntryId}", request.Id);

        // Map to DTO and return
        return new StockEntryDto
        {
            Id = stockEntry.Id,
            StockEntryNo = stockEntry.EntryNumber.Value,
            StockEntryType = "Stock Entry",
            Purpose = "Stock Entry Rejection",
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
            Status = "Rejected",
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



