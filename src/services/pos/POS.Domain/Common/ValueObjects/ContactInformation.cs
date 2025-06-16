using POS.Domain.SeedWork;

namespace POS.Domain.Common.ValueObjects;

public class ContactInformation : ValueObject
{
    public string Email { get; private set; }
    public string PhoneNumber { get; private set; }

    public ContactInformation(string email, string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number cannot be empty", nameof(phoneNumber));

        if (!IsValidEmail(email))
            throw new ArgumentException("Invalid email format", nameof(email));
        if (!IsValidPhoneNumber(phoneNumber))
            throw new ArgumentException("Invalid phone number format", nameof(phoneNumber));

        Email = email.ToLowerInvariant();
        PhoneNumber = phoneNumber;
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private static bool IsValidPhoneNumber(string phoneNumber)
    {
        // Basic validation - can be enhanced based on specific requirements
        return phoneNumber.Length >= 10 && phoneNumber.All(c => char.IsDigit(c) || c == '+' || c == '-' || c == ' ');
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Email;
        yield return PhoneNumber;
    }

    public override string ToString()
    {
        return $"Email: {Email}, Phone: {PhoneNumber}";
    }
} 
