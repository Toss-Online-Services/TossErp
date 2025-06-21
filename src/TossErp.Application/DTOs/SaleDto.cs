namespace TossErp.Application.DTOs;

public record SaleDto(
    Guid Id,
    string SaleNumber,
    Guid BusinessId,
    Guid? CustomerId,
    decimal SubTotal,
    decimal TaxAmount,
    decimal DiscountAmount,
    decimal TotalAmount,
    string PaymentMethod,
    string PaymentStatus,
    string SaleStatus,
    DateTime SaleDate,
    Guid? CreatedBy,
    string? Notes,
    bool IsOffline,
    DateTime? SyncedAt,
    List<SaleItemDto> Items,
    List<PaymentTransactionDto> Payments);

public record CreateSaleDto(
    Guid BusinessId,
    Guid? CustomerId,
    string PaymentMethod,
    Guid? CreatedBy = null,
    string? Notes = null,
    bool IsOffline = false);

public record SaleItemDto(
    Guid Id,
    Guid SaleId,
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal? Discount,
    decimal TotalPrice);

public record AddSaleItemDto(
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal? Discount = null);

public record PaymentTransactionDto(
    Guid Id,
    Guid SaleId,
    decimal Amount,
    string PaymentMethod,
    string TransactionId,
    string Status,
    DateTime CreatedAt,
    DateTime? ProcessedAt,
    string? ErrorMessage);

public record AddPaymentDto(
    decimal Amount,
    string PaymentMethod,
    string TransactionId = "",
    string Status = "completed");

public record SalesSummaryDto(
    decimal TotalSales,
    int TransactionCount,
    decimal AverageTransaction,
    List<string> TopProducts,
    DateTime FromDate,
    DateTime ToDate); 
