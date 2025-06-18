namespace POS.Domain.AggregatesModel.StoreAggregate;

public class StoreHours
{
    public DayOfWeek Day { get; private set; }
    public TimeSpan OpenTime { get; private set; }
    public TimeSpan CloseTime { get; private set; }
    public bool IsOpen { get; private set; }

    private StoreHours() { } // For EF Core

    public StoreHours(DayOfWeek day, TimeSpan openTime, TimeSpan closeTime, bool isOpen = true)
    {
        Day = day;
        OpenTime = openTime;
        CloseTime = closeTime;
        IsOpen = isOpen;
    }

    public void UpdateHours(TimeSpan openTime, TimeSpan closeTime)
    {
        OpenTime = openTime;
        CloseTime = closeTime;
    }
}
