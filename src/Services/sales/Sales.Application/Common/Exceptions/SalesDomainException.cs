namespace TossErp.Sales.Application.Common.Exceptions;

/// <summary>
/// Domain exception for Sales service with error codes
/// </summary>
public class SalesDomainException : Exception
{
    public string ErrorCode { get; }

    public SalesDomainException(string message, string errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }

    public SalesDomainException(string message, string errorCode, Exception innerException) : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}

/// <summary>
/// Error codes for Sales service
/// </summary>
public static class SalesErrorCodes
{
    // Till-related errors (1000-1099)
    public const string TillNotFound = "SALES-1000";
    public const string TillNotOpen = "SALES-1001";
    public const string TillAlreadyOpen = "SALES-1002";
    public const string TillAlreadyClosed = "SALES-1003";
    public const string TillInsufficientCash = "SALES-1004";
    public const string TillInvalidBalance = "SALES-1005";

    // Sale-related errors (1100-1199)
    public const string SaleNotFound = "SALES-1100";
    public const string SaleAlreadyCompleted = "SALES-1101";
    public const string SaleAlreadyCancelled = "SALES-1102";
    public const string SaleNoItems = "SALES-1103";
    public const string SaleNoPayments = "SALES-1104";
    public const string SaleInsufficientPayment = "SALES-1105";
    public const string SaleInvalidStatus = "SALES-1106";
    public const string SaleInvalidDiscount = "SALES-1107";

    // Item-related errors (1200-1299)
    public const string ItemNotFound = "SALES-1200";
    public const string ItemInvalidQuantity = "SALES-1201";
    public const string ItemInvalidPrice = "SALES-1202";
    public const string ItemInvalidTaxRate = "SALES-1203";
    public const string ItemOutOfStock = "SALES-1204";

    // Payment-related errors (1300-1399)
    public const string PaymentFailed = "SALES-1300";
    public const string PaymentInvalidAmount = "SALES-1301";
    public const string PaymentInvalidMethod = "SALES-1302";
    public const string PaymentGatewayError = "SALES-1303";
    public const string PaymentInsufficientFunds = "SALES-1304";

    // Receipt-related errors (1400-1499)
    public const string ReceiptGenerationFailed = "SALES-1400";
    public const string ReceiptNumberInvalid = "SALES-1401";
    public const string ReceiptNumberDuplicate = "SALES-1402";

    // Validation errors (1500-1599)
    public const string ValidationError = "SALES-1500";
    public const string InvalidCustomerData = "SALES-1501";
    public const string InvalidTillData = "SALES-1502";
    public const string InvalidSaleData = "SALES-1503";

    // System errors (1600-1699)
    public const string DatabaseError = "SALES-1600";
    public const string ExternalServiceError = "SALES-1601";
    public const string ConfigurationError = "SALES-1602";
    public const string UnauthorizedAccess = "SALES-1603";
}
