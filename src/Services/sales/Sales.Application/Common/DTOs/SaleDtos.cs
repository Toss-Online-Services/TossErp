using TossErp.Sales.Domain.Enums;
using TossErp.Sales.Domain.ValueObjects;

namespace TossErp.Sales.Application.Common.DTOs;

/// <summary>
/// Sale DTO for API responses
/// </summary>
public class SaleDto
{
    public Guid Id { get; set; }
    public string ReceiptNumber { get; set; } = string.Empty;
    public Guid TillId { get; set; }
    public string TillName { get; set; } = string.Empty;
    public Guid? CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public SaleStatus Status { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal Total { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal ChangeAmount { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public List<SaleItemDto> Items { get; set; } = new();
    public List<PaymentDto> Payments { get; set; } = new();
}

/// <summary>
/// Sale item DTO
/// </summary>
public class SaleItemDto
{
    public Guid Id { get; set; }
    public Guid ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string ItemSku { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TaxRate { get; set; }
    public decimal LineTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal LineTotalIncludingTax { get; set; }
}

/// <summary>
/// Payment DTO
/// </summary>
public class PaymentDto
{
    public Guid Id { get; set; }
    public PaymentMethod Method { get; set; }
    public decimal Amount { get; set; }
    public string? Reference { get; set; }
    public string? CardLast4Digits { get; set; }
    public string? CardType { get; set; }
    public string? AuthorizationCode { get; set; }
    public DateTime ProcessedAt { get; set; }
    public bool IsSuccessful { get; set; }
    public string? FailureReason { get; set; }
}

/// <summary>
/// Request DTO for creating a sale
/// </summary>
public class CreateSaleRequest
{
    public Guid TillId { get; set; }
    public Guid? CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public List<CreateSaleItemRequest> Items { get; set; } = new();
    public decimal? DiscountAmount { get; set; }
    public string? DiscountReason { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Request DTO for creating a sale item
/// </summary>
public class CreateSaleItemRequest
{
    public Guid ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string ItemSku { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TaxRate { get; set; }
}

/// <summary>
/// Request DTO for adding a payment
/// </summary>
public class AddPaymentRequest
{
    public PaymentMethod Method { get; set; }
    public decimal Amount { get; set; }
    public string? Reference { get; set; }
    public string? CardLast4Digits { get; set; }
    public string? CardType { get; set; }
    public string? AuthorizationCode { get; set; }
}

/// <summary>
/// Daily sales summary DTO
/// </summary>
public class DailySalesSummaryDto
{
    public DateTime Date { get; set; }
    public Guid? TillId { get; set; }
    public string TillName { get; set; } = string.Empty;
    public int TotalSales { get; set; }
    public int TotalCancelled { get; set; }
    public int TotalTransactions { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalTax { get; set; }
    public decimal TotalDiscount { get; set; }
    public decimal AverageTransactionValue { get; set; }
    public List<TopSellingItemDto> TopSellingItems { get; set; } = new();
    public List<SalesByHourDto> SalesByHour { get; set; } = new();
    public List<PaymentMethodBreakdownDto> PaymentMethodBreakdown { get; set; } = new();
}

/// <summary>
/// Top selling item DTO
/// </summary>
public class TopSellingItemDto
{
    public Guid ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string ItemSku { get; set; } = string.Empty;
    public decimal TotalQuantity { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AveragePrice { get; set; }
}

/// <summary>
/// Sales by hour DTO
/// </summary>
public class SalesByHourDto
{
    public int Hour { get; set; }
    public int SalesCount { get; set; }
    public decimal TotalRevenue { get; set; }
}

/// <summary>
/// Payment method breakdown DTO
/// </summary>
public class PaymentMethodBreakdownDto
{
    public PaymentMethod Method { get; set; }
    public int Count { get; set; }
    public decimal TotalAmount { get; set; }
}
