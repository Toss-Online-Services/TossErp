using POS.Domain.AggregatesModel.CustomerAggregate;
using POS.Domain.Exceptions;
using Xunit;
using POS.Domain.Common;
using FluentAssertions;

namespace POS.Domain.Tests.AggregatesModel.CustomerAggregate.ValueObjects;

public class LoyaltyProgramTests
{
    [Fact]
    public void Create_WithValidParameters_CreatesLoyaltyProgram()
    {
        // Arrange
        var name = "Test Program";
        var description = "Test Description";
        var membershipNumber = "TEST123";
        var membershipTier = "Bronze";
        var enrolledBy = "Test User";

        // Act
        var loyaltyProgram = new LoyaltyProgram(name, description, membershipNumber, membershipTier, enrolledBy);

        // Assert
        Assert.Equal(name, loyaltyProgram.Name);
        Assert.Equal(description, loyaltyProgram.Description);
        Assert.Equal(membershipNumber, loyaltyProgram.MembershipNumber);
        Assert.Equal(membershipTier, loyaltyProgram.MembershipTier);
        Assert.Equal(0, loyaltyProgram.PointsBalance);
        Assert.True(loyaltyProgram.IsActive);
        Assert.Equal(enrolledBy, loyaltyProgram.EnrolledBy);
        Assert.False(loyaltyProgram.IsExpired);
    }

    [Theory]
    [InlineData("", "Description", "TEST123", "Bronze", "Test User", "Loyalty program name cannot be empty")]
    [InlineData("Test Program", "", "TEST123", "Bronze", "Test User", "Loyalty program description cannot be empty")]
    [InlineData("Test Program", "Description", "", "Bronze", "Test User", "Membership number cannot be empty")]
    [InlineData("Test Program", "Description", "TEST123", "", "Test User", "Membership tier cannot be empty")]
    [InlineData("Test Program", "Description", "TEST123", "Bronze", "", "Enroller name cannot be empty")]
    public void Create_WithInvalidParameters_ThrowsDomainException(
        string name, string description, string membershipNumber, string membershipTier, string enrolledBy, string expectedMessage)
    {
        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            new LoyaltyProgram(name, description, membershipNumber, membershipTier, enrolledBy));
        Assert.Equal(expectedMessage, exception.Message);
    }

    [Fact]
    public void AddPoints_WithValidAmount_AddsPoints()
    {
        // Arrange
        var loyaltyProgram = new LoyaltyProgram("Test", "Test", "TEST123", "Bronze", "Test User");
        var points = 100m;
        var reason = "Test points";

        // Act
        loyaltyProgram.AddPoints(points, reason);

        // Assert
        Assert.Equal(points, loyaltyProgram.PointsBalance);
    }

    [Theory]
    [InlineData(0, "Test reason", "Points to add must be greater than zero")]
    [InlineData(-100, "Test reason", "Points to add must be greater than zero")]
    [InlineData(100, "", "Reason for adding points cannot be empty")]
    public void AddPoints_WithInvalidParameters_ThrowsDomainException(
        decimal points, string reason, string expectedMessage)
    {
        // Arrange
        var loyaltyProgram = new LoyaltyProgram("Test", "Test", "TEST123", "Bronze", "Test User");

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => loyaltyProgram.AddPoints(points, reason));
        Assert.Equal(expectedMessage, exception.Message);
    }

    [Fact]
    public void AddPoints_WhenInactive_ThrowsDomainException()
    {
        // Arrange
        var loyaltyProgram = new LoyaltyProgram("Test", "Test", "TEST123", "Bronze", "Test User");
        loyaltyProgram.Deactivate();

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            loyaltyProgram.AddPoints(100, "Test reason"));
        Assert.Equal("Cannot add points to an inactive loyalty program", exception.Message);
    }

    [Fact]
    public void AddPoints_WhenExpired_ThrowsDomainException()
    {
        // Arrange
        var fakeTime = new FakeTimeProvider { UtcNow = DateTime.UtcNow };
        var expiry = fakeTime.UtcNow.AddDays(1);
        var program = new LoyaltyProgram("Test", "Test", "TEST123", "Bronze", "Test User", expiry, fakeTime);
        // Advance time to after expiry
        fakeTime.UtcNow = expiry.AddSeconds(1);
        var pointsToAdd = 100m;
        var reason = "Purchase";

        // Act
        var action = () => program.AddPoints(pointsToAdd, reason);

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("Cannot add points to an expired loyalty program");
    }

    [Fact]
    public void RedeemPoints_WithValidAmount_RedeemsPoints()
    {
        // Arrange
        var loyaltyProgram = new LoyaltyProgram("Test", "Test", "TEST123", "Bronze", "Test User");
        loyaltyProgram.AddPoints(100, "Initial points");
        var points = 50m;
        var reason = "Test redemption";

        // Act
        loyaltyProgram.RedeemPoints(points, reason);

        // Assert
        Assert.Equal(50m, loyaltyProgram.PointsBalance);
    }

    [Theory]
    [InlineData(0, "Test reason", "Points to redeem must be greater than zero")]
    [InlineData(-100, "Test reason", "Points to redeem must be greater than zero")]
    [InlineData(100, "", "Reason for redeeming points cannot be empty")]
    public void RedeemPoints_WithInvalidParameters_ThrowsDomainException(
        decimal points, string reason, string expectedMessage)
    {
        // Arrange
        var loyaltyProgram = new LoyaltyProgram("Test", "Test", "TEST123", "Bronze", "Test User");
        loyaltyProgram.AddPoints(50, "Initial points");

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => loyaltyProgram.RedeemPoints(points, reason));
        Assert.Equal(expectedMessage, exception.Message);
    }

    [Fact]
    public void RedeemPoints_WithInsufficientBalance_ThrowsDomainException()
    {
        // Arrange
        var loyaltyProgram = new LoyaltyProgram("Test", "Test", "TEST123", "Bronze", "Test User");
        loyaltyProgram.AddPoints(50, "Initial points");

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            loyaltyProgram.RedeemPoints(100, "Test reason"));
        Assert.Equal("Insufficient points balance", exception.Message);
    }

    [Fact]
    public void RedeemPoints_WhenInactive_ThrowsDomainException()
    {
        // Arrange
        var loyaltyProgram = new LoyaltyProgram("Test", "Test", "TEST123", "Bronze", "Test User");
        loyaltyProgram.AddPoints(100, "Initial points");
        loyaltyProgram.Deactivate();

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            loyaltyProgram.RedeemPoints(50, "Test reason"));
        Assert.Equal("Cannot redeem points from an inactive loyalty program", exception.Message);
    }

    [Fact]
    public void RedeemPoints_WhenExpired_ThrowsDomainException()
    {
        // Arrange
        var fakeTime = new FakeTimeProvider { UtcNow = DateTime.UtcNow };
        var expiry = fakeTime.UtcNow.AddDays(1);
        var program = new LoyaltyProgram("Test", "Test", "TEST123", "Bronze", "Test User", expiry, fakeTime);
        // Advance time to after expiry
        fakeTime.UtcNow = expiry.AddSeconds(1);
        var pointsToRedeem = 100m;
        var reason = "Reward";

        // Act
        var action = () => program.RedeemPoints(pointsToRedeem, reason);

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("Cannot redeem points from an expired loyalty program");
    }

    [Fact]
    public void Deactivate_WhenActive_DeactivatesProgram()
    {
        // Arrange
        var loyaltyProgram = new LoyaltyProgram("Test", "Test", "TEST123", "Bronze", "Test User");

        // Act
        loyaltyProgram.Deactivate();

        // Assert
        Assert.False(loyaltyProgram.IsActive);
    }

    [Fact]
    public void Deactivate_WhenAlreadyInactive_ThrowsDomainException()
    {
        // Arrange
        var loyaltyProgram = new LoyaltyProgram("Test", "Test", "TEST123", "Bronze", "Test User");
        loyaltyProgram.Deactivate();

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => loyaltyProgram.Deactivate());
        Assert.Equal("Loyalty program is already inactive", exception.Message);
    }

    [Fact]
    public void Reactivate_WhenInactive_ReactivatesProgram()
    {
        // Arrange
        var loyaltyProgram = new LoyaltyProgram("Test", "Test", "TEST123", "Bronze", "Test User");
        loyaltyProgram.Deactivate();

        // Act
        loyaltyProgram.Reactivate();

        // Assert
        Assert.True(loyaltyProgram.IsActive);
    }

    [Fact]
    public void Reactivate_WhenAlreadyActive_ThrowsDomainException()
    {
        // Arrange
        var loyaltyProgram = new LoyaltyProgram("Test", "Test", "TEST123", "Bronze", "Test User");

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => loyaltyProgram.Reactivate());
        Assert.Equal("Loyalty program is already active", exception.Message);
    }

    [Fact]
    public void Reactivate_WhenExpired_ThrowsDomainException()
    {
        // Arrange
        var fakeTime = new FakeTimeProvider { UtcNow = DateTime.UtcNow };
        var expiry = fakeTime.UtcNow.AddDays(1);
        var program = new LoyaltyProgram("Test", "Test", "TEST123", "Bronze", "Test User", expiry, fakeTime);
        // Advance time to after expiry
        fakeTime.UtcNow = expiry.AddSeconds(1);
        program.Deactivate();

        // Act
        var action = () => program.Reactivate();

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("Cannot reactivate an expired loyalty program");
    }

    [Fact]
    public void UpdateMembershipTier_WithValidTier_UpdatesTier()
    {
        // Arrange
        var loyaltyProgram = new LoyaltyProgram("Test", "Test", "TEST123", "Bronze", "Test User");
        var newTier = "Silver";

        // Act
        loyaltyProgram.UpdateMembershipTier(newTier);

        // Assert
        Assert.Equal(newTier, loyaltyProgram.MembershipTier);
    }

    [Theory]
    [InlineData("")]
    public void UpdateMembershipTier_WithInvalidTier_ThrowsDomainException(string newTier)
    {
        // Arrange
        var loyaltyProgram = new LoyaltyProgram("Test", "Test", "TEST123", "Bronze", "Test User");

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => loyaltyProgram.UpdateMembershipTier(newTier));
        Assert.Equal("Membership tier cannot be empty", exception.Message);
    }

    [Fact]
    public void UpdateMembershipTier_WhenInactive_ThrowsDomainException()
    {
        // Arrange
        var loyaltyProgram = new LoyaltyProgram("Test", "Test", "TEST123", "Bronze", "Test User");
        loyaltyProgram.Deactivate();

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            loyaltyProgram.UpdateMembershipTier("Silver"));
        Assert.Equal("Cannot update tier of an inactive loyalty program", exception.Message);
    }

    [Fact]
    public void UpdateMembershipTier_WhenExpired_ThrowsDomainException()
    {
        // Arrange
        var fakeTime = new FakeTimeProvider { UtcNow = DateTime.UtcNow };
        var expiry = fakeTime.UtcNow.AddDays(1);
        var program = new LoyaltyProgram("Test", "Test", "TEST123", "Bronze", "Test User", expiry, fakeTime);
        // Advance time to after expiry
        fakeTime.UtcNow = expiry.AddSeconds(1);
        var newTier = "Platinum";

        // Act
        var action = () => program.UpdateMembershipTier(newTier);

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("Cannot update tier of an expired loyalty program");
    }

    [Fact]
    public void SetExpiryDate_WithFutureDate_SetsExpiryDate()
    {
        // Arrange
        var loyaltyProgram = new LoyaltyProgram("Test", "Test", "TEST123", "Bronze", "Test User");
        var expiryDate = DateTime.UtcNow.AddDays(30);

        // Act
        loyaltyProgram.SetExpiryDate(expiryDate);

        // Assert
        Assert.Equal(expiryDate, loyaltyProgram.ExpiresAt);
        Assert.False(loyaltyProgram.IsExpired);
    }

    [Fact]
    public void SetExpiryDate_WithPastDate_ThrowsDomainException()
    {
        // Arrange
        var fakeTime = new FakeTimeProvider { UtcNow = DateTime.UtcNow };
        var program = new LoyaltyProgram("Test", "Test", "TEST123", "Bronze", "Test User", null, fakeTime);
        var pastDate = fakeTime.UtcNow.AddDays(-1);

        // Act
        var action = () => program.SetExpiryDate(pastDate);

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("Expiry date cannot be in the past");
    }

    [Fact]
    public void SetExpiryDate_WithNull_ClearsExpiryDate()
    {
        // Arrange
        var loyaltyProgram = new LoyaltyProgram("Test", "Test", "TEST123", "Bronze", "Test User");
        loyaltyProgram.SetExpiryDate(DateTime.UtcNow.AddDays(30));

        // Act
        loyaltyProgram.SetExpiryDate(null);

        // Assert
        Assert.Null(loyaltyProgram.ExpiresAt);
        Assert.False(loyaltyProgram.IsExpired);
    }
}

// Add a fake time provider for testing
public class FakeTimeProvider : ITimeProvider
{
    public DateTime UtcNow { get; set; }
} 
