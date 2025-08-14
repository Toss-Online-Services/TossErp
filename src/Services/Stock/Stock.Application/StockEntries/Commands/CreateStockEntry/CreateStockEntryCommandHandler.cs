using System;
using MediatR;
using TossErp.Stock.Application.Common.DTOs;
using TossErp.Stock.Application.Common.Interfaces;
using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Aggregates.StockEntryAggregate;
using Microsoft.Extensions.Logging;

namespace TossErp.Stock.Application.StockEntries.Commands.CreateStockEntry;

/// <summary>
/// Handler for creating stock entries
/// </summary>
public class CreateStockEntryCommandHandler : IRequestHandler<CreateStockEntryCommand, StockEntryDto>
{
    private readonly IStockEntryRepository _stockEntryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<CreateStockEntryCommandHandler> _logger;

    public CreateStockEntryCommandHandler(
        IStockEntryRepository stockEntryRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<CreateStockEntryCommandHandler> logger)
    {
        _stockEntryRepository = stockEntryRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<StockEntryDto> Handle(CreateStockEntryCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating stock entry: {EntryType}", request.StockEntryType);

        // Generate a unique entry number
        var entryNumber = await _stockEntryRepository.GetNextEntryNumberAsync(request.Company, cancellationToken);
        
        // Create the stock entry aggregate
        var stockEntry = new StockEntryAggregate(
            new TossErp.Stock.Domain.ValueObjects.StockEntryNo(entryNumber),
            request.PostingDate,
            request.Company,
            request.ReferenceNo,
            request.Remarks
        );

        // Add stock entry details
        foreach (var detail in request.Details)
        {
            var stockEntryDetail = new TossErp.Stock.Domain.Aggregates.StockEntryAggregate.Entities.StockEntryDetail(
                stockEntry.Id,
                Guid.NewGuid(), // ItemId - this should be resolved from ItemCode
                Guid.NewGuid(), // WarehouseId - this should be resolved from WarehouseCode
                new TossErp.Stock.Domain.ValueObjects.Quantity(detail.Qty, "PCS"), // Default unit
                new TossErp.Stock.Domain.ValueObjects.Rate(detail.Rate),
                null, // BinId - optional
                detail.BatchNo,
                detail.SerialNo,
                null, // ExpiryDate - could be calculated or provided
                detail.Remarks
            );

            stockEntry.AddDetail(stockEntryDetail);
        }

        // Save to repository
        await _stockEntryRepository.AddAsync(stockEntry, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully created stock entry {EntryNumber}", entryNumber);

        return new StockEntryDto
        {
            Id = stockEntry.Id,
            StockEntryNo = entryNumber,
            StockEntryType = request.StockEntryType,
            Purpose = request.Purpose,
            PostingDate = request.PostingDate,
            PostingTime = request.PostingTime,
            Company = request.Company,
            SourceWarehouse = request.SourceWarehouse,
            TargetWarehouse = request.TargetWarehouse,
            SourceCostCenter = request.SourceCostCenter,
            TargetCostCenter = request.TargetCostCenter,
            Project = request.Project,
            ReferenceNo = request.ReferenceNo,
            ReferenceType = request.ReferenceType,
            Status = stockEntry.IsPosted ? "Posted" : "Draft",
            Remarks = request.Remarks,
            TotalAmount = stockEntry.GetTotalValue(),
            TotalQty = stockEntry.GetTotalQuantity(),
            IsOpening = request.IsOpening,
            IsRepostItemValuation = request.IsRepostItemValuation,
            IsRepostItemValuationAllowed = request.IsRepostItemValuationAllowed,
            IsRepostItemValuationDone = request.IsRepostItemValuationDone,
            Created = DateTime.UtcNow,
            CreatedBy = _currentUserService.UserId ?? string.Empty,
            LastModified = DateTime.UtcNow,
            LastModifiedBy = _currentUserService.UserId ?? string.Empty
        };
    }
}
