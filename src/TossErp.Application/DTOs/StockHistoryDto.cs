namespace TossErp.Application.DTOs;

public record StockHistoryDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    StockAdjustmentType Type,
    int Quantity,
    int PreviousStock,
    int NewStock,
    string Reason,
    string User,
    DateTime Date);

public record StockAdjustmentDto(
    Guid ProductId,
    StockAdjustmentType Type,
    int Quantity,
    string Reason);

public enum StockAdjustmentType
{
    Add,
    Remove,
    Set
} 
