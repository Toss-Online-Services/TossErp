using MediatR;
using TossErp.Stock.Application.Common.DTOs;
using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Aggregates.WarehouseAggregate.Entities;
using Microsoft.Extensions.Logging;

namespace TossErp.Stock.Application.Bins.Commands.DeleteBin;

public class DeleteBinCommandHandler : IRequestHandler<DeleteBinCommand>
{
    private readonly IRepository<Bin> _repository;

    public DeleteBinCommandHandler(IRepository<Bin> repository)
    {
        _repository = repository;
    }

    public async Task Handle(DeleteBinCommand request, CancellationToken cancellationToken)
    {
        var bin = await _repository.GetByIdAsync(request.Id, cancellationToken);
        
        if (bin == null)
            throw new TossErp.Stock.Domain.Exceptions.NotFoundException(nameof(Bin), request.Id);

        await _repository.DeleteAsync(bin, cancellationToken);
    }
} 
