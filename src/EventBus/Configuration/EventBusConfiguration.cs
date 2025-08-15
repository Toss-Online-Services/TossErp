namespace eShop.EventBus.Configuration;

public class EventBusConfiguration
{
    public string EventBusConnection { get; set; } = string.Empty;
    public string EventBusUserName { get; set; } = string.Empty;
    public string EventBusPassword { get; set; } = string.Empty;
    public string EventBusRetryCount { get; set; } = "5";
    public string SubscriptionClientName { get; set; } = string.Empty;
}
