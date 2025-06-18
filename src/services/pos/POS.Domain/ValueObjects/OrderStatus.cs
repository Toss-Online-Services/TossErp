namespace POS.Domain.ValueObjects;

/// <summary>
/// Represents the status of an order
/// </summary>
public class OrderStatus : ValueObject
{
    public static OrderStatus Created => new(nameof(Created));
    public static OrderStatus Confirmed => new(nameof(Confirmed));
    public static OrderStatus Processing => new(nameof(Processing));
    public static OrderStatus Completed => new(nameof(Completed));
    public static OrderStatus Cancelled => new(nameof(Cancelled));
    public static OrderStatus Refunded => new(nameof(Refunded));

    public string Value { get; }

    private OrderStatus() { } // For EF Core

    private OrderStatus(string value)
    {
        Value = value;
    }

    public static OrderStatus FromString(string status)
    {
        return status.ToLower() switch
        {
            "created" => Created,
            "confirmed" => Confirmed,
            "processing" => Processing,
            "completed" => Completed,
            "cancelled" => Cancelled,
            "refunded" => Refunded,
            _ => throw new DomainException($"Invalid order status: {status}")
        };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
} 
