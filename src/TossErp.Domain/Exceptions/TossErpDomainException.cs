namespace TossErp.Domain.Exceptions
{
    public class TossErpDomainException : Exception
    {
        public TossErpDomainException()
        { }

        public TossErpDomainException(string message)
            : base(message)
        { }

        public TossErpDomainException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
} 
