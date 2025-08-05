using MediatR;
using TossErp.Stock.Application.Common.DTOs;
using TossErp.Stock.Application.Common.Interfaces;
using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Entities;
using TossErp.Stock.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace TossErp.Stock.Application.Batches.Commands.UpdateBatch;

public class UpdateBatchCommandHandler : IRequestHandler<UpdateBatchCommand, BatchDto>
{
    private readonly IBatchRepository _batchRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<UpdateBatchCommandHandler> _logger;

    public UpdateBatchCommandHandler(
        IBatchRepository batchRepository,
        IItemRepository itemRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<UpdateBatchCommandHandler> logger)
    {
        _batchRepository = batchRepository;
        _itemRepository = itemRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<BatchDto> Handle(UpdateBatchCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating batch {BatchId}", request.Id);

        // Get existing batch
        var batch = await _batchRepository.GetByIdAsync(request.Id, cancellationToken);
        if (batch == null)
        {
            throw new TossErp.Stock.Domain.Exceptions.NotFoundException($"Batch with ID {request.Id} not found");
        }

        // Get item for mapping
        var item = await _itemRepository.GetByIdAsync(batch.Item.Id, cancellationToken);
        if (item == null)
        {
            throw new TossErp.Stock.Domain.Exceptions.NotFoundException($"Item with ID {batch.Item.Id} not found");
        }

        // Update properties using domain methods
        if (!string.IsNullOrEmpty(request.Name) && request.Name != batch.Name)
        {
            // Check if new name already exists for this item
            var existingBatch = await _batchRepository.GetByNameAsync(request.Name, cancellationToken);
            if (existingBatch != null && existingBatch.Id != request.Id && existingBatch.Item.Id == batch.Item.Id)
            {
                throw new ValidationException($"Batch name {request.Name} already exists for item {item.ItemName}");
            }
        }

        if (request.ManufacturingDate.HasValue)
        {
            batch.UpdateManufacturingDate(request.ManufacturingDate.Value);
        }

        if (request.ExpiryDate.HasValue)
        {
            batch.UpdateExpiryDate(request.ExpiryDate.Value);
        }

        if (request.WarrantyExpiryDate.HasValue)
        {
            batch.UpdateWarrantyExpiryDate(request.WarrantyExpiryDate.Value);
        }

        if (!string.IsNullOrEmpty(request.Supplier))
        {
            batch.UpdateSupplier(request.Supplier);
        }

        if (!string.IsNullOrEmpty(request.ReferenceDocumentType) || !string.IsNullOrEmpty(request.ReferenceDocumentNo))
        {
            batch.UpdateReferenceDocument(request.ReferenceDocumentType, request.ReferenceDocumentNo, request.ReferenceDocumentDetailNo);
        }

        if (!string.IsNullOrEmpty(request.Description))
        {
            batch.UpdateDescription(request.Description);
        }

        if (!string.IsNullOrEmpty(request.Remarks))
        {
            batch.UpdateRemarks(request.Remarks);
        }

        if (request.Quantity >= 0)
        {
            batch.UpdateQuantity(request.Quantity);
        }

        // Update retain sample information if provided
        if (request.RetainSample > 0 || !string.IsNullOrEmpty(request.RetainSampleUOM))
        {
            batch.UpdateRetainSample(
                request.RetainSample,
                request.RetainSampleQuantity,
                request.RetainSampleUOM,
                request.RetainSampleUOMQuantity,
                request.RetainSampleWarehouse,
                request.RetainSampleBin);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully updated batch {BatchId}", batch.Id);

        // Map to DTO and return
        return new BatchDto
        {
            Id = batch.Id,
            BatchId = batch.Name,
            ItemCode = batch.ItemCode,
            ItemName = item.ItemName,
            Description = batch.Description ?? string.Empty,
            ManufacturingDate = batch.ManufacturingDate,
            ExpiryDate = batch.ExpiryDate,
            WarrantyExpiryDate = batch.WarrantyExpiryDate,
            Qty = batch.Quantity,
            TransferQty = batch.TransferQuantity,
            ConsumedQty = batch.ConsumedQuantity,
            DispatchedQty = batch.DispatchedQuantity,
            ReturnedQty = batch.ReturnedQuantity,
            ScrappedQty = batch.ScrappedQuantity,
            RetainSample = batch.RetainSample,
            RetainSampleQty = batch.RetainSampleQuantity,
            RetainSampleUOM = batch.RetainSampleUOM,
            RetainSampleUOMQty = batch.RetainSampleUOMQuantity,
            RetainSampleWarehouse = batch.RetainSampleWarehouse,
            RetainSampleBin = batch.RetainSampleBin,
            Disabled = batch.IsDisabled,
            Created = DateTime.UtcNow, // Use current time since entity doesn't expose Created
            CreatedBy = _currentUserService.UserId ?? string.Empty,
            LastModified = DateTime.UtcNow, // Use current time since entity doesn't expose LastModified
            LastModifiedBy = _currentUserService.UserId ?? string.Empty
        };
    }
} 
