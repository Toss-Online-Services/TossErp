using POS.Domain.Common;

namespace POS.Domain.Common.ValueObjects;

public class Address : ValueObject
{
    public string Street { get; private set; }
    public string City { get; private set; }
    public string State { get; private set; }
    public string Country { get; private set; }
    public string PostalCode { get; private set; }
    public string? AdditionalInfo { get; private set; }

    private Address() { } // For EF Core

    public Address(
        string street,
        string city,
        string state,
        string country,
        string postalCode,
        string? additionalInfo = null)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new DomainException("Street cannot be empty");

        if (string.IsNullOrWhiteSpace(city))
            throw new DomainException("City cannot be empty");

        if (string.IsNullOrWhiteSpace(state))
            throw new DomainException("State cannot be empty");

        if (string.IsNullOrWhiteSpace(country))
            throw new DomainException("Country cannot be empty");

        if (string.IsNullOrWhiteSpace(postalCode))
            throw new DomainException("Postal code cannot be empty");

        Street = street;
        City = city;
        State = state;
        Country = country;
        PostalCode = postalCode;
        AdditionalInfo = additionalInfo;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return State;
        yield return Country;
        yield return PostalCode;
        yield return AdditionalInfo;
    }

    public override bool Equals(object? obj)
    {
        if (obj is Address other)
        {
            return Street == other.Street &&
                   City == other.City &&
                   State == other.State &&
                   Country == other.Country &&
                   PostalCode == other.PostalCode &&
                   AdditionalInfo == other.AdditionalInfo;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Street, City, State, Country, PostalCode, AdditionalInfo);
    }

    public override string ToString()
    {
        var address = $"{Street}, {City}, {State} {PostalCode}, {Country}";
        if (!string.IsNullOrWhiteSpace(AdditionalInfo))
        {
            address += $" ({AdditionalInfo})";
        }
        return address;
    }

    public bool IsInCountry(string country)
    {
        return Country.Equals(country, StringComparison.OrdinalIgnoreCase);
    }

    public bool IsInState(string state)
    {
        return State.Equals(state, StringComparison.OrdinalIgnoreCase);
    }

    public bool IsInCity(string city)
    {
        return City.Equals(city, StringComparison.OrdinalIgnoreCase);
    }

    public bool HasPostalCode(string postalCode)
    {
        return PostalCode.Equals(postalCode, StringComparison.OrdinalIgnoreCase);
    }

    public Address WithAdditionalInfo(string additionalInfo)
    {
        return new Address(Street, City, State, Country, PostalCode, additionalInfo);
    }

    public Address WithoutAdditionalInfo()
    {
        return new Address(Street, City, State, Country, PostalCode);
    }
} 
