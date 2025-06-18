namespace POS.Domain.Common
{
    public interface ITimeProvider
    {
        DateTime UtcNow { get; }
    }
} 
