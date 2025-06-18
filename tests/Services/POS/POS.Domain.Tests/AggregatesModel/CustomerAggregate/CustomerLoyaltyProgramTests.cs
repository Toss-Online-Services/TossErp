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
} 
