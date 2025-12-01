namespace Toss.Domain.Enums;

/// <summary>
/// Types of business events for analytics tracking
/// </summary>
public enum BusinessEventType
{
    Login = 0,
    PosSale = 1,
    StockAlertResolved = 2,
    ModuleUsage = 3,
    StockOut = 4,
    PurchaseOrderCreated = 5,
    InvoiceCreated = 6,
    PaymentReceived = 7,
    CustomerCreated = 8,
    ProductCreated = 9,
    Other = 99
}

