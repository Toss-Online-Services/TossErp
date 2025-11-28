namespace Toss.Application.Operations.Queries.GetTodayView;

public record OperationsTodayDto(
    DateOnly Date,
    TodayTotalsDto Totals,
    TodayCashDto Cash,
    IReadOnlyList<TodayLowStockItemDto> LowStock,
    IReadOnlyList<TodayPurchaseOrderDto> PendingPurchaseOrders,
    IReadOnlyList<TodayCustomerOrderDto> PendingCustomerOrders,
    IReadOnlyList<TodayAlertDto> Alerts)
{
    public static OperationsTodayDto Empty(DateOnly date) =>
        new(
            date,
            new TodayTotalsDto(0m, 0, 0m),
            new TodayCashDto(0m, 0m),
            Array.Empty<TodayLowStockItemDto>(),
            Array.Empty<TodayPurchaseOrderDto>(),
            Array.Empty<TodayCustomerOrderDto>(),
            Array.Empty<TodayAlertDto>());
}

public record TodayTotalsDto(decimal SalesTotal, int Transactions, decimal AverageTicket);

public record TodayCashDto(decimal CashIn, decimal CashOut);

public record TodayLowStockItemDto(
    int ProductId,
    string ProductName,
    string ProductSku,
    int CurrentQuantity,
    int MinimumQuantity,
    int? ReorderQuantity);

public record TodayPurchaseOrderDto(
    int PurchaseOrderId,
    string PurchaseOrderNumber,
    string SupplierName,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? ExpectedAt);

public record TodayCustomerOrderDto(
    int CustomerOrderId,
    string OrderNumber,
    string CustomerName,
    string Status,
    DateTimeOffset CreatedAt);

public record TodayAlertDto(
    string Type,
    string Message);

