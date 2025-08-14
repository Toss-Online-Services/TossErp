namespace TossErp.Stock.Domain.Exceptions;

public class InvalidStockOperationException : StockDomainException
{
    public string? Operation { get; }
    public string? Reason { get; }

    public InvalidStockOperationException(string operation, string reason) 
        : base($"Invalid stock operation '{operation}': {reason}")
    {
        Operation = operation;
        Reason = reason;
    }

    public InvalidStockOperationException(string message)
        : base(message)
    {
    }

    public InvalidStockOperationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
} 
