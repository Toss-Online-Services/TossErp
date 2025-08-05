using MediatR;

namespace TossErp.Stock.Application.Batches.Commands.DeleteBatch;

public record DeleteBatchCommand : IRequest<bool>
{
    public Guid Id { get; init; }
} 
