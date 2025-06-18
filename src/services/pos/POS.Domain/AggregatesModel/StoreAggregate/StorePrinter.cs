namespace POS.Domain.AggregatesModel.StoreAggregate;

public class StorePrinter
{
    public string PrinterId { get; private set; }
    public string Name { get; private set; }
    public string Type { get; private set; }
    public bool IsDefault { get; private set; }
    public bool IsActive { get; private set; }

    private StorePrinter()
    {
        PrinterId = string.Empty;
        Name = string.Empty;
        Type = string.Empty;
        IsDefault = false;
        IsActive = true;
    }

    public StorePrinter(string printerId, string type, string? name = null)
    {
        PrinterId = printerId;
        Type = type;
        Name = name ?? type;
        IsDefault = false;
        IsActive = true;
    }

    public void UpdateName(string name)
    {
        Name = name;
    }
} 
