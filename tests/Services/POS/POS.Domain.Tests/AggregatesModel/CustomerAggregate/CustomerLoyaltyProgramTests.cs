using POS.Domain.AggregatesModel.CustomerAggregate;
using POS.Domain.Exceptions;
using Xunit;
using FluentAssertions;

namespace POS.Domain.Tests.AggregatesModel.CustomerAggregate;

public class CustomerLoyaltyProgramTests
{
    private readonly string _programName;
    private readonly string _membershipNumber;
    private readonly string _membershipTier;

    public CustomerLoyaltyProgramTests()
    {
        _programName = "Gold Rewards";
        _membershipNumber = "GOLD123";
        _membershipTier = "Gold";
    }

    [Fact]
    public void Constructor_WithValidParameters_CreatesLoyaltyProgramSuccessfully()
    {
        // Arrange & Act
        var program = new CustomerLoyaltyProgram(
            _programName,
            _membershipNumber,
            _membershipTier);

        // Assert
        program.ProgramName.Should().Be(_programName);
        program.MembershipNumber.Should().Be(_membershipNumber);
        program.MembershipTier.Should().Be(_membershipTier);
        program.PointsBalance.Should().Be(0);
        program.LifetimePoints.Should().Be(0);
        program.IsActive.Should().BeTrue();
        program.EnrollmentDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        program.ExpiryDate.Should().BeNull();
        program.ReferralCode.Should().BeNull();
        program.ReferralCount.Should().Be(0);
    }

    [Fact]
    public void Constructor_WithEmptyProgramName_ThrowsDomainException()
    {
        // Act
        var action = () => new CustomerLoyaltyProgram("", _membershipNumber, _membershipTier);

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("Program name cannot be empty");
    }

    [Fact]
    public void Constructor_WithEmptyMembershipNumber_ThrowsDomainException()
    {
        // Act
        var action = () => new CustomerLoyaltyProgram(_programName, "", _membershipTier);

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("Membership number cannot be empty");
    }

    [Fact]
    public void Constructor_WithEmptyMembershipTier_ThrowsDomainException()
    {
        // Act
        var action = () => new CustomerLoyaltyProgram(_programName, _membershipNumber, "");

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("Membership tier cannot be empty");
    }

    [Fact]
    public void AddPoints_WithValidAmount_AddsPointsSuccessfully()
    {
        // Arrange
        var program = new CustomerLoyaltyProgram(_programName, _membershipNumber, _membershipTier);
        var pointsToAdd = 100m;
        var reason = "Purchase";

        // Act
        program.AddPoints(pointsToAdd, reason);

        // Assert
        program.PointsBalance.Should().Be(pointsToAdd);
        program.LifetimePoints.Should().Be(pointsToAdd);
        program.LastPointsEarned.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void AddPoints_WithNegativeAmount_ThrowsDomainException()
    {
        // Arrange
        var program = new CustomerLoyaltyProgram(_programName, _membershipNumber, _membershipTier);
        var pointsToAdd = -100m;
        var reason = "Purchase";

        // Act
        var action = () => program.AddPoints(pointsToAdd, reason);

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("Points must be greater than zero");
    }

    [Fact]
    public void AddPoints_WithEmptyReason_ThrowsDomainException()
    {
        // Arrange
        var program = new CustomerLoyaltyProgram(_programName, _membershipNumber, _membershipTier);
        var pointsToAdd = 100m;

        // Act
        var action = () => program.AddPoints(pointsToAdd, "");

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("Reason for points cannot be empty");
    }

    [Fact]
    public void RedeemPoints_WithValidAmount_RedeemsPointsSuccessfully()
    {
        // Arrange
        var program = new CustomerLoyaltyProgram(_programName, _membershipNumber, _membershipTier);
        program.AddPoints(1000m, "Initial points");
        var pointsToRedeem = 500m;
        var reason = "Reward";

        // Act
        program.RedeemPoints(pointsToRedeem, reason);

        // Assert
        program.PointsBalance.Should().Be(500m);
        program.LifetimePoints.Should().Be(1000m);
        program.LastPointsRedeemed.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void RedeemPoints_WithInsufficientPoints_ThrowsDomainException()
    {
        // Arrange
        var program = new CustomerLoyaltyProgram(_programName, _membershipNumber, _membershipTier);
        program.AddPoints(100m, "Initial points");
        var pointsToRedeem = 500m;
        var reason = "Reward";

        // Act
        var action = () => program.RedeemPoints(pointsToRedeem, reason);

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("Insufficient points balance");
    }

    [Fact]
    public void RedeemPoints_WithEmptyReason_ThrowsDomainException()
    {
        // Arrange
        var program = new CustomerLoyaltyProgram(_programName, _membershipNumber, _membershipTier);
        program.AddPoints(100m, "Initial points");
        var pointsToRedeem = 50m;

        // Act
        var action = () => program.RedeemPoints(pointsToRedeem, "");

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("Reason for redemption cannot be empty");
    }

    [Fact]
    public void UpdateMembershipTier_WithValidTier_UpdatesSuccessfully()
    {
        // Arrange
        var program = new CustomerLoyaltyProgram(_programName, _membershipNumber, _membershipTier);
        var newTier = "Platinum";

        // Act
        program.UpdateMembershipTier(newTier);

        // Assert
        program.MembershipTier.Should().Be(newTier);
        program.LastModifiedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UpdateMembershipTier_WithEmptyTier_ThrowsDomainException()
    {
        // Arrange
        var program = new CustomerLoyaltyProgram(_programName, _membershipNumber, _membershipTier);

        // Act
        var action = () => program.UpdateMembershipTier("");

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("Membership tier cannot be empty");
    }

    [Fact]
    public void UpdateExpiryDate_WithValidDate_UpdatesSuccessfully()
    {
        // Arrange
        var program = new CustomerLoyaltyProgram(_programName, _membershipNumber, _membershipTier);
        var expiryDate = DateTime.UtcNow.AddYears(1);

        // Act
        program.UpdateExpiryDate(expiryDate);

        // Assert
        program.ExpiryDate.Should().Be(expiryDate);
        program.LastModifiedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Deactivate_WhenActive_DeactivatesSuccessfully()
    {
        // Arrange
        var program = new CustomerLoyaltyProgram(_programName, _membershipNumber, _membershipTier);

        // Act
        program.Deactivate();

        // Assert
        program.IsActive.Should().BeFalse();
        program.LastModifiedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Reactivate_WhenInactive_ReactivatesSuccessfully()
    {
        // Arrange
        var program = new CustomerLoyaltyProgram(_programName, _membershipNumber, _membershipTier);
        program.Deactivate();

        // Act
        program.Reactivate();

        // Assert
        program.IsActive.Should().BeTrue();
        program.LastModifiedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void IncrementReferralCount_IncrementsSuccessfully()
    {
        // Arrange
        var program = new CustomerLoyaltyProgram(_programName, _membershipNumber, _membershipTier);
        var initialCount = program.ReferralCount;

        // Act
        program.IncrementReferralCount();

        // Assert
        program.ReferralCount.Should().Be(initialCount + 1);
        program.LastModifiedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void IsExpired_WhenExpiryDateIsPast_ReturnsTrue()
    {
        // Arrange
        var program = new CustomerLoyaltyProgram(_programName, _membershipNumber, _membershipTier);
        program.UpdateExpiryDate(DateTime.UtcNow.AddDays(-1));

        // Act & Assert
        program.IsExpired.Should().BeTrue();
    }

    [Fact]
    public void IsExpired_WhenExpiryDateIsFuture_ReturnsFalse()
    {
        // Arrange
        var program = new CustomerLoyaltyProgram(_programName, _membershipNumber, _membershipTier);
        program.UpdateExpiryDate(DateTime.UtcNow.AddDays(1));

        // Act & Assert
        program.IsExpired.Should().BeFalse();
    }

    [Fact]
    public void IsExpired_WhenNoExpiryDate_ReturnsFalse()
    {
        // Arrange
        var program = new CustomerLoyaltyProgram(_programName, _membershipNumber, _membershipTier);

        // Act & Assert
        program.IsExpired.Should().BeFalse();
    }

    [Fact]
    public void Create_WithValidParameters_CreatesLoyaltyProgram()
    {
        // Arrange
        var programName = "Test Program";
        var membershipNumber = "TEST123";
        var membershipTier = "Bronze";
        var referralCode = "REF123";

        // Act
        var loyaltyProgram = new CustomerLoyaltyProgram(
            programName,
            membershipNumber,
            membershipTier,
            referralCode: referralCode);

        // Assert
        Assert.NotEqual(Guid.Empty, loyaltyProgram.Id);
        Assert.Equal(programName, loyaltyProgram.ProgramName);
        Assert.Equal(membershipNumber, loyaltyProgram.MembershipNumber);
        Assert.Equal(membershipTier, loyaltyProgram.MembershipTier);
        Assert.Equal(0, loyaltyProgram.PointsBalance);
        Assert.Equal(0, loyaltyProgram.LifetimePoints);
        Assert.True(loyaltyProgram.IsActive);
        Assert.Equal(referralCode, loyaltyProgram.ReferralCode);
        Assert.Equal(0, loyaltyProgram.ReferralCount);
        Assert.NotEqual(default, loyaltyProgram.CreatedAt);
        Assert.Null(loyaltyProgram.LastModifiedAt);
    }

    [Theory]
    [InlineData("", "TEST123", "Bronze", "Program name cannot be empty")]
    [InlineData("Test Program", "", "Bronze", "Membership number cannot be empty")]
    [InlineData("Test Program", "TEST123", "", "Membership tier cannot be empty")]
    public void Create_WithInvalidParameters_ThrowsDomainException(
        string programName, string membershipNumber, string membershipTier, string expectedMessage)
    {
        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            new CustomerLoyaltyProgram(programName, membershipNumber, membershipTier));
        Assert.Equal(expectedMessage, exception.Message);
    }

    [Fact]
    public void AddPoints_WithValidAmount_AddsPoints()
    {
        // Arrange
        var loyaltyProgram = new CustomerLoyaltyProgram("Test", "TEST123", "Bronze");
        var points = 100m;
        var reason = "Test points";

        // Act
        loyaltyProgram.AddPoints(points, reason);

        // Assert
        Assert.Equal(points, loyaltyProgram.PointsBalance);
        Assert.Equal(points, loyaltyProgram.LifetimePoints);
        Assert.NotEqual(default, loyaltyProgram.LastPointsEarned);
        Assert.NotEqual(default, loyaltyProgram.LastModifiedAt);
    }

    [Theory]
    [InlineData(0, "Test reason", "Points must be greater than zero")]
    [InlineData(-100, "Test reason", "Points must be greater than zero")]
    [InlineData(100, "", "Reason for points cannot be empty")]
    public void AddPoints_WithInvalidParameters_ThrowsDomainException(
        decimal points, string reason, string expectedMessage)
    {
        // Arrange
        var loyaltyProgram = new CustomerLoyaltyProgram("Test", "TEST123", "Bronze");

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => loyaltyProgram.AddPoints(points, reason));
        Assert.Equal(expectedMessage, exception.Message);
    }

    [Fact]
    public void RedeemPoints_WithValidAmount_RedeemsPoints()
    {
        // Arrange
        var loyaltyProgram = new CustomerLoyaltyProgram("Test", "TEST123", "Bronze");
        loyaltyProgram.AddPoints(100, "Initial points");
        var points = 50m;
        var reason = "Test redemption";

        // Act
        loyaltyProgram.RedeemPoints(points, reason);

        // Assert
        Assert.Equal(50m, loyaltyProgram.PointsBalance);
        Assert.Equal(100m, loyaltyProgram.LifetimePoints);
        Assert.NotEqual(default, loyaltyProgram.LastPointsRedeemed);
        Assert.NotEqual(default, loyaltyProgram.LastModifiedAt);
    }

    [Theory]
    [InlineData(100, "", "Reason for redemption cannot be empty")]
    public void RedeemPoints_WithInvalidParameters_ThrowsDomainException(decimal points, string reason, string expectedMessage)
    {
        // Arrange
        var program = new CustomerLoyaltyProgram("Test", "TEST123", "Bronze");
        program.AddPoints(200, "Initial points"); // Ensure sufficient balance

        // Act
        var exception = Assert.Throws<DomainException>(() => program.RedeemPoints(points, reason));

        // Assert
        Assert.Equal(expectedMessage, exception.Message);
    }

    [Fact]
    public void RedeemPoints_WithInsufficientBalance_ThrowsDomainException()
    {
        // Arrange
        var loyaltyProgram = new CustomerLoyaltyProgram("Test", "TEST123", "Bronze");
        loyaltyProgram.AddPoints(50, "Initial points");

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            loyaltyProgram.RedeemPoints(100, "Test reason"));
        Assert.Equal("Insufficient points balance", exception.Message);
    }

    [Fact]
    public void UpdateMembershipTier_WithValidTier_UpdatesTier()
    {
        // Arrange
        var loyaltyProgram = new CustomerLoyaltyProgram("Test", "TEST123", "Bronze");
        var newTier = "Silver";

        // Act
        loyaltyProgram.UpdateMembershipTier(newTier);

        // Assert
        Assert.Equal(newTier, loyaltyProgram.MembershipTier);
        Assert.NotEqual(default, loyaltyProgram.LastModifiedAt);
    }

    [Theory]
    [InlineData("")]
    public void UpdateMembershipTier_WithInvalidTier_ThrowsDomainException(string newTier)
    {
        // Arrange
        var program = new CustomerLoyaltyProgram(_programName, _membershipNumber, _membershipTier);

        // Act
        var action = () => program.UpdateMembershipTier(newTier);

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("Membership tier cannot be empty");
    }

    [Fact]
    public void UpdateExpiryDate_WithValidDate_UpdatesExpiryDate()
    {
        // Arrange
        var loyaltyProgram = new CustomerLoyaltyProgram("Test", "TEST123", "Bronze");
        var expiryDate = DateTime.UtcNow.AddDays(30);

        // Act
        loyaltyProgram.UpdateExpiryDate(expiryDate);

        // Assert
        Assert.Equal(expiryDate, loyaltyProgram.ExpiryDate);
        Assert.NotEqual(default, loyaltyProgram.LastModifiedAt);
    }

    [Fact]
    public void UpdateExpiryDate_WithNull_ClearsExpiryDate()
    {
        // Arrange
        var loyaltyProgram = new CustomerLoyaltyProgram("Test", "TEST123", "Bronze");
        loyaltyProgram.UpdateExpiryDate(DateTime.UtcNow.AddDays(30));

        // Act
        loyaltyProgram.UpdateExpiryDate(null);

        // Assert
        Assert.Null(loyaltyProgram.ExpiryDate);
        Assert.NotEqual(default, loyaltyProgram.LastModifiedAt);
    }

    [Fact]
    public void Deactivate_SetsIsActiveToFalse()
    {
        // Arrange
        var loyaltyProgram = new CustomerLoyaltyProgram("Test", "TEST123", "Bronze");

        // Act
        loyaltyProgram.Deactivate();

        // Assert
        Assert.False(loyaltyProgram.IsActive);
        Assert.NotEqual(default, loyaltyProgram.LastModifiedAt);
    }

    [Fact]
    public void Reactivate_SetsIsActiveToTrue()
    {
        // Arrange
        var loyaltyProgram = new CustomerLoyaltyProgram("Test", "TEST123", "Bronze");
        loyaltyProgram.Deactivate();

        // Act
        loyaltyProgram.Reactivate();

        // Assert
        Assert.True(loyaltyProgram.IsActive);
        Assert.NotEqual(default, loyaltyProgram.LastModifiedAt);
    }

    [Fact]
    public void IncrementReferralCount_IncrementsCount()
    {
        // Arrange
        var loyaltyProgram = new CustomerLoyaltyProgram("Test", "TEST123", "Bronze");

        // Act
        loyaltyProgram.IncrementReferralCount();

        // Assert
        Assert.Equal(1, loyaltyProgram.ReferralCount);
        Assert.NotEqual(default, loyaltyProgram.LastModifiedAt);
    }

    [Fact]
    public void IsExpired_WithFutureExpiryDate_ReturnsFalse()
    {
        // Arrange
        var loyaltyProgram = new CustomerLoyaltyProgram("Test", "TEST123", "Bronze");
        loyaltyProgram.UpdateExpiryDate(DateTime.UtcNow.AddDays(30));

        // Act & Assert
        Assert.False(loyaltyProgram.IsExpired);
    }

    [Fact]
    public void IsExpired_WithPastExpiryDate_ReturnsTrue()
    {
        // Arrange
        var loyaltyProgram = new CustomerLoyaltyProgram("Test", "TEST123", "Bronze");
        loyaltyProgram.UpdateExpiryDate(DateTime.UtcNow.AddDays(-1));

        // Act & Assert
        Assert.True(loyaltyProgram.IsExpired);
    }

    [Fact]
    public void IsExpired_WithNoExpiryDate_ReturnsFalse()
    {
        // Arrange
        var loyaltyProgram = new CustomerLoyaltyProgram("Test", "TEST123", "Bronze");

        // Act & Assert
        Assert.False(loyaltyProgram.IsExpired);
    }
} 
