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
    private readonly string _website;
    private readonly string _description;
    private readonly string _taxId;
    private readonly string _licenseNumber;
    private readonly string _logoUrl;
    private readonly string _bannerUrl;
    private readonly Dictionary<string, string> _socialMediaLinks;
    private readonly StoreSettings _settings;

    public StoreTests()
    {
        _name = "Test Store";
        _code = "TS001";
        _address = new Address("123 Main St", "Test City", "Test State", "Test Country", "12345");
        _phone = "+1234567890";
        _email = "store@example.com";
        _timeZone = "America/New_York";
        _website = "https://teststore.com";
        _description = "Test store description";
        _taxId = "TAX123456";
        _licenseNumber = "LIC123456";
        _logoUrl = "https://teststore.com/logo.png";
        _bannerUrl = "https://teststore.com/banner.png";
        _socialMediaLinks = new Dictionary<string, string>
        {
            { "Facebook", "https://facebook.com/teststore" },
            { "Twitter", "https://twitter.com/teststore" }
        };
        _settings = new StoreSettings
        {
            Currency = "USD",
            Language = "en-US",
            TaxRate = 0.08m,
            EnableLoyaltyProgram = true,
            EnableInventoryTracking = true
        };
    }

    [Fact]
    public void Constructor_WithValidParameters_CreatesStoreSuccessfully()
    {
        // Act
        var store = new Store(_name, _code, _address, _phone, _email, _timeZone, _website, _description, _taxId, _licenseNumber, _logoUrl, _bannerUrl, _socialMediaLinks, _settings);

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
        store.SocialMediaLinks.Should().BeEquivalentTo(_socialMediaLinks);
        store.Settings.Should().Be(_settings);
        store.IsActive.Should().BeTrue();
        store.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Constructor_WithEmptyName_ThrowsDomainException()
    {
        // Act & Assert
        var action = () => new Store(string.Empty, _code, _address, _phone, _email, _timeZone, _website, _description, _taxId, _licenseNumber, _logoUrl, _bannerUrl, _socialMediaLinks, _settings);
        action.Should().Throw<DomainException>().WithMessage("Name cannot be empty");
    }

    [Fact]
    public void Constructor_WithEmptyCode_ThrowsDomainException()
    {
        // Act & Assert
        var action = () => new Store(_name, string.Empty, _address, _phone, _email, _timeZone, _website, _description, _taxId, _licenseNumber, _logoUrl, _bannerUrl, _socialMediaLinks, _settings);
        action.Should().Throw<DomainException>().WithMessage("Code cannot be empty");
    }

    [Fact]
    public void Constructor_WithNullAddress_ThrowsDomainException()
    {
        // Act & Assert
        var action = () => new Store(_name, _code, null, _phone, _email, _timeZone, _website, _description, _taxId, _licenseNumber, _logoUrl, _bannerUrl, _socialMediaLinks, _settings);
        action.Should().Throw<DomainException>().WithMessage("Address cannot be null");
    }

    [Fact]
    public void Constructor_WithEmptyPhone_ThrowsDomainException()
    {
        // Act & Assert
        var action = () => new Store(_name, _code, _address, string.Empty, _email, _timeZone, _website, _description, _taxId, _licenseNumber, _logoUrl, _bannerUrl, _socialMediaLinks, _settings);
        action.Should().Throw<DomainException>().WithMessage("Phone cannot be empty");
    }

    [Fact]
    public void Constructor_WithEmptyEmail_ThrowsDomainException()
    {
        // Act & Assert
        var action = () => new Store(_name, _code, _address, _phone, string.Empty, _timeZone, _website, _description, _taxId, _licenseNumber, _logoUrl, _bannerUrl, _socialMediaLinks, _settings);
        action.Should().Throw<DomainException>().WithMessage("Email cannot be empty");
    }

    [Fact]
    public void Constructor_WithInvalidEmail_ThrowsDomainException()
    {
        // Act & Assert
        var action = () => new Store(_name, _code, _address, _phone, "invalid-email", _timeZone, _website, _description, _taxId, _licenseNumber, _logoUrl, _bannerUrl, _socialMediaLinks, _settings);
        action.Should().Throw<DomainException>().WithMessage("Invalid email format");
    }

    [Fact]
    public void Constructor_WithEmptyTimeZone_ThrowsDomainException()
    {
        // Act & Assert
        var action = () => new Store(_name, _code, _address, _phone, _email, string.Empty, _website, _description, _taxId, _licenseNumber, _logoUrl, _bannerUrl, _socialMediaLinks, _settings);
        action.Should().Throw<DomainException>().WithMessage("Time zone cannot be empty");
    }

    [Fact]
    public void Constructor_WithNullSettings_ThrowsDomainException()
    {
        // Act & Assert
        var action = () => new Store(_name, _code, _address, _phone, _email, _timeZone, _website, _description, _taxId, _licenseNumber, _logoUrl, _bannerUrl, _socialMediaLinks, null);
        action.Should().Throw<DomainException>().WithMessage("Settings cannot be null");
    }

    [Fact]
    public void UpdateDetails_WithValidParameters_UpdatesSuccessfully()
    {
        // Arrange
        var store = new Store(_name, _code, _address, _phone, _email, _timeZone, _website, _description, _taxId, _licenseNumber, _logoUrl, _bannerUrl, _socialMediaLinks, _settings);
        var newName = "Updated Store";
        var newCode = "TS002";
        var newAddress = new Address("456 Oak St", "Suite 200", "Newtown", "NY", "67890", "USA");
        var newPhone = "0987654321";
        var newEmail = "updated@example.com";
        var newWebsite = "https://updatedstore.com";
        var newDescription = "Updated store description";
        var newTaxId = "TAX456";
        var newLicenseNumber = "LIC456";
        var newLogoUrl = "https://updatedstore.com/logo.png";
        var newBannerUrl = "https://updatedstore.com/banner.png";
        var newSocialMediaLinks = new Dictionary<string, string>
        {
            { "Facebook", "https://facebook.com/updatedstore" },
            { "Twitter", "https://twitter.com/updatedstore" }
        };

        // Act
        store.UpdateDetails(newName, newCode, newAddress, newPhone, newEmail, newWebsite, newDescription, newTaxId, newLicenseNumber, newLogoUrl, newBannerUrl, newSocialMediaLinks);

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
        store.SocialMediaLinks.Should().BeEquivalentTo(newSocialMediaLinks);
        store.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UpdateSettings_WithValidSettings_UpdatesSuccessfully()
    {
        // Arrange
        var store = new Store(_name, _code, _address, _phone, _email, _timeZone, _website, _description, _taxId, _licenseNumber, _logoUrl, _bannerUrl, _socialMediaLinks, _settings);
        var newSettings = new StoreSettings
        {
            Currency = "EUR",
            Language = "de-DE",
            TaxRate = 0.09m,
            EnableLoyaltyProgram = false,
            EnableInventoryTracking = false
        };

        // Act
        store.UpdateSettings(newSettings);

        // Assert
        store.Settings.Should().Be(newSettings);
        store.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void AddStoreHours_WithValidHours_AddsSuccessfully()
    {
        // Arrange
        var store = new Store(_name, _code, _address, _phone, _email, _timeZone, _website, _description, _taxId, _licenseNumber, _logoUrl, _bannerUrl, _socialMediaLinks, _settings);
        var storeHours = new StoreHours(DayOfWeek.Monday, new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0), true);

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
        var store = new Store(_name, _code, _address, _phone, _email, _timeZone, _website, _description, _taxId, _licenseNumber, _logoUrl, _bannerUrl, _socialMediaLinks, _settings);
        var storeHours = new StoreHours(DayOfWeek.Monday, new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0), true);
        store.AddStoreHours(storeHours);

        // Act & Assert
        var action = () => store.AddStoreHours(storeHours);
        action.Should().Throw<DomainException>().WithMessage("Store hours for Monday already exist");
    }

    [Fact]
    public void UpdateStoreHours_WithValidHours_UpdatesSuccessfully()
    {
        // Arrange
        var store = new Store(_name, _code, _address, _phone, _email, _timeZone, _website, _description, _taxId, _licenseNumber, _logoUrl, _bannerUrl, _socialMediaLinks, _settings);
        var storeHours = new StoreHours(DayOfWeek.Monday, new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0), true);
        store.AddStoreHours(storeHours);
        var updatedHours = new StoreHours(DayOfWeek.Monday, new TimeSpan(10, 0, 0), new TimeSpan(18, 0, 0), true);

        // Act
        store.UpdateStoreHours(updatedHours);

        // Assert
        store.StoreHours.Should().ContainSingle();
        store.StoreHours.First().OpenTime.Should().Be(new TimeSpan(10, 0, 0));
        store.StoreHours.First().CloseTime.Should().Be(new TimeSpan(18, 0, 0));
        store.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UpdateStoreHours_WithNonExistentDay_ThrowsDomainException()
    {
        // Arrange
        var store = new Store(_name, _code, _address, _phone, _email, _timeZone, _website, _description, _taxId, _licenseNumber, _logoUrl, _bannerUrl, _socialMediaLinks, _settings);
        var storeHours = new StoreHours(DayOfWeek.Monday, new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0), true);

        // Act & Assert
        var action = () => store.UpdateStoreHours(storeHours);
        action.Should().Throw<DomainException>().WithMessage("Store hours for Monday do not exist");
    }

    [Fact]
    public void RemoveStoreHours_WithValidDay_RemovesSuccessfully()
    {
        // Arrange
        var store = new Store(_name, _code, _address, _phone, _email, _timeZone, _website, _description, _taxId, _licenseNumber, _logoUrl, _bannerUrl, _socialMediaLinks, _settings);
        var storeHours = new StoreHours(DayOfWeek.Monday, new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0), true);
        store.AddStoreHours(storeHours);

        // Act
        store.RemoveStoreHours("Monday");

        // Assert
        store.StoreHours.Should().BeEmpty();
        store.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void RemoveStoreHours_WithInvalidDay_ThrowsDomainException()
    {
        // Arrange
        var store = new Store(_name, _code, _address, _phone, _email, _timeZone, _website, _description, _taxId, _licenseNumber, _logoUrl, _bannerUrl, _socialMediaLinks, _settings);

        // Act & Assert
        var action = () => store.RemoveStoreHours("InvalidDay");
        action.Should().Throw<DomainException>().WithMessage("Invalid day: InvalidDay");
    }

    [Fact]
    public void RemoveStoreHours_WithNonExistentDay_ThrowsDomainException()
    {
        // Arrange
        var store = new Store(_name, _code, _address, _phone, _email, _timeZone, _website, _description, _taxId, _licenseNumber, _logoUrl, _bannerUrl, _socialMediaLinks, _settings);

        // Act & Assert
        var action = () => store.RemoveStoreHours("Monday");
        action.Should().Throw<DomainException>().WithMessage("Store hours for Monday do not exist");
    }

    [Fact]
    public void AddDevice_WithValidDevice_AddsSuccessfully()
    {
        // Arrange
        var store = new Store(_name, _code, _address, _phone, _email, _timeZone, _website, _description, _taxId, _licenseNumber, _logoUrl, _bannerUrl, _socialMediaLinks, _settings);
        var device = new StoreDevice("DEV001", "POS", "Main Register");

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
        var store = new Store(_name, _code, _address, _phone, _email, _timeZone, _website, _description, _taxId, _licenseNumber, _logoUrl, _bannerUrl, _socialMediaLinks, _settings);
        var device = new StoreDevice("DEV001", "POS", "Main Register");
        store.AddDevice(device);

        // Act & Assert
        var action = () => store.AddDevice(device);
        action.Should().Throw<DomainException>().WithMessage("Device with ID DEV001 already exists");
    }

    [Fact]
    public void UpdateDevice_WithValidDevice_UpdatesSuccessfully()
    {
        // Arrange
        var store = new Store(_name, _code, _address, _phone, _email, _timeZone, _website, _description, _taxId, _licenseNumber, _logoUrl, _bannerUrl, _socialMediaLinks, _settings);
        var device = new StoreDevice("DEV001", "POS", "Main Register");
        store.AddDevice(device);
        var updatedDevice = new StoreDevice("DEV001", "POS", "Updated Register");

        // Act
        store.UpdateDevice(updatedDevice);

        // Assert
        store.Devices.Should().ContainSingle();
        store.Devices.First().Name.Should().Be("Updated Register");
        store.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UpdateDevice_WithNonExistentDevice_ThrowsDomainException()
    {
        // Arrange
        var store = new Store(_name, _code, _address, _phone, _email, _timeZone, _website, _description, _taxId, _licenseNumber, _logoUrl, _bannerUrl, _socialMediaLinks, _settings);
        var device = new StoreDevice("DEV001", "POS", "Main Register");

        // Act & Assert
        var action = () => store.UpdateDevice(device);
        action.Should().Throw<DomainException>().WithMessage("Device with ID DEV001 does not exist");
    }

    [Fact]
    public void RemoveDevice_WithValidDeviceId_RemovesSuccessfully()
    {
        // Arrange
        var store = new Store(_name, _code, _address, _phone, _email, _timeZone, _website, _description, _taxId, _licenseNumber, _logoUrl, _bannerUrl, _socialMediaLinks, _settings);
        var device = new StoreDevice("DEV001", "POS", "Main Register");
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
        var store = new Store(_name, _code, _address, _phone, _email, _timeZone, _website, _description, _taxId, _licenseNumber, _logoUrl, _bannerUrl, _socialMediaLinks, _settings);

        // Act & Assert
        var action = () => store.RemoveDevice("DEV001");
        action.Should().Throw<DomainException>().WithMessage("Device with ID DEV001 does not exist");
    }

    [Fact]
    public void AddPrinter_WithValidPrinter_AddsSuccessfully()
    {
        // Arrange
        var store = new Store(_name, _code, _address, _phone, _email, _timeZone, _website, _description, _taxId, _licenseNumber, _logoUrl, _bannerUrl, _socialMediaLinks, _settings);
        var printer = new StorePrinter("PRN001", "Receipt", "Main Printer");

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
        var store = new Store(_name, _code, _address, _phone, _email, _timeZone, _website, _description, _taxId, _licenseNumber, _logoUrl, _bannerUrl, _socialMediaLinks, _settings);
        var printer = new StorePrinter("PRN001", "Receipt", "Main Printer");
        store.AddPrinter(printer);

        // Act & Assert
        var action = () => store.AddPrinter(printer);
        action.Should().Throw<DomainException>().WithMessage("Printer with ID PRN001 already exists");
    }

    [Fact]
    public void UpdatePrinter_WithValidPrinter_UpdatesSuccessfully()
    {
        // Arrange
        var store = new Store(_name, _code, _address, _phone, _email, _timeZone, _website, _description, _taxId, _licenseNumber, _logoUrl, _bannerUrl, _socialMediaLinks, _settings);
        var printer = new StorePrinter("PRN001", "Receipt", "Main Printer");
        store.AddPrinter(printer);
        var updatedPrinter = new StorePrinter("PRN001", "Receipt", "Updated Printer");

        // Act
        store.UpdatePrinter(updatedPrinter);

        // Assert
        store.Printers.Should().ContainSingle();
        store.Printers.First().Name.Should().Be("Updated Printer");
        store.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UpdatePrinter_WithNonExistentPrinter_ThrowsDomainException()
    {
        // Arrange
        var store = new Store(_name, _code, _address, _phone, _email, _timeZone, _website, _description, _taxId, _licenseNumber, _logoUrl, _bannerUrl, _socialMediaLinks, _settings);
        var printer = new StorePrinter("PRN001", "Receipt", "Main Printer");

        // Act & Assert
        var action = () => store.UpdatePrinter(printer);
        action.Should().Throw<DomainException>().WithMessage("Printer with ID PRN001 does not exist");
    }

    [Fact]
    public void RemovePrinter_WithValidPrinterId_RemovesSuccessfully()
    {
        // Arrange
        var store = new Store(_name, _code, _address, _phone, _email, _timeZone, _website, _description, _taxId, _licenseNumber, _logoUrl, _bannerUrl, _socialMediaLinks, _settings);
        var printer = new StorePrinter("PRN001", "Receipt", "Main Printer");
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
        var store = new Store(_name, _code, _address, _phone, _email, _timeZone, _website, _description, _taxId, _licenseNumber, _logoUrl, _bannerUrl, _socialMediaLinks, _settings);

        // Act & Assert
        var action = () => store.RemovePrinter("PRN001");
        action.Should().Throw<DomainException>().WithMessage("Printer with ID PRN001 does not exist");
    }

    [Fact]
    public void Deactivate_WhenActive_DeactivatesSuccessfully()
    {
        // Arrange
        var store = new Store(_name, _code, _address, _phone, _email, _timeZone, _website, _description, _taxId, _licenseNumber, _logoUrl, _bannerUrl, _socialMediaLinks, _settings);

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
        var store = new Store(_name, _code, _address, _phone, _email, _timeZone, _website, _description, _taxId, _licenseNumber, _logoUrl, _bannerUrl, _socialMediaLinks, _settings);
        store.Deactivate();

        // Act & Assert
        var action = () => store.Deactivate();
        action.Should().Throw<DomainException>().WithMessage("Store is already deactivated");
    }

    [Fact]
    public void Activate_WhenInactive_ActivatesSuccessfully()
    {
        // Arrange
        var store = new Store(_name, _code, _address, _phone, _email, _timeZone, _website, _description, _taxId, _licenseNumber, _logoUrl, _bannerUrl, _socialMediaLinks, _settings);
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
        var store = new Store(_name, _code, _address, _phone, _email, _timeZone, _website, _description, _taxId, _licenseNumber, _logoUrl, _bannerUrl, _socialMediaLinks, _settings);

        // Act & Assert
        var action = () => store.Activate();
        action.Should().Throw<DomainException>().WithMessage("Store is already active");
    }
} 
