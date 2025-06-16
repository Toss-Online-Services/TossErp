using POS.Domain.SeedWork;

namespace POS.Domain.Common.ValueObjects;

public class Address : ValueObject
{
    public string Street { get; private set; }
    public string City { get; private set; }
    public string State { get; private set; }
    public string Country { get; private set; }
    public string PostalCode { get; private set; }

    public Address(string street, string city, string state, string country, string postalCode)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new ArgumentException("Street cannot be empty", nameof(street));
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City cannot be empty", nameof(city));
        if (string.IsNullOrWhiteSpace(state))
            throw new ArgumentException("State cannot be empty", nameof(state));
        if (string.IsNullOrWhiteSpace(country))
            throw new ArgumentException("Country cannot be empty", nameof(country));
        if (string.IsNullOrWhiteSpace(postalCode))
            throw new ArgumentException("Postal code cannot be empty", nameof(postalCode));

        Street = street;
        City = city;
        State = state;
        Country = country;
        PostalCode = postalCode;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return State;
        yield return Country;
        yield return PostalCode;
    }

    public override string ToString()
    {
        return $"{Street}, {City}, {State} {PostalCode}, {Country}";
    }
} 
