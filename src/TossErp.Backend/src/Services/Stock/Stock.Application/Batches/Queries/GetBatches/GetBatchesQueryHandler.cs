using MediatR;
using TossErp.Stock.Application.Common.DTOs;
using TossErp.Stock.Domain.Common;
using Microsoft.Extensions.Logging;

namespace TossErp.Stock.Application.Batches.Queries.GetBatches;

public class GetBatchesQueryHandler : IRequestHandler<GetBatchesQuery, List<BatchDto>>
{
    private readonly IBatchRepository _batchRepository;
    private readonly IItemRepository _itemRepository;
    private readonly ILogger<GetBatchesQueryHandler> _logger;

    public GetBatchesQueryHandler(
        IBatchRepository batchRepository,
        IItemRepository itemRepository,
        ILogger<GetBatchesQueryHandler> logger)
    {
        _batchRepository = batchRepository;
        _itemRepository = itemRepository;
        _logger = logger;
    }

    public async Task<List<BatchDto>> Handle(GetBatchesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting batches with filters: ItemId={ItemId}, SearchTerm={SearchTerm}, IsDisabled={IsDisabled}", 
            request.ItemId, request.SearchTerm, request.IsDisabled);

        var batches = await _batchRepository.GetAllAsync(cancellationToken);

        var batchDtos = new List<BatchDto>();

        foreach (var batch in batches)
        {
            var item = await _itemRepository.GetByIdAsync(batch.Item.Id, cancellationToken);
            
            batchDtos.Add(new BatchDto
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
            });
        }

        _logger.LogInformation("Retrieved {Count} batches", batchDtos.Count);

        return batchDtos;
    }
} 
