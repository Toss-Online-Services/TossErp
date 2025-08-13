namespace TossErp.Contracts.Events;

public record SaleItem(Guid ProductId, decimal Quantity, decimal Price);

public record SaleCompleted(
    string TenantId,
    Guid OrderId,
    IReadOnlyList<SaleItem> Items,
    decimal Total,
    string PaymentType,
    DateTimeOffset At
);

public record StockLow(
    string TenantId,
    Guid ProductId,
    decimal OnHand,
    decimal Threshold,
    DateTimeOffset At
);

public record GroupBuyCreated(string TenantId, Guid GroupBuyId, DateTimeOffset At);
public record GroupBuyJoined(string TenantId, Guid GroupBuyId, string UserId, decimal Quantity, DateTimeOffset At);
public record GroupBuyFinalized(string TenantId, Guid GroupBuyId, DateTimeOffset At);

public record StockReceived(string TenantId, Guid ProductId, decimal Quantity, DateTimeOffset At);
public record ExpenseRecorded(string TenantId, Guid ExpenseId, decimal Amount, DateTimeOffset At);
public record DailySummaryReady(string TenantId, DateTimeOffset At);



