using MediatR;
using TossErp.Stock.Application.Common.Interfaces;
using TossErp.Stock.Domain.Entities;
using TossErp.Stock.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace TossErp.Stock.Application.StockEntries.Commands.UpdateStockEntry;

public class UpdateStockEntryCommandHandler : IRequestHandler<UpdateStockEntryCommand, Guid>
{
    private readonly IStockEntryRepository _stockEntryRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<UpdateStockEntryCommandHandler> _logger;

    public UpdateStockEntryCommandHandler(
        IStockEntryRepository stockEntryRepository,
        ICurrentUserService currentUserService,
        ILogger<UpdateStockEntryCommandHandler> logger)
    {
        _stockEntryRepository = stockEntryRepository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<Guid> Handle(UpdateStockEntryCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating stock entry {StockEntryId}", request.Id);

        var stockEntry = await _stockEntryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (stockEntry == null)
        {
            throw new TossErp.Stock.Domain.Exceptions.NotFoundException($"Stock entry with ID {request.Id} not found.");
        }

        // Validate current status - can only update draft entries
        if (stockEntry.IsPosted)
        {
            throw new ValidationException($"Stock entry {request.Id} cannot be updated. Current status: Posted");
        }

        // Update the stock entry
        stockEntry.Update(request.ReferenceNo, request.Notes);

        // Save changes
        await _stockEntryRepository.UpdateAsync(stockEntry, cancellationToken);

        _logger.LogInformation("Successfully updated stock entry {StockEntryId}", request.Id);

        return stockEntry.Id;
    }
} 
