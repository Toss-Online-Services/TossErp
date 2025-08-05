namespace TossErp.Stock.Domain.Exceptions;

public abstract class StockDomainException : Exception
{
    protected StockDomainException(string message) : base(message)
    {
    }

    protected StockDomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
} 
