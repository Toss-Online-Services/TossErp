namespace Toss.Application.Common.Extensions;

public static class DateTimeExtensions
{
    public static DateTime ToDateTime(this DateTimeOffset dateTimeOffset)
    {
        return dateTimeOffset.DateTime;
    }
    
    public static DateTime? ToDateTime(this DateTimeOffset? dateTimeOffset)
    {
        return dateTimeOffset?.DateTime;
    }
}

