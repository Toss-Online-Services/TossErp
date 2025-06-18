using POS.Domain.AggregatesModel.CustomerAggregate.ValueObjects;
using POS.Domain.Exceptions;
using Xunit;

namespace POS.Domain.Tests.AggregatesModel.CustomerAggregate.ValueObjects;

public class CustomerPreferencesTests
{
    [Fact]
    public void Create_WithValidParameters_CreatesPreferences()
    {
        // Arrange & Act
        var preferences = CustomerPreferences.Create(
            receiveEmailNotifications: true,
            receiveSMSNotifications: false,
            receivePostalMail: true,
            preferredLanguage: "es",
            preferredCurrency: "EUR",
            preferredPaymentMethod: "PayPal",
            preferredShippingMethod: "Express",
            optInMarketing: true,
            optInThirdParty: false,
            dietaryRestrictions: "Vegetarian",
            specialInstructions: "Handle with care");

        // Assert
        Assert.True(preferences.ReceiveEmailNotifications);
        Assert.False(preferences.ReceiveSMSNotifications);
        Assert.True(preferences.ReceivePostalMail);
        Assert.Equal("es", preferences.PreferredLanguage);
        Assert.Equal("EUR", preferences.PreferredCurrency);
        Assert.Equal("PayPal", preferences.PreferredPaymentMethod);
        Assert.Equal("Express", preferences.PreferredShippingMethod);
        Assert.True(preferences.OptInMarketing);
        Assert.False(preferences.OptInThirdParty);
        Assert.Equal("Vegetarian", preferences.DietaryRestrictions);
        Assert.Equal("Handle with care", preferences.SpecialInstructions);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_WithInvalidLanguage_ThrowsDomainException(string invalidLanguage)
    {
        // Arrange & Act & Assert
        Assert.Throws<DomainException>(() => CustomerPreferences.Create(preferredLanguage: invalidLanguage));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_WithInvalidCurrency_ThrowsDomainException(string invalidCurrency)
    {
        // Arrange & Act & Assert
        Assert.Throws<DomainException>(() => CustomerPreferences.Create(preferredCurrency: invalidCurrency));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_WithInvalidPaymentMethod_ThrowsDomainException(string invalidPaymentMethod)
    {
        // Arrange & Act & Assert
        Assert.Throws<DomainException>(() => CustomerPreferences.Create(preferredPaymentMethod: invalidPaymentMethod));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_WithInvalidShippingMethod_ThrowsDomainException(string invalidShippingMethod)
    {
        // Arrange & Act & Assert
        Assert.Throws<DomainException>(() => CustomerPreferences.Create(preferredShippingMethod: invalidShippingMethod));
    }

    [Fact]
    public void WithUpdatedPreferences_WithPartialUpdates_ReturnsNewPreferences()
    {
        // Arrange
        var originalPreferences = CustomerPreferences.Create(
            receiveEmailNotifications: true,
            receiveSMSNotifications: true,
            preferredLanguage: "en",
            preferredCurrency: "USD");

        // Act
        var updatedPreferences = originalPreferences.WithUpdatedPreferences(
            receiveEmailNotifications: false,
            preferredLanguage: "fr");

        // Assert
        Assert.False(updatedPreferences.ReceiveEmailNotifications);
        Assert.True(updatedPreferences.ReceiveSMSNotifications); // Unchanged
        Assert.Equal("fr", updatedPreferences.PreferredLanguage);
        Assert.Equal("USD", updatedPreferences.PreferredCurrency); // Unchanged
    }
} 
