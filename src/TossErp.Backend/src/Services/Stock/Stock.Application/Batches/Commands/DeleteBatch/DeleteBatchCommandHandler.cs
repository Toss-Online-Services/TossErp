using MediatR;
using TossErp.Stock.Application.Common.DTOs;
using TossErp.Stock.Application.Common.Interfaces;
using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Entities;
using TossErp.Stock.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace TossErp.Stock.Application.Batches.Commands.DeleteBatch;

public class DeleteBatchCommandHandler : IRequestHandler<DeleteBatchCommand, bool>
{
    private readonly IBatchRepository _batchRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<DeleteBatchCommandHandler> _logger;

    public DeleteBatchCommandHandler(
        IBatchRepository batchRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<DeleteBatchCommandHandler> logger)
    {
        _batchRepository = batchRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteBatchCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting batch {BatchId}", request.Id);

        // Get existing batch
        var batch = await _batchRepository.GetByIdAsync(request.Id, cancellationToken);
        if (batch == null)
        {
            throw new TossErp.Stock.Domain.Exceptions.NotFoundException($"Batch with ID {request.Id} not found");
        }

        // Check if batch can be deleted (business rules)
        if (batch.HasAvailableQuantity())
        {
            throw new ValidationException($"Cannot delete batch {batch.Name} - it still has available quantity");
        }

        // Soft delete by disabling the batch
        batch.Disable();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully deleted batch {BatchId}", batch.Id);

        return true;
    }
} 
