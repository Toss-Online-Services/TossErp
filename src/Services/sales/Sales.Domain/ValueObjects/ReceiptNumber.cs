using System.Text.RegularExpressions;

namespace TossErp.Sales.Domain.ValueObjects;

/// <summary>
/// Receipt number value object with validation
/// </summary>
public sealed record ReceiptNumber
{
    private static readonly Regex ReceiptNumberPattern = new(@"^[A-Z]{2,4}-\d{8,12}$", RegexOptions.Compiled);
    
    public string Value { get; }

    public ReceiptNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Receipt number cannot be null or empty", nameof(value));

        if (!ReceiptNumberPattern.IsMatch(value))
            throw new ArgumentException($"Invalid receipt number format: {value}. Expected format: XX-########", nameof(value));

        Value = value.ToUpperInvariant();
    }

    /// <summary>
    /// Generate a new receipt number with the given prefix
    /// </summary>
    /// <param name="prefix">Store/till prefix (2-4 characters)</param>
    /// <param name="sequenceNumber">Sequential number</param>
    /// <returns>New receipt number</returns>
    public static ReceiptNumber Generate(string prefix, long sequenceNumber)
    {
        if (string.IsNullOrWhiteSpace(prefix))
            throw new ArgumentException("Prefix cannot be null or empty", nameof(prefix));

        if (prefix.Length < 2 || prefix.Length > 4)
            throw new ArgumentException("Prefix must be 2-4 characters long", nameof(prefix));

        if (sequenceNumber <= 0)
            throw new ArgumentException("Sequence number must be positive", nameof(sequenceNumber));

        var receiptNumber = $"{prefix.ToUpperInvariant()}-{sequenceNumber:D8}";
        return new ReceiptNumber(receiptNumber);
    }

    public static implicit operator string(ReceiptNumber receiptNumber) => receiptNumber.Value;

    public override string ToString() => Value;
}
