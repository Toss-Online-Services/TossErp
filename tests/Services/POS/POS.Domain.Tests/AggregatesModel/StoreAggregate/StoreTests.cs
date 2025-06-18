using FluentAssertions;
using POS.Domain.AggregatesModel.StoreAggregate;
using POS.Domain.Common.ValueObjects;
using POS.Domain.Enums;
using POS.Domain.Exceptions;
using Xunit;

namespace POS.Domain.Tests.AggregatesModel.StoreAggregate;

public class StoreTests
{
    private readonly string _name;
    private readonly string _code;
    private readonly Address _address;
    private readonly string _phone;
    private readonly string _email;
    private readonly string _timeZone;
    private readonly string? _website;
    private readonly string? _description;
    private readonly string? _taxId;
    private readonly string? _licenseNumber;
    private readonly string? _logoUrl;
    private readonly string? _bannerUrl;
    private readonly string? _socialMediaLinks;

    public StoreTests()
    {
        _name = "Test Store";
        _code = "TS001";
        _address = new Address("123 Test St", "Test City", "Test State", "Test Country", "12345");
        _phone = "+1234567890";
        _email = "test@store.com";
        _timeZone = "UTC";
        _website = "https://teststore.com";
        _description = "Test store description";
        _taxId = "TAX123";
        _licenseNumber = "LIC123";
        _logoUrl = "https://teststore.com/logo.png";
        _bannerUrl = "https://teststore.com/banner.png";
        _socialMediaLinks = "https://facebook.com/teststore";
    }

    [Fact]
    public void Constructor_WithValidParameters_CreatesStoreSuccessfully()
    {
        // Act
        var store = new Store(
            _name,
            _code,
            _address,
            _phone,
            _email,
            _timeZone,
            _website,
            _description,
            _taxId,
            _licenseNumber,
            _logoUrl,
            _bannerUrl,
            _socialMediaLinks);

        // Assert
        store.Name.Should().Be(_name);
        store.Code.Should().Be(_code);
        store.Address.Should().Be(_address);
        store.Phone.Should().Be(_phone);
        store.Email.Should().Be(_email);
        store.TimeZone.Should().Be(_timeZone);
        store.Website.Should().Be(_website);
        store.Description.Should().Be(_description);
        store.TaxId.Should().Be(_taxId);
        store.LicenseNumber.Should().Be(_licenseNumber);
        store.LogoUrl.Should().Be(_logoUrl);
        store.BannerUrl.Should().Be(_bannerUrl);
        store.SocialMediaLinks.Should().Be(_socialMediaLinks);
        store.IsActive.Should().BeTrue();
        store.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        store.UpdatedAt.Should().BeNull();
        store.StoreHours.Should().BeEmpty();
        store.Devices.Should().BeEmpty();
        store.Printers.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithEmptyName_ThrowsDomainException(string name)
    {
        // Act
        var act = () => new Store(
            name,
            _code,
            _address,
            _phone,
            _email,
            _timeZone);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Store name cannot be empty");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithEmptyCode_ThrowsDomainException(string code)
    {
        // Act
        var act = () => new Store(
            _name,
            code,
            _address,
            _phone,
            _email,
            _timeZone);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Store code cannot be empty");
    }

    [Fact]
    public void Constructor_WithNullAddress_ThrowsDomainException()
    {
        // Act
        var act = () => new Store(
            _name,
            _code,
            null!,
            _phone,
            _email,
            _timeZone);

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithEmptyPhone_ThrowsDomainException(string phone)
    {
        // Act
        var act = () => new Store(
            _name,
            _code,
            _address,
            phone,
            _email,
            _timeZone);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Store phone cannot be empty");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithEmptyEmail_ThrowsDomainException(string email)
    {
        // Act
        var act = () => new Store(
            _name,
            _code,
            _address,
            _phone,
            email,
            _timeZone);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Store email cannot be empty");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithEmptyTimeZone_ThrowsDomainException(string timeZone)
    {
        // Act
        var act = () => new Store(
            _name,
            _code,
            _address,
            _phone,
            _email,
            timeZone);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("TimeZone cannot be empty");
    }

    [Fact]
    public void UpdateDetails_WithValidParameters_UpdatesSuccessfully()
    {
        // Arrange
        var store = new Store(
            _name,
            _code,
            _address,
            _phone,
            _email,
            _timeZone,
            _website,
            _description,
            _taxId,
            _licenseNumber,
            _logoUrl,
            _bannerUrl,
            _socialMediaLinks);

        var newName = "Updated Store";
        var newCode = "US001";
        var newAddress = new Address("456 Updated St", "Updated City", "Updated State", "Updated Country", "54321");
        var newPhone = "+9876543210";
        var newEmail = "updated@store.com";
        var newWebsite = "https://updatedstore.com";
        var newDescription = "Updated store description";
        var newTaxId = "TAX456";
        var newLicenseNumber = "LIC456";
        var newLogoUrl = "https://updatedstore.com/logo.png";
        var newBannerUrl = "https://updatedstore.com/banner.png";
        var newSocialMediaLinks = "https://facebook.com/updatedstore";

        // Act
        store.UpdateDetails(
            newName,
            newCode,
            newAddress,
            newPhone,
            newEmail,
            newWebsite,
            newDescription,
            newTaxId,
            newLicenseNumber,
            newLogoUrl,
            newBannerUrl,
            newSocialMediaLinks);

        // Assert
        store.Name.Should().Be(newName);
        store.Code.Should().Be(newCode);
        store.Address.Should().Be(newAddress);
        store.Phone.Should().Be(newPhone);
        store.Email.Should().Be(newEmail);
        store.Website.Should().Be(newWebsite);
        store.Description.Should().Be(newDescription);
        store.TaxId.Should().Be(newTaxId);
        store.LicenseNumber.Should().Be(newLicenseNumber);
        store.LogoUrl.Should().Be(newLogoUrl);
        store.BannerUrl.Should().Be(newBannerUrl);
        store.SocialMediaLinks.Should().Be(newSocialMediaLinks);
        store.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateDetails_WithEmptyName_ThrowsDomainException(string name)
    {
        // Arrange
        var store = new Store(
            _name,
            _code,
            _address,
            _phone,
            _email,
            _timeZone,
            _website,
            _description,
            _taxId,
            _licenseNumber,
            _logoUrl,
            _bannerUrl,
            _socialMediaLinks);

        // Act
        var act = () => store.UpdateDetails(
            name,
            _code,
            _address,
            _phone,
            _email,
            _website,
            _description,
            _taxId,
            _licenseNumber,
            _logoUrl,
            _bannerUrl,
            _socialMediaLinks);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Store name cannot be empty");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateDetails_WithEmptyCode_ThrowsDomainException(string code)
    {
        // Arrange
        var store = new Store(
            _name,
            _code,
            _address,
            _phone,
            _email,
            _timeZone,
            _website,
            _description,
            _taxId,
            _licenseNumber,
            _logoUrl,
            _bannerUrl,
            _socialMediaLinks);

        // Act
        var act = () => store.UpdateDetails(
            _name,
            code,
            _address,
            _phone,
            _email,
            _website,
            _description,
            _taxId,
            _licenseNumber,
            _logoUrl,
            _bannerUrl,
            _socialMediaLinks);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Store code cannot be empty");
    }

    [Fact]
    public void UpdateDetails_WithNullAddress_ThrowsDomainException()
    {
        // Arrange
        var store = new Store(
            _name,
            _code,
            _address,
            _phone,
            _email,
            _timeZone,
            _website,
            _description,
            _taxId,
            _licenseNumber,
            _logoUrl,
            _bannerUrl,
            _socialMediaLinks);

        // Act
        var act = () => store.UpdateDetails(
            _name,
            _code,
            null!,
            _phone,
            _email,
            _website,
            _description,
            _taxId,
            _licenseNumber,
            _logoUrl,
            _bannerUrl,
            _socialMediaLinks);

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateDetails_WithEmptyPhone_ThrowsDomainException(string phone)
    {
        // Arrange
        var store = new Store(
            _name,
            _code,
            _address,
            phone,
            _email,
            _timeZone,
            _website,
            _description,
            _taxId,
            _licenseNumber,
            _logoUrl,
            _bannerUrl,
            _socialMediaLinks);

        // Act
        var act = () => store.UpdateDetails(
            _name,
            _code,
            _address,
            phone,
            _email,
            _website,
            _description,
            _taxId,
            _licenseNumber,
            _logoUrl,
            _bannerUrl,
            _socialMediaLinks);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Store phone cannot be empty");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateDetails_WithEmptyEmail_ThrowsDomainException(string email)
    {
        // Arrange
        var store = new Store(
            _name,
            _code,
            _address,
            _phone,
            email,
            _timeZone,
            _website,
            _description,
            _taxId,
            _licenseNumber,
            _logoUrl,
            _bannerUrl,
            _socialMediaLinks);

        // Act
        var act = () => store.UpdateDetails(
            _name,
            _code,
            _address,
            _phone,
            email,
            _website,
            _description,
            _taxId,
            _licenseNumber,
            _logoUrl,
            _bannerUrl,
            _socialMediaLinks);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Store email cannot be empty");
    }

    [Fact]
    public void UpdateSettings_WithValidSettings_UpdatesSuccessfully()
    {
        // Arrange
        var store = new Store(
            _name,
            _code,
            _address,
            _phone,
            _email,
            _timeZone,
            _website,
            _description,
            _taxId,
            _licenseNumber,
            _logoUrl,
            _bannerUrl,
            _socialMediaLinks);

        var newSettings = new StoreSettings(
            enableTax: true,
            taxRate: 0.1m,
            enableDiscounts: true,
            maxDiscountPercentage: 20,
            requireCustomerInfo: true,
            enableLoyaltyProgram: true,
            receiptHeader: "Updated Header",
            receiptFooter: "Updated Footer",
            enableStockAlerts: true,
            enableEmailNotifications: true,
            enableSMSNotifications: true,
            emailTemplate: "Updated Email Template",
            smsTemplate: "Updated SMS Template");

        // Act
        store.UpdateSettings(newSettings);

        // Assert
        store.Settings.Should().Be(newSettings);
        store.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UpdateSettings_WithNullSettings_ThrowsDomainException()
    {
        // Arrange
        var store = new Store(
            _name,
            _code,
            _address,
            _phone,
            _email,
            _timeZone,
            _website,
            _description,
            _taxId,
            _licenseNumber,
            _logoUrl,
            _bannerUrl,
            _socialMediaLinks);

        // Act
        var act = () => store.UpdateSettings(null!);

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void AddStoreHours_WithValidParameters_AddsSuccessfully()
    {
        // Arrange
        var store = new Store(
            _name,
            _code,
            _address,
            _phone,
            _email,
            _timeZone,
            _website,
            _description,
            _taxId,
            _licenseNumber,
            _logoUrl,
            _bannerUrl,
            _socialMediaLinks);

        var storeHours = new StoreHours(
            DayOfWeek.Monday,
            TimeSpan.FromHours(9),
            TimeSpan.FromHours(17));

        // Act
        store.AddStoreHours(storeHours);

        // Assert
        store.StoreHours.Should().ContainSingle();
        store.StoreHours.First().Should().Be(storeHours);
        store.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void AddStoreHours_WithDuplicateDay_ThrowsDomainException()
    {
        // Arrange
        var store = new Store(
            _name,
            _code,
            _address,
            _phone,
            _email,
            _timeZone,
            _website,
            _description,
            _taxId,
            _licenseNumber,
            _logoUrl,
            _bannerUrl,
            _socialMediaLinks);

        var storeHours = new StoreHours(
            DayOfWeek.Monday,
            TimeSpan.FromHours(9),
            TimeSpan.FromHours(17));

        store.AddStoreHours(storeHours);

        // Act
        var act = () => store.AddStoreHours(storeHours);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage($"Store hours for {DayOfWeek.Monday} already exist");
    }

    [Fact]
    public void UpdateStoreHours_WithValidParameters_UpdatesSuccessfully()
    {
        // Arrange
        var store = new Store(
            _name,
            _code,
            _address,
            _phone,
            _email,
            _timeZone,
            _website,
            _description,
            _taxId,
            _licenseNumber,
            _logoUrl,
            _bannerUrl,
            _socialMediaLinks);

        var storeHours = new StoreHours(
            DayOfWeek.Monday,
            TimeSpan.FromHours(9),
            TimeSpan.FromHours(17));

        store.AddStoreHours(storeHours);

        var updatedStoreHours = new StoreHours(
            DayOfWeek.Monday,
            TimeSpan.FromHours(10),
            TimeSpan.FromHours(18));

        // Act
        store.UpdateStoreHours(updatedStoreHours);

        // Assert
        var updatedHours = store.StoreHours.First();
        updatedHours.OpenTime.Should().Be(TimeSpan.FromHours(10));
        updatedHours.CloseTime.Should().Be(TimeSpan.FromHours(18));
        store.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UpdateStoreHours_WithNonExistentDay_ThrowsDomainException()
    {
        // Arrange
        var store = new Store(
            _name,
            _code,
            _address,
            _phone,
            _email,
            _timeZone,
            _website,
            _description,
            _taxId,
            _licenseNumber,
            _logoUrl,
            _bannerUrl,
            _socialMediaLinks);

        var storeHours = new StoreHours(
            DayOfWeek.Monday,
            TimeSpan.FromHours(9),
            TimeSpan.FromHours(17));

        // Act
        var act = () => store.UpdateStoreHours(storeHours);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage($"Store hours for {DayOfWeek.Monday} do not exist");
    }

    [Fact]
    public void RemoveStoreHours_WithValidDay_RemovesSuccessfully()
    {
        // Arrange
        var store = new Store(
            _name,
            _code,
            _address,
            _phone,
            _email,
            _timeZone,
            _website,
            _description,
            _taxId,
            _licenseNumber,
            _logoUrl,
            _bannerUrl,
            _socialMediaLinks);

        var storeHours = new StoreHours(
            DayOfWeek.Monday,
            TimeSpan.FromHours(9),
            TimeSpan.FromHours(17));

        store.AddStoreHours(storeHours);

        // Act
        store.RemoveStoreHours(DayOfWeek.Monday.ToString());

        // Assert
        store.StoreHours.Should().BeEmpty();
        store.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void RemoveStoreHours_WithInvalidDay_ThrowsDomainException()
    {
        // Arrange
        var store = new Store(
            _name,
            _code,
            _address,
            _phone,
            _email,
            _timeZone,
            _website,
            _description,
            _taxId,
            _licenseNumber,
            _logoUrl,
            _bannerUrl,
            _socialMediaLinks);

        // Act
        var act = () => store.RemoveStoreHours("InvalidDay");

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Invalid day: InvalidDay");
    }

    [Fact]
    public void RemoveStoreHours_WithNonExistentDay_ThrowsDomainException()
    {
        // Arrange
        var store = new Store(
            _name,
            _code,
            _address,
            _phone,
            _email,
            _timeZone,
            _website,
            _description,
            _taxId,
            _licenseNumber,
            _logoUrl,
            _bannerUrl,
            _socialMediaLinks);

        // Act
        var act = () => store.RemoveStoreHours(DayOfWeek.Monday.ToString());

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage($"Store hours for {DayOfWeek.Monday} do not exist");
    }

    [Fact]
    public void AddDevice_WithValidParameters_AddsSuccessfully()
    {
        // Arrange
        var store = new Store(
            _name,
            _code,
            _address,
            _phone,
            _email,
            _timeZone,
            _website,
            _description,
            _taxId,
            _licenseNumber,
            _logoUrl,
            _bannerUrl,
            _socialMediaLinks);

        var device = new StoreDevice("DEV001", "POS", "Main POS Terminal");

        // Act
        store.AddDevice(device);

        // Assert
        store.Devices.Should().ContainSingle();
        store.Devices.First().Should().Be(device);
        store.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void AddDevice_WithDuplicateDeviceId_ThrowsDomainException()
    {
        // Arrange
        var store = new Store(
            _name,
            _code,
            _address,
            _phone,
            _email,
            _timeZone,
            _website,
            _description,
            _taxId,
            _licenseNumber,
            _logoUrl,
            _bannerUrl,
            _socialMediaLinks);

        var device = new StoreDevice("DEV001", "POS", "Main POS Terminal");
        store.AddDevice(device);

        // Act
        var act = () => store.AddDevice(device);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Device with ID DEV001 already exists");
    }

    [Fact]
    public void UpdateDevice_WithValidParameters_UpdatesSuccessfully()
    {
        // Arrange
        var store = new Store(
            _name,
            _code,
            _address,
            _phone,
            _email,
            _timeZone,
            _website,
            _description,
            _taxId,
            _licenseNumber,
            _logoUrl,
            _bannerUrl,
            _socialMediaLinks);

        var device = new StoreDevice("DEV001", "POS", "Main POS Terminal");
        store.AddDevice(device);

        var updatedDevice = new StoreDevice("DEV001", "POS", "Updated POS Terminal");

        // Act
        store.UpdateDevice(updatedDevice);

        // Assert
        var updated = store.Devices.First();
        updated.Name.Should().Be("Updated POS Terminal");
        store.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UpdateDevice_WithNonExistentDeviceId_ThrowsDomainException()
    {
        // Arrange
        var store = new Store(
            _name,
            _code,
            _address,
            _phone,
            _email,
            _timeZone,
            _website,
            _description,
            _taxId,
            _licenseNumber,
            _logoUrl,
            _bannerUrl,
            _socialMediaLinks);

        var device = new StoreDevice("DEV001", "POS", "Main POS Terminal");

        // Act
        var act = () => store.UpdateDevice(device);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Device with ID DEV001 does not exist");
    }

    [Fact]
    public void RemoveDevice_WithValidDeviceId_RemovesSuccessfully()
    {
        // Arrange
        var store = new Store(
            _name,
            _code,
            _address,
            _phone,
            _email,
            _timeZone,
            _website,
            _description,
            _taxId,
            _licenseNumber,
            _logoUrl,
            _bannerUrl,
            _socialMediaLinks);

        var device = new StoreDevice("DEV001", "POS", "Main POS Terminal");
        store.AddDevice(device);

        // Act
        store.RemoveDevice("DEV001");

        // Assert
        store.Devices.Should().BeEmpty();
        store.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void RemoveDevice_WithNonExistentDeviceId_ThrowsDomainException()
    {
        // Arrange
        var store = new Store(
            _name,
            _code,
            _address,
            _phone,
            _email,
            _timeZone,
            _website,
            _description,
            _taxId,
            _licenseNumber,
            _logoUrl,
            _bannerUrl,
            _socialMediaLinks);

        // Act
        var act = () => store.RemoveDevice("DEV001");

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Device with ID DEV001 does not exist");
    }

    [Fact]
    public void AddPrinter_WithValidParameters_AddsSuccessfully()
    {
        // Arrange
        var store = new Store(
            _name,
            _code,
            _address,
            _phone,
            _email,
            _timeZone,
            _website,
            _description,
            _taxId,
            _licenseNumber,
            _logoUrl,
            _bannerUrl,
            _socialMediaLinks);

        var printer = new StorePrinter("PRN001", "Receipt", "Main Receipt Printer");

        // Act
        store.AddPrinter(printer);

        // Assert
        store.Printers.Should().ContainSingle();
        store.Printers.First().Should().Be(printer);
        store.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void AddPrinter_WithDuplicatePrinterId_ThrowsDomainException()
    {
        // Arrange
        var store = new Store(
            _name,
            _code,
            _address,
            _phone,
            _email,
            _timeZone,
            _website,
            _description,
            _taxId,
            _licenseNumber,
            _logoUrl,
            _bannerUrl,
            _socialMediaLinks);

        var printer = new StorePrinter("PRN001", "Receipt", "Main Receipt Printer");
        store.AddPrinter(printer);

        // Act
        var act = () => store.AddPrinter(printer);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Printer with ID PRN001 already exists");
    }

    [Fact]
    public void UpdatePrinter_WithValidParameters_UpdatesSuccessfully()
    {
        // Arrange
        var store = new Store(
            _name,
            _code,
            _address,
            _phone,
            _email,
            _timeZone,
            _website,
            _description,
            _taxId,
            _licenseNumber,
            _logoUrl,
            _bannerUrl,
            _socialMediaLinks);

        var printer = new StorePrinter("PRN001", "Receipt", "Main Receipt Printer");
        store.AddPrinter(printer);

        var updatedPrinter = new StorePrinter("PRN001", "Receipt", "Updated Receipt Printer");

        // Act
        store.UpdatePrinter(updatedPrinter);

        // Assert
        var updated = store.Printers.First();
        updated.Name.Should().Be("Updated Receipt Printer");
        store.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UpdatePrinter_WithNonExistentPrinterId_ThrowsDomainException()
    {
        // Arrange
        var store = new Store(
            _name,
            _code,
            _address,
            _phone,
            _email,
            _timeZone,
            _website,
            _description,
            _taxId,
            _licenseNumber,
            _logoUrl,
            _bannerUrl,
            _socialMediaLinks);

        var printer = new StorePrinter("PRN001", "Receipt", "Main Receipt Printer");

        // Act
        var act = () => store.UpdatePrinter(printer);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Printer with ID PRN001 does not exist");
    }

    [Fact]
    public void RemovePrinter_WithValidPrinterId_RemovesSuccessfully()
    {
        // Arrange
        var store = new Store(
            _name,
            _code,
            _address,
            _phone,
            _email,
            _timeZone,
            _website,
            _description,
            _taxId,
            _licenseNumber,
            _logoUrl,
            _bannerUrl,
            _socialMediaLinks);

        var printer = new StorePrinter("PRN001", "Receipt", "Main Receipt Printer");
        store.AddPrinter(printer);

        // Act
        store.RemovePrinter("PRN001");

        // Assert
        store.Printers.Should().BeEmpty();
        store.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void RemovePrinter_WithNonExistentPrinterId_ThrowsDomainException()
    {
        // Arrange
        var store = new Store(
            _name,
            _code,
            _address,
            _phone,
            _email,
            _timeZone,
            _website,
            _description,
            _taxId,
            _licenseNumber,
            _logoUrl,
            _bannerUrl,
            _socialMediaLinks);

        // Act
        var act = () => store.RemovePrinter("PRN001");

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Printer with ID PRN001 does not exist");
    }

    [Fact]
    public void Deactivate_WhenActive_DeactivatesSuccessfully()
    {
        // Arrange
        var store = new Store(
            _name,
            _code,
            _address,
            _phone,
            _email,
            _timeZone,
            _website,
            _description,
            _taxId,
            _licenseNumber,
            _logoUrl,
            _bannerUrl,
            _socialMediaLinks);

        // Act
        store.Deactivate();

        // Assert
        store.IsActive.Should().BeFalse();
        store.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Deactivate_WhenAlreadyInactive_ThrowsDomainException()
    {
        // Arrange
        var store = new Store(
            _name,
            _code,
            _address,
            _phone,
            _email,
            _timeZone,
            _website,
            _description,
            _taxId,
            _licenseNumber,
            _logoUrl,
            _bannerUrl,
            _socialMediaLinks);

        store.Deactivate();

        // Act
        var act = () => store.Deactivate();

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Store is already deactivated");
    }

    [Fact]
    public void Activate_WhenInactive_ActivatesSuccessfully()
    {
        // Arrange
        var store = new Store(
            _name,
            _code,
            _address,
            _phone,
            _email,
            _timeZone,
            _website,
            _description,
            _taxId,
            _licenseNumber,
            _logoUrl,
            _bannerUrl,
            _socialMediaLinks);

        store.Deactivate();

        // Act
        store.Activate();

        // Assert
        store.IsActive.Should().BeTrue();
        store.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Activate_WhenAlreadyActive_ThrowsDomainException()
    {
        // Arrange
        var store = new Store(
            _name,
            _code,
            _address,
            _phone,
            _email,
            _timeZone,
            _website,
            _description,
            _taxId,
            _licenseNumber,
            _logoUrl,
            _bannerUrl,
            _socialMediaLinks);

        // Act
        var act = () => store.Activate();

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Store is already active");
    }
} 
