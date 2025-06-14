using TossErp.POS.Domain.Common;

namespace TossErp.POS.Domain.AggregatesModel.BuyerAggregate
{
    public class Address : ValueObject
    {
        public string Street { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }
        public string Country { get; private set; }
        public string ZipCode { get; private set; }

        private Address() { }

        public Address(string street, string city, string state, string country, string zipCode)
        {
            if (string.IsNullOrWhiteSpace(street))
                throw new DomainException("Street cannot be empty");
            if (string.IsNullOrWhiteSpace(city))
                throw new DomainException("City cannot be empty");
            if (string.IsNullOrWhiteSpace(state))
                throw new DomainException("State cannot be empty");
            if (string.IsNullOrWhiteSpace(country))
                throw new DomainException("Country cannot be empty");
            if (string.IsNullOrWhiteSpace(zipCode))
                throw new DomainException("Zip code cannot be empty");

            Street = street;
            City = city;
            State = state;
            Country = country;
            ZipCode = zipCode;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Street;
            yield return City;
            yield return State;
            yield return Country;
            yield return ZipCode;
        }
    }
} 
