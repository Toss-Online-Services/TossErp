using POS.Domain.AggregatesModel.CustomerAggregate;
using POS.Domain.Common.ValueObjects;
using POS.Domain.Enums;
using POS.Domain.Exceptions;
using POS.Domain.Tests.Common;
using Xunit;
using FluentAssertions;

namespace POS.Domain.Tests.AggregatesModel.CustomerAggregate;

public class CustomerTests
{
    private readonly Guid _storeId;
    private readonly string _firstName;
    private readonly string _lastName;
    private readonly string _email;
    private readonly string _phoneNumber;

    public CustomerTests()
    {
        _storeId = Guid.NewGuid();
        _firstName = "John";
        _lastName = "Doe";
        _email = "john.doe@example.com";
        _phoneNumber = "1234567890";
    }

    [Fact]
    public void Constructor_WithValidParameters_CreatesCustomerSuccessfully()
    {
        // Arrange & Act
        var customer = new Customer(_firstName, _lastName, _email, _phoneNumber);

        // Assert
        customer.FirstName.Should().Be(_firstName);
        customer.LastName.Should().Be(_lastName);
        customer.Email.Should().Be(_email);
        customer.PhoneNumber.Should().Be(_phoneNumber);
        customer.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        customer.IsActive.Should().BeTrue();
        customer.CustomerType.Should().Be(CustomerType.Regular);
        customer.PriceLists.Should().BeEmpty();
        customer.Contacts.Should().BeEmpty();
        customer.Documents.Should().BeEmpty();
        customer.CustomerNotes.Should().BeEmpty();
        customer.Preferences.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithEmptyFirstName_ThrowsDomainException()
    {
        // Act
        var action = () => new Customer("", _lastName, _email, _phoneNumber);

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("First name cannot be empty");
    }

    [Fact]
    public void Constructor_WithEmptyLastName_ThrowsDomainException()
    {
        // Act
        var action = () => new Customer(_firstName, "", _email, _phoneNumber);

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("Last name cannot be empty");
    }

    [Fact]
    public void Constructor_WithInvalidEmail_ThrowsDomainException()
    {
        // Act
        var action = () => new Customer(_firstName, _lastName, "invalid-email", _phoneNumber);

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("Invalid email format");
    }

    [Fact]
    public void Constructor_WithEmptyPhoneNumber_ThrowsDomainException()
    {
        // Act
        var action = () => new Customer(_firstName, _lastName, _email, "");

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("Phone number cannot be empty");
    }

    [Fact]
    public void UpdateContactInfo_WithValidParameters_UpdatesSuccessfully()
    {
        // Arrange
        var customer = new Customer(_firstName, _lastName, _email, _phoneNumber);
        var newFirstName = "Jane";
        var newLastName = "Smith";
        var newEmail = "jane.smith@example.com";
        var newPhoneNumber = "0987654321";

        // Act
        customer.UpdateContactInfo(newFirstName, newLastName, newEmail, newPhoneNumber);

        // Assert
        customer.FirstName.Should().Be(newFirstName);
        customer.LastName.Should().Be(newLastName);
        customer.Email.Should().Be(newEmail);
        customer.PhoneNumber.Should().Be(newPhoneNumber);
        customer.LastModifiedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UpdateAddress_WithValidAddress_UpdatesSuccessfully()
    {
        // Arrange
        var customer = new Customer(_firstName, _lastName, _email, _phoneNumber);
        var address = new Address("123 Main St", "New York", "NY", "USA", "10001");

        // Act
        customer.UpdateAddress(address);

        // Assert
        customer.Address.Should().Be(address);
        customer.LastModifiedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void SetCreditLimit_WithValidAmount_SetsSuccessfully()
    {
        // Arrange
        var customer = new Customer(_firstName, _lastName, _email, _phoneNumber);
        var creditLimit = 1000m;

        // Act
        customer.SetCreditLimit(creditLimit);

        // Assert
        customer.CreditLimit.Should().Be(creditLimit);
        customer.LastModifiedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void SetCreditLimit_WithNegativeAmount_ThrowsDomainException()
    {
        // Arrange
        var customer = new Customer(_firstName, _lastName, _email, _phoneNumber);
        var creditLimit = -1000m;

        // Act
        var action = () => customer.SetCreditLimit(creditLimit);

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("Credit limit cannot be negative");
    }

    [Fact]
    public void AddPriceList_WithValidPriceList_AddsSuccessfully()
    {
        // Arrange
        var customer = new Customer(_firstName, _lastName, _email, _phoneNumber);
        var priceList = new PriceList("VIP Prices", "Special pricing for VIP customers", true, "System");

        // Act
        customer.AddPriceList(priceList);

        // Assert
        customer.PriceLists.Should().ContainSingle();
        customer.PriceLists.First().Should().Be(priceList);
        customer.LastModifiedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void RemovePriceList_WithExistingPriceList_RemovesSuccessfully()
    {
        // Arrange
        var customer = new Customer(_firstName, _lastName, _email, _phoneNumber);
        var priceList = new PriceList("VIP Prices", "Special pricing for VIP customers", true, "System");
        customer.AddPriceList(priceList);

        // Act
        customer.RemovePriceList(priceList.Id);

        // Assert
        customer.PriceLists.Should().BeEmpty();
        customer.LastModifiedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void RemovePriceList_WithNonExistentPriceList_ThrowsDomainException()
    {
        // Arrange
        var customer = new Customer(_firstName, _lastName, _email, _phoneNumber);

        // Act
        var action = () => customer.RemovePriceList(Guid.NewGuid());

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("Price list not found");
    }

    [Fact]
    public void EnrollInLoyaltyProgram_WithValidProgram_EnrollsSuccessfully()
    {
        // Arrange
        var customer = new Customer(_firstName, _lastName, _email, _phoneNumber);
        var program = new LoyaltyProgram("Gold", "Premium rewards program", "12345", "Gold", "System");

        // Act
        customer.EnrollInLoyaltyProgram(program);

        // Assert
        customer.LoyaltyProgram.Should().Be(program);
        customer.LastModifiedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void EnrollInLoyaltyProgram_WhenAlreadyEnrolled_ThrowsDomainException()
    {
        // Arrange
        var customer = new Customer(_firstName, _lastName, _email, _phoneNumber);
        var program1 = new LoyaltyProgram("Gold", "Premium rewards program", "12345", "Gold", "System");
        var program2 = new LoyaltyProgram("Silver", "Standard rewards program", "67890", "Silver", "System");
        customer.EnrollInLoyaltyProgram(program1);

        // Act
        var action = () => customer.EnrollInLoyaltyProgram(program2);

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("Customer is already enrolled in a loyalty program");
    }

    [Fact]
    public void AddContact_WithValidContact_AddsSuccessfully()
    {
        // Arrange
        var customer = new Customer(_firstName, _lastName, _email, _phoneNumber);
        var contact = new CustomerContact("Jane", "Smith", "jane.smith@example.com", "0987654321", "Manager");

        // Act
        customer.AddContact(contact);

        // Assert
        customer.Contacts.Should().ContainSingle();
        customer.Contacts.First().Should().Be(contact);
        customer.LastModifiedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void AddContact_WithDuplicateEmail_ThrowsDomainException()
    {
        // Arrange
        var customer = new Customer(_firstName, _lastName, _email, _phoneNumber);
        var contact1 = new CustomerContact("Jane", "Smith", "jane.smith@example.com", "0987654321", "Manager");
        var contact2 = new CustomerContact("John", "Doe", "jane.smith@example.com", "1234567890", "Assistant");
        customer.AddContact(contact1);

        // Act
        var action = () => customer.AddContact(contact2);

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("Contact with this email already exists");
    }

    [Fact]
    public void FullName_ReturnsCorrectFormat()
    {
        // Arrange
        var customer = new Customer(_firstName, _lastName, _email, _phoneNumber);

        // Act
        var fullName = customer.FullName;

        // Assert
        fullName.Should().Be($"{_firstName} {_lastName}");
    }

    [Fact]
    public void HasCreditAvailable_WhenBalanceBelowLimit_ReturnsTrue()
    {
        // Arrange
        var customer = new Customer(_firstName, _lastName, _email, _phoneNumber);
        customer.SetCreditLimit(1000m);

        // Act
        var hasCreditAvailable = customer.HasCreditAvailable;

        // Assert
        hasCreditAvailable.Should().BeTrue();
    }

    [Fact]
    public void AvailableCredit_ReturnsCorrectAmount()
    {
        // Arrange
        var customer = new Customer(_firstName, _lastName, _email, _phoneNumber);
        customer.SetCreditLimit(1000m);

        // Act
        var availableCredit = customer.AvailableCredit;

        // Assert
        availableCredit.Should().Be(1000m);
    }

    [Fact]
    public void IsOverdue_WhenNoPurchases_ReturnsFalse()
    {
        // Arrange
        var customer = new Customer(_firstName, _lastName, _email, _phoneNumber);

        // Act
        var isOverdue = customer.IsOverdue;

        // Assert
        isOverdue.Should().BeFalse();
    }
} 
