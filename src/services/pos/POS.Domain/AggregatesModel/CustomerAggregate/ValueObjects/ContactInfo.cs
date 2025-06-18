using POS.Domain.Common;

namespace POS.Domain.AggregatesModel.CustomerAggregate.ValueObjects;

public record ContactInfo
{
    public string Email { get; }
    public string PhoneNumber { get; }

    public ContactInfo(string email, string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email cannot be empty");
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new DomainException("Phone number cannot be empty");

        if (!IsValidEmail(email))
            throw new DomainException("Invalid email format");

        Email = email.Trim().ToLowerInvariant();
        PhoneNumber = phoneNumber.Trim();
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

    public static ContactInfo Create(string email, string phoneNumber) => 
        new(email, phoneNumber);
} 
