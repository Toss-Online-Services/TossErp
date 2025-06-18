using POS.Domain.Common;

namespace POS.Domain.AggregatesModel.CustomerAggregate.ValueObjects;

public record CustomerName
{
    public string FirstName { get; }
    public string LastName { get; }

    public CustomerName(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new DomainException("First name cannot be empty");
        if (string.IsNullOrWhiteSpace(lastName))
            throw new DomainException("Last name cannot be empty");

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
    }

    public string FullName => $"{FirstName} {LastName}";

    public static CustomerName Create(string firstName, string lastName) => 
        new(firstName, lastName);
} 