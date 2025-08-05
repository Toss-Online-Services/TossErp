using System.ComponentModel;
using System.Globalization;

namespace TossErp.Stock.Domain.ValueObjects;

[TypeConverter(typeof(SerialNumberTypeConverter))]
public record SerialNumber
{
    public string Value { get; }

    public SerialNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Serial number cannot be empty.", nameof(value));

        Value = value.Trim().ToUpperInvariant();
    }

    public static implicit operator string(SerialNumber serialNumber) => serialNumber.Value;

    public static explicit operator SerialNumber(string value) => new(value);

    public override string ToString() => Value;
}

public class SerialNumberTypeConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string stringValue)
        {
            return new SerialNumber(stringValue);
        }

        return base.ConvertFrom(context, culture, value);
    }
} 
