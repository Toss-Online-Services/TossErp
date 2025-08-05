using System;
using MediatR;
using TossErp.Stock.Application.Common.Interfaces;
using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Aggregates.ItemAggregate;
using Microsoft.Extensions.Logging;

namespace TossErp.Stock.Application.Items.Commands.DeleteItem;

/// <summary>
/// Handler for deleting items
/// </summary>
public class DeleteItemCommandHandler : IRequestHandler<DeleteItemCommand, bool>
{
    private readonly IItemRepository _itemRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteItemCommandHandler> _logger;

    public DeleteItemCommandHandler(
        IItemRepository itemRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteItemCommandHandler> logger)
    {
        _itemRepository = itemRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteItemCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting item {ItemId}", request.Id);

        var item = await _itemRepository.GetByIdAsync(request.Id, cancellationToken);
        if (item == null)
        {
            _logger.LogWarning("Item with ID {ItemId} not found", request.Id);
            return false;
        }

        await _itemRepository.DeleteAsync(item, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully deleted item {ItemId}", request.Id);
        return true;
    }
}
