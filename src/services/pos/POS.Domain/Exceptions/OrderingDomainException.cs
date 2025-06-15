namespace POS.Domain.Exceptions;

/// <summary>
/// Exception type for domain exceptions
/// </summary>
public class POSDomainException : Exception
{
    public POSDomainException()
    { }

    public POSDomainException(string message)
        : base(message)
    { }

    public POSDomainException(string message, Exception innerException)
        : base(message, innerException)
    { }
}
