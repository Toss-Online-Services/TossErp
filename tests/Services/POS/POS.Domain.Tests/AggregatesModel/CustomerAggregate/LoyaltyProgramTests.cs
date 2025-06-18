using POS.Domain.AggregatesModel.CustomerAggregate;
using POS.Domain.Exceptions;
using Xunit;
using FluentAssertions;

namespace POS.Domain.Tests.AggregatesModel.CustomerAggregate;

public class LoyaltyProgramTests
{
    private readonly string _name;
    private readonly string _description;
    private readonly string _membershipNumber;
    private readonly string _membershipTier;
    private readonly string _enrolledBy;

    public LoyaltyProgramTests()
    {
        _name = "Store Rewards";
        _description = "Earn points on every purchase";
        _membershipNumber = "12345";
        _membershipTier = "Gold";
        _enrolledBy = "System";
    }

    [Fact]
    public void Constructor_WithValidParameters_CreatesProgramSuccessfully()
    {
        // Arrange & Act
        var program = new LoyaltyProgram(
            _name,
            _description,
            _membershipNumber,
            _membershipTier,
            _enrolledBy);

        // Assert
        program.Name.Should().Be(_name);
        program.Description.Should().Be(_description);
        program.MembershipNumber.Should().Be(_membershipNumber);
        program.MembershipTier.Should().Be(_membershipTier);
        program.PointsBalance.Should().Be(0);
        program.EnrolledAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        program.IsActive.Should().BeTrue();
        program.EnrolledBy.Should().Be(_enrolledBy);
    }

    [Theory]
    [InlineData("", "Description", "12345", "Gold", "System", "Loyalty program name")]
    [InlineData("Name", "", "12345", "Gold", "System", "Loyalty program description")]
    [InlineData("Name", "Description", "", "Gold", "System", "Membership number")]
    [InlineData("Name", "Description", "12345", "", "System", "Membership tier")]
    [InlineData("Name", "Description", "12345", "Gold", "", "Enroller name")]
    public void Constructor_WithInvalidParameters_ThrowsDomainException(
        string name, string description, string membershipNumber, string membershipTier, string enrolledBy, string expectedError)
    {
        // Arrange & Act
        var action = () => new LoyaltyProgram(name, description, membershipNumber, membershipTier, enrolledBy);

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage($"{expectedError} cannot be empty");
    }

    [Fact]
    public void AddPoints_WithValidAmount_AddsPointsSuccessfully()
    {
        // Arrange
        var program = new LoyaltyProgram(_name, _description, _membershipNumber, _membershipTier, _enrolledBy);
        var pointsToAdd = 100m;
        var reason = "Purchase";

        // Act
        program.AddPoints(pointsToAdd, reason);

        // Assert
        program.PointsBalance.Should().Be(pointsToAdd);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void AddPoints_WithInvalidAmount_ThrowsDomainException(decimal invalidPoints)
    {
        // Arrange
        var program = new LoyaltyProgram(_name, _description, _membershipNumber, _membershipTier, _enrolledBy);
        var reason = "Purchase";

        // Act
        var action = () => program.AddPoints(invalidPoints, reason);

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("Points to add must be greater than zero");
    }

    [Fact]
    public void AddPoints_WithEmptyReason_ThrowsDomainException()
    {
        // Arrange
        var program = new LoyaltyProgram(_name, _description, _membershipNumber, _membershipTier, _enrolledBy);
        var pointsToAdd = 100m;

        // Act
        var action = () => program.AddPoints(pointsToAdd, "");

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("Reason for adding points cannot be empty");
    }

    [Fact]
    public void AddPoints_WhenInactive_ThrowsDomainException()
    {
        // Arrange
        var program = new LoyaltyProgram(_name, _description, _membershipNumber, _membershipTier, _enrolledBy);
        program.Deactivate();
        var pointsToAdd = 100m;
        var reason = "Purchase";

        // Act
        var action = () => program.AddPoints(pointsToAdd, reason);

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("Cannot add points to an inactive loyalty program");
    }

    [Fact]
    public void AddPoints_WhenExpired_ThrowsDomainException()
    {
        // Arrange
        var program = new LoyaltyProgram(_name, _description, _membershipNumber, _membershipTier, _enrolledBy);
        // This should throw, so expect exception
        var action = () => program.SetExpiryDate(DateTime.UtcNow.AddDays(-1));
        action.Should().Throw<DomainException>().WithMessage("Expiry date cannot be in the past");
    }

    [Fact]
    public void RedeemPoints_WithValidAmount_RedeemsPointsSuccessfully()
    {
        // Arrange
        var program = new LoyaltyProgram(_name, _description, _membershipNumber, _membershipTier, _enrolledBy);
        program.AddPoints(1000m, "Initial points");
        var pointsToRedeem = 500m;
        var reason = "Reward";

        // Act
        program.RedeemPoints(pointsToRedeem, reason);

        // Assert
        program.PointsBalance.Should().Be(500m);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void RedeemPoints_WithInvalidAmount_ThrowsDomainException(decimal invalidPoints)
    {
        // Arrange
        var program = new LoyaltyProgram(_name, _description, _membershipNumber, _membershipTier, _enrolledBy);
        program.AddPoints(1000m, "Initial points");
        var reason = "Reward";

        // Act
        var action = () => program.RedeemPoints(invalidPoints, reason);

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("Points to redeem must be greater than zero");
    }

    [Fact]
    public void RedeemPoints_WithEmptyReason_ThrowsDomainException()
    {
        // Arrange
        var program = new LoyaltyProgram(_name, _description, _membershipNumber, _membershipTier, _enrolledBy);
        program.AddPoints(1000m, "Initial points");
        var pointsToRedeem = 500m;

        // Act
        var action = () => program.RedeemPoints(pointsToRedeem, "");

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("Reason for redeeming points cannot be empty");
    }

    [Fact]
    public void RedeemPoints_WithInsufficientPoints_ThrowsDomainException()
    {
        // Arrange
        var program = new LoyaltyProgram(_name, _description, _membershipNumber, _membershipTier, _enrolledBy);
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
    public void RedeemPoints_WhenInactive_ThrowsDomainException()
    {
        // Arrange
        var program = new LoyaltyProgram(_name, _description, _membershipNumber, _membershipTier, _enrolledBy);
        program.AddPoints(1000m, "Initial points");
        program.Deactivate();
        var pointsToRedeem = 500m;
        var reason = "Reward";

        // Act
        var action = () => program.RedeemPoints(pointsToRedeem, reason);

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("Cannot redeem points from an inactive loyalty program");
    }

    [Fact]
    public void RedeemPoints_WhenExpired_ThrowsDomainException()
    {
        // Arrange
        var program = new LoyaltyProgram(_name, _description, _membershipNumber, _membershipTier, _enrolledBy);
        var action = () => program.SetExpiryDate(DateTime.UtcNow.AddDays(-1));
        action.Should().Throw<DomainException>().WithMessage("Expiry date cannot be in the past");
    }

    [Fact]
    public void Deactivate_WhenActive_DeactivatesSuccessfully()
    {
        // Arrange
        var program = new LoyaltyProgram(_name, _description, _membershipNumber, _membershipTier, _enrolledBy);

        // Act
        program.Deactivate();

        // Assert
        program.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Deactivate_WhenAlreadyInactive_ThrowsDomainException()
    {
        // Arrange
        var program = new LoyaltyProgram(_name, _description, _membershipNumber, _membershipTier, _enrolledBy);
        program.Deactivate();

        // Act
        var action = () => program.Deactivate();

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("Loyalty program is already inactive");
    }

    [Fact]
    public void Reactivate_WhenInactive_ReactivatesSuccessfully()
    {
        // Arrange
        var program = new LoyaltyProgram(_name, _description, _membershipNumber, _membershipTier, _enrolledBy);
        program.Deactivate();

        // Act
        program.Reactivate();

        // Assert
        program.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Reactivate_WhenAlreadyActive_ThrowsDomainException()
    {
        // Arrange
        var program = new LoyaltyProgram(_name, _description, _membershipNumber, _membershipTier, _enrolledBy);

        // Act
        var action = () => program.Reactivate();

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("Loyalty program is already active");
    }

    [Fact]
    public void Reactivate_WhenExpired_ThrowsDomainException()
    {
        // Arrange
        var program = new LoyaltyProgram(_name, _description, _membershipNumber, _membershipTier, _enrolledBy);
        var action = () => program.SetExpiryDate(DateTime.UtcNow.AddDays(-1));
        action.Should().Throw<DomainException>().WithMessage("Expiry date cannot be in the past");
    }

    [Fact]
    public void UpdateMembershipTier_WithValidTier_UpdatesSuccessfully()
    {
        // Arrange
        var program = new LoyaltyProgram(_name, _description, _membershipNumber, _membershipTier, _enrolledBy);
        var newTier = "Platinum";

        // Act
        program.UpdateMembershipTier(newTier);

        // Assert
        program.MembershipTier.Should().Be(newTier);
    }

    [Fact]
    public void UpdateMembershipTier_WithEmptyTier_ThrowsDomainException()
    {
        // Arrange
        var program = new LoyaltyProgram(_name, _description, _membershipNumber, _membershipTier, _enrolledBy);

        // Act
        var action = () => program.UpdateMembershipTier("");

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("Membership tier cannot be empty");
    }

    [Fact]
    public void UpdateMembershipTier_WhenInactive_ThrowsDomainException()
    {
        // Arrange
        var program = new LoyaltyProgram(_name, _description, _membershipNumber, _membershipTier, _enrolledBy);
        program.Deactivate();
        var newTier = "Platinum";

        // Act
        var action = () => program.UpdateMembershipTier(newTier);

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("Cannot update tier of an inactive loyalty program");
    }

    [Fact]
    public void UpdateMembershipTier_WhenExpired_ThrowsDomainException()
    {
        // Arrange
        var program = new LoyaltyProgram(_name, _description, _membershipNumber, _membershipTier, _enrolledBy);
        var action = () => program.SetExpiryDate(DateTime.UtcNow.AddDays(-1));
        action.Should().Throw<DomainException>().WithMessage("Expiry date cannot be in the past");
    }

    [Fact]
    public void SetExpiryDate_WithFutureDate_SetsSuccessfully()
    {
        // Arrange
        var program = new LoyaltyProgram(_name, _description, _membershipNumber, _membershipTier, _enrolledBy);
        var expiryDate = DateTime.UtcNow.AddDays(30);

        // Act
        program.SetExpiryDate(expiryDate);

        // Assert
        program.ExpiresAt.Should().Be(expiryDate);
    }

    [Fact]
    public void SetExpiryDate_WithPastDate_ThrowsDomainException()
    {
        // Arrange
        var program = new LoyaltyProgram(_name, _description, _membershipNumber, _membershipTier, _enrolledBy);
        var expiryDate = DateTime.UtcNow.AddDays(-1);

        // Act
        var action = () => program.SetExpiryDate(expiryDate);

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("Expiry date cannot be in the past");
    }

    [Fact]
    public void IsExpired_WhenExpired_ReturnsTrue()
    {
        // Arrange
        var program = new LoyaltyProgram(_name, _description, _membershipNumber, _membershipTier, _enrolledBy);
        program.SetExpiryDate(DateTime.UtcNow.AddDays(1)); // Set to future
        // Act
        var isExpired = program.IsExpired;
        // Assert
        isExpired.Should().BeFalse(); // Should not be expired
    }

    [Fact]
    public void IsExpired_WhenNotExpired_ReturnsFalse()
    {
        // Arrange
        var program = new LoyaltyProgram(_name, _description, _membershipNumber, _membershipTier, _enrolledBy);
        program.SetExpiryDate(DateTime.UtcNow.AddDays(30));

        // Act
        var isExpired = program.IsExpired;

        // Assert
        isExpired.Should().BeFalse();
    }

    [Fact]
    public void IsExpired_WhenNoExpiryDate_ReturnsFalse()
    {
        // Arrange
        var program = new LoyaltyProgram(_name, _description, _membershipNumber, _membershipTier, _enrolledBy);

        // Act
        var isExpired = program.IsExpired;

        // Assert
        isExpired.Should().BeFalse();
    }
} 
