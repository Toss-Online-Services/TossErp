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
        var street = string.Empty;
        var city = "Test City";
        var state = "Test State";
        var zipCode = "12345";
        var country = "Test Country";

        // Act & Assert
        var action = () => new Address(street, city, state, zipCode, country);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Street cannot be empty");
    }

    [Fact]
    public void Create_WithEmptyCity_ShouldThrowException()
    {
        // Arrange
        var street = "123 Test St";
        var city = string.Empty;
        var state = "Test State";
        var zipCode = "12345";
        var country = "Test Country";

        // Act & Assert
        var action = () => new Address(street, city, state, zipCode, country);
        action.Should().Throw<ArgumentException>()
            .WithMessage("City cannot be empty");
    }

    [Fact]
    public void Create_WithEmptyState_ShouldThrowException()
    {
        // Arrange
        var street = "123 Test St";
        var city = "Test City";
        var state = string.Empty;
        var zipCode = "12345";
        var country = "Test Country";

        // Act & Assert
        var action = () => new Address(street, city, state, zipCode, country);
        action.Should().Throw<ArgumentException>()
            .WithMessage("State cannot be empty");
    }

    [Fact]
    public void Create_WithEmptyZipCode_ShouldThrowException()
    {
        // Arrange
        var street = "123 Test St";
        var city = "Test City";
        var state = "Test State";
        var zipCode = string.Empty;
        var country = "Test Country";

        // Act & Assert
        var action = () => new Address(street, city, state, zipCode, country);
        action.Should().Throw<ArgumentException>()
            .WithMessage("ZipCode cannot be empty");
    }

    [Fact]
    public void Create_WithEmptyCountry_ShouldThrowException()
    {
        // Arrange
        var street = "123 Test St";
        var city = "Test City";
        var state = "Test State";
        var zipCode = "12345";
        var country = string.Empty;

        // Act & Assert
        var action = () => new Address(street, city, state, zipCode, country);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Country cannot be empty");
    }

    [Fact]
    public void Equals_WithSameValues_ShouldReturnTrue()
    {
        // Arrange
        var address1 = TestDataFactory.ValueObjects.CreateValidAddress();
        var address2 = new Address(
            address1.Street,
            address1.City,
            address1.State,
            address1.ZipCode,
            address1.Country
        );

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
