namespace Stock.Domain.Exceptions;

public class StockEntryNotFoundException : StockDomainException
{
    public string StockEntryName { get; }

    public StockEntryNotFoundException(string stockEntryName) 
        : base($"Stock entry with name '{stockEntryName}' was not found.")
    {
        StockEntryName = stockEntryName;
    }
} 
