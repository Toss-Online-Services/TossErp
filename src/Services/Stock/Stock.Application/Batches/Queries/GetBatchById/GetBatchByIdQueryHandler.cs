using MediatR;
using TossErp.Stock.Application.Common.DTOs;
using TossErp.Stock.Domain.Common;
using Microsoft.Extensions.Logging;

namespace TossErp.Stock.Application.Batches.Queries.GetBatchById;

public class GetBatchByIdQueryHandler : IRequestHandler<GetBatchByIdQuery, BatchDto?>
{
    private readonly IBatchRepository _batchRepository;
    private readonly IItemRepository _itemRepository;
    private readonly ILogger<GetBatchByIdQueryHandler> _logger;

    public GetBatchByIdQueryHandler(
        IBatchRepository batchRepository,
        IItemRepository itemRepository,
        ILogger<GetBatchByIdQueryHandler> logger)
    {
        _batchRepository = batchRepository;
        _itemRepository = itemRepository;
        _logger = logger;
    }

    public async Task<BatchDto?> Handle(GetBatchByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting batch by ID {BatchId}", request.Id);

        var batch = await _batchRepository.GetByIdAsync(request.Id, cancellationToken);
        if (batch == null)
        {
            _logger.LogWarning("Batch with ID {BatchId} not found", request.Id);
            return null;
        }

        var item = await _itemRepository.GetByIdAsync(batch.Item.Id, cancellationToken);

        var batchDto = new BatchDto
        {
            Id = batch.Id,
            BatchId = batch.Name,
            ItemCode = batch.ItemCode,
            ItemName = item?.ItemName ?? string.Empty,
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
            CreatedBy = string.Empty, // Use empty string since entity doesn't expose CreatedBy
            LastModified = DateTime.UtcNow, // Use current time since entity doesn't expose LastModified
            LastModifiedBy = string.Empty // Use empty string since entity doesn't expose LastModifiedBy
        };

        _logger.LogInformation("Successfully retrieved batch {BatchId}", batch.Id);

        return batchDto;
    }
} 
