using MediatR;

namespace TossErp.Stock.Application.StockEntries.Commands.DeleteStockEntry;

public record DeleteStockEntryCommand(Guid Id) : IRequest; 
