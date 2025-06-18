using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.OrderAggregate;

public class OrderStatus : Enumeration
{
    public static OrderStatus Created = new(0, nameof(Created));
    public static OrderStatus Draft = new(1, nameof(Draft));
    public static OrderStatus Confirmed = new(2, nameof(Confirmed));
    public static OrderStatus Processing = new(3, nameof(Processing));
    public static OrderStatus Ready = new(4, nameof(Ready));
    public static OrderStatus Completed = new(5, nameof(Completed));
    public static OrderStatus Cancelled = new(6, nameof(Cancelled));
    public static OrderStatus Refunded = new(7, nameof(Refunded));

    public string Value { get; }

    // For EF Core
    private OrderStatus() : base(-1, string.Empty) { Value = string.Empty; }

    public OrderStatus(int id, string name) : base(id, name)
    {
        Value = name;
    }
} 
