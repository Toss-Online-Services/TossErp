namespace POS.Domain.AggregatesModel.StoreAggregate;

public class StoreDevice
{
    public string DeviceId { get; private set; }
    public string Name { get; private set; }
    public string Type { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime LastSeen { get; private set; }

    private StoreDevice()
    {
        DeviceId = string.Empty;
        Name = string.Empty;
        Type = string.Empty;
        IsActive = true;
        LastSeen = DateTime.UtcNow;
    }

    public StoreDevice(string deviceId, string type, string? name = null)
    {
        DeviceId = deviceId;
        Type = type;
        Name = name ?? type;
        IsActive = true;
        LastSeen = DateTime.UtcNow;
    }

    public void UpdateLastSeen()
    {
        LastSeen = DateTime.UtcNow;
    }

    public void UpdateName(string name)
    {
        Name = name;
    }
}
