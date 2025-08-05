using System.ComponentModel;
using System.Globalization;

namespace TossErp.Stock.Domain.ValueObjects;

[TypeConverter(typeof(StockEntryNoTypeConverter))]
public record StockEntryNo
{
    public string Value { get; }

    public StockEntryNo(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Stock entry number cannot be empty.", nameof(value));

        Value = value.Trim().ToUpperInvariant();
    }

    public static implicit operator string(StockEntryNo stockEntryNo) => stockEntryNo.Value;

    public static explicit operator StockEntryNo(string value) => new(value);

    public override string ToString() => Value;
}

public class StockEntryNoTypeConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string stringValue)
        {
            return new StockEntryNo(stringValue);
        }

        return base.ConvertFrom(context, culture, value);
    }
} 
