using System.Text.RegularExpressions;

namespace Toss.Domain.ValueObjects;

public partial class PhoneNumber : ValueObject
{
    public string Number { get; private set; }

    private PhoneNumber() 
    { 
        Number = string.Empty;
    }

    public PhoneNumber(string number)
    {
        if (string.IsNullOrWhiteSpace(number))
            throw new ArgumentException("Phone number cannot be empty", nameof(number));

        var cleaned = CleanPhoneNumber(number);
        
        if (!IsValidSouthAfricanNumber(cleaned))
            throw new ArgumentException($"Invalid South African phone number: {number}", nameof(number));

        Number = cleaned;
    }

    private static string CleanPhoneNumber(string number)
    {
        // Remove spaces, dashes, parentheses
        return PhoneCleanRegex().Replace(number, "");
    }

    private static bool IsValidSouthAfricanNumber(string number)
    {
        // South African numbers: +27 followed by 9 digits
        // Or 0 followed by 9 digits
        return PhoneValidationRegex().IsMatch(number);
    }

    public string ToInternationalFormat()
    {
        if (Number.StartsWith("+27"))
            return Number;

        if (Number.StartsWith("0"))
            return "+27" + Number[1..];

        if (Number.StartsWith("27"))
            return "+" + Number;

        return Number;
    }

    public string ToLocalFormat()
    {
        if (Number.StartsWith("+27"))
            return "0" + Number[3..];

        if (Number.StartsWith("27"))
            return "0" + Number[2..];

        return Number;
    }

    public override string ToString() => ToInternationalFormat();

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Number;
    }

    [GeneratedRegex(@"[\s\-\(\)]")]
    private static partial Regex PhoneCleanRegex();

    [GeneratedRegex(@"^(\+27|27|0)[6-8][0-9]{8}$")]
    private static partial Regex PhoneValidationRegex();
}

