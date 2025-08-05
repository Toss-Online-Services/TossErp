using MediatR;

namespace TossErp.Stock.Application.Bins.Commands.DeleteBin;

public record DeleteBinCommand(Guid Id) : IRequest; 
