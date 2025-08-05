using MediatR;
using TossErp.Stock.Application.Common.DTOs;
using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Aggregates.StockEntryAggregate;
using Microsoft.Extensions.Logging;

namespace TossErp.Stock.Application.StockEntries.Commands.DeleteStockEntry;

public class DeleteStockEntryCommandHandler : IRequestHandler<DeleteStockEntryCommand>
{
    private readonly IRepository<StockEntryAggregate> _repository;

    public DeleteStockEntryCommandHandler(IRepository<StockEntryAggregate> repository)
    {
        _repository = repository;
    }

    public async Task Handle(DeleteStockEntryCommand request, CancellationToken cancellationToken)
    {
        var stockEntry = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (stockEntry == null)
        {
            throw new TossErp.Stock.Domain.Exceptions.NotFoundException($"Stock entry with ID {request.Id} not found");
        }

        await _repository.DeleteAsync(stockEntry, cancellationToken);
    }
} 
