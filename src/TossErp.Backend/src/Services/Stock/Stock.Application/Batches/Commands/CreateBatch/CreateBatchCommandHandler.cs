using System;
using MediatR;
using TossErp.Stock.Application.Common.DTOs;
using TossErp.Stock.Application.Common.Interfaces;
using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Entities;
using TossErp.Stock.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace TossErp.Stock.Application.Batches.Commands.CreateBatch;

public class CreateBatchCommandHandler : IRequestHandler<CreateBatchCommand, BatchDto>
{
    private readonly IBatchRepository _batchRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<CreateBatchCommandHandler> _logger;

    public CreateBatchCommandHandler(
        IBatchRepository batchRepository,
        IItemRepository itemRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<CreateBatchCommandHandler> logger)
    {
        _batchRepository = batchRepository;
        _itemRepository = itemRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<BatchDto> Handle(CreateBatchCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating batch {BatchName} for item {ItemId}", request.Name, request.ItemId);

        // Validate item exists
        var item = await _itemRepository.GetByIdAsync(request.ItemId, cancellationToken);
        if (item == null)
        {
            throw new TossErp.Stock.Domain.Exceptions.NotFoundException($"Item with ID {request.ItemId} not found");
        }

        // Check if batch name already exists for this item
        var existingBatch = await _batchRepository.GetByNameAsync(request.Name, cancellationToken);
        if (existingBatch != null && existingBatch.Item.Id == request.ItemId)
        {
            throw new ValidationException($"Batch name {request.Name} already exists for item {item.ItemName}");
        }

        // Validate quantity
        if (request.Quantity < 0)
        {
            throw new ValidationException("Quantity cannot be negative");
        }

        // Create batch using the domain entity constructor
        var batch = new Batch(request.Name, item, request.Quantity);

        // Update additional properties if provided
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

        _batchRepository.Add(batch);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully created batch {BatchId} with name {BatchName}", batch.Id, batch.Name);

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
