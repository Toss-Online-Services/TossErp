using FluentAssertions;
using POS.Domain.Common.ValueObjects;
using POS.Domain.Tests.Common;
using Xunit;

namespace POS.Domain.Tests.Common.ValueObjects;

public class AddressTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateAddress()
    {
        // Arrange & Act
        var address = TestDataFactory.ValueObjects.CreateValidAddress();

        // Assert
        address.Should().NotBeNull();
        address.Street.Should().NotBeNullOrEmpty();
        address.City.Should().NotBeNullOrEmpty();
        address.State.Should().NotBeNullOrEmpty();
        address.ZipCode.Should().NotBeNullOrEmpty();
        address.Country.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Create_WithEmptyStreet_ShouldThrowException()
    {
        // Arrange
        var city = "City";
        var state = "State";
        var country = "Country";
        var zipCode = "12345";

        // Act
        var action = () => new Address(string.Empty, city, state, country, zipCode);

        // Assert
        action.Should().Throw<POS.Domain.Exceptions.DomainException>()
            .WithMessage("Street cannot be empty");
    }

    [Fact]
    public void Create_WithEmptyCity_ShouldThrowException()
    {
        // Arrange
        var street = "Street";
        var state = "State";
        var country = "Country";
        var zipCode = "12345";

        // Act
        var action = () => new Address(street, string.Empty, state, country, zipCode);

        // Assert
        action.Should().Throw<POS.Domain.Exceptions.DomainException>()
            .WithMessage("City cannot be empty");
    }

    [Fact]
    public void Create_WithEmptyState_ShouldThrowException()
    {
        // Arrange
        var street = "Street";
        var city = "City";
        var country = "Country";
        var zipCode = "12345";

        // Act
        var action = () => new Address(street, city, string.Empty, country, zipCode);

        // Assert
        action.Should().Throw<POS.Domain.Exceptions.DomainException>()
            .WithMessage("State cannot be empty");
    }

    [Fact]
    public void Create_WithEmptyZipCode_ShouldThrowException()
    {
        // Arrange
        var street = "Street";
        var city = "City";
        var state = "State";
        var country = "Country";

        // Act
        var action = () => new Address(street, city, state, country, string.Empty);

        // Assert
        action.Should().Throw<POS.Domain.Exceptions.DomainException>()
            .WithMessage("Zip code cannot be empty");
    }

    [Fact]
    public void Create_WithEmptyCountry_ShouldThrowException()
    {
        // Arrange
        var street = "Street";
        var city = "City";
        var state = "State";
        var zipCode = "12345";

        // Act
        var action = () => new Address(street, city, state, string.Empty, zipCode);

        // Assert
        action.Should().Throw<POS.Domain.Exceptions.DomainException>()
            .WithMessage("Country cannot be empty");
    }

    [Fact]
    public void Equals_WithSameValues_ShouldReturnTrue()
    {
        // Arrange
        var address1 = new Address("Street", "City", "State", "12345", "Country");
        var address2 = new Address("Street", "City", "State", "12345", "Country");

        // Act & Assert
        address1.Equals(address2).Should().BeTrue();
        (address1 == address2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentValues_ShouldReturnFalse()
    {
        // Arrange
        var address1 = TestDataFactory.ValueObjects.CreateValidAddress();
        var address2 = TestDataFactory.ValueObjects.CreateValidAddress();

        // Act & Assert
        address1.Equals(address2).Should().BeFalse();
        (address1 != address2).Should().BeTrue();
    }

    [Fact]
    public void ToString_ShouldReturnFormattedString()
    {
        // Arrange
        var address = TestDataFactory.ValueObjects.CreateValidAddress();

        // Act
        var result = address.ToString();

        // Assert
        result.Should().Be($"{address.Street}, {address.City}, {address.State} {address.ZipCode}, {address.Country}");
    }
} 
