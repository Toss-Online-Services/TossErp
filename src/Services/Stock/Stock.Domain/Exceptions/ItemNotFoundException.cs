namespace TossErp.Stock.Domain.Exceptions;

public class ItemNotFoundException : NotFoundException
{
    public ItemNotFoundException()
        : base()
    {
    }

    public ItemNotFoundException(string message)
        : base(message)
    {
    }

    public ItemNotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public ItemNotFoundException(Guid itemId)
        : base("Item", itemId)
    {
    }
} 
