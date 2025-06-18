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

    #region Constructor Tests

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
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidName_ThrowsDomainException(string name)
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
    public void Constructor_WithInvalidCode_ThrowsDomainException(string code)
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
        act.Should().Throw<DomainException>()
            .WithMessage("Store address cannot be null");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidPhone_ThrowsDomainException(string phone)
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
    public void Constructor_WithInvalidEmail_ThrowsDomainException(string email)
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
    public void Constructor_WithInvalidTimeZone_ThrowsDomainException(string timeZone)
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

    #endregion

    #region Update Details Tests

    [Fact]
    public void UpdateDetails_WithValidParameters_UpdatesSuccessfully()
    {
        // Arrange
        var store = new Store(_name, _code, _address, _phone, _email, _timeZone);
        var newName = "Updated Store";
        var newCode = "US001";
        var newAddress = new Address("456 New St", "New City", "New State", "New Country", "54321");
        var newPhone = "+9876543210";
        var newEmail = "updated@store.com";
        var newWebsite = "https://updatedstore.com";
        var newDescription = "Updated store description";
        var newTaxId = "TAX456";
        var newLicenseNumber = "LIC456";
        var newLogoUrl = "https://updatedstore.com/logo.png";
        var newBannerUrl = "https://updatedstore.com/banner.png";
        var newSocialMediaLinks = "https://twitter.com/updatedstore";

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
    public void UpdateDetails_WithInvalidName_ThrowsDomainException(string name)
    {
        // Arrange
        var store = new Store(_name, _code, _address, _phone, _email, _timeZone);

        // Act
        var act = () => store.UpdateDetails(
            name,
            _code,
            _address,
            _phone,
            _email);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Store name cannot be empty");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateDetails_WithInvalidCode_ThrowsDomainException(string code)
    {
        // Arrange
        var store = new Store(_name, _code, _address, _phone, _email, _timeZone);

        // Act
        var act = () => store.UpdateDetails(
            _name,
            code,
            _address,
            _phone,
            _email);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Store code cannot be empty");
    }

    [Fact]
    public void UpdateDetails_WithNullAddress_ThrowsDomainException()
    {
        // Arrange
        var store = new Store(_name, _code, _address, _phone, _email, _timeZone);

        // Act
        var act = () => store.UpdateDetails(
            _name,
            _code,
            null!,
            _phone,
            _email);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Store address cannot be null");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateDetails_WithInvalidPhone_ThrowsDomainException(string phone)
    {
        // Arrange
        var store = new Store(_name, _code, _address, _phone, _email, _timeZone);

        // Act
        var act = () => store.UpdateDetails(
            _name,
            _code,
            _address,
            phone,
            _email);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Store phone cannot be empty");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateDetails_WithInvalidEmail_ThrowsDomainException(string email)
    {
        // Arrange
        var store = new Store(_name, _code, _address, _phone, _email, _timeZone);

        // Act
        var act = () => store.UpdateDetails(
            _name,
            _code,
            _address,
            _phone,
            email);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Store email cannot be empty");
    }

    #endregion

    #region Store Settings Tests

    [Fact]
    public void UpdateSettings_WithValidSettings_UpdatesSuccessfully()
    {
        // Arrange
        var store = new Store(_name, _code, _address, _phone, _email, _timeZone);
        var settings = new StoreSettings(
            enableTax: true,
            taxRate: 0.1m,
            enableDiscounts: true,
            maxDiscountPercentage: 20,
            requireCustomerInfo: true,
            enableLoyaltyProgram: true,
            receiptHeader: "Test Header",
            receiptFooter: "Test Footer",
            enableStockAlerts: true,
            enableEmailNotifications: true,
            enableSMSNotifications: true,
            emailTemplate: "Test Email Template",
            smsTemplate: "Test SMS Template");

        // Act
        store.UpdateSettings(settings);

        // Assert
        store.Settings.Should().Be(settings);
        store.Settings.EnableTax.Should().BeTrue();
        store.Settings.TaxRate.Should().Be(0.1m);
        store.Settings.EnableDiscounts.Should().BeTrue();
        store.Settings.MaxDiscountPercentage.Should().Be(20);
        store.Settings.RequireCustomerInfo.Should().BeTrue();
        store.Settings.EnableLoyaltyProgram.Should().BeTrue();
        store.Settings.ReceiptHeader.Should().Be("Test Header");
        store.Settings.ReceiptFooter.Should().Be("Test Footer");
        store.Settings.EnableStockAlerts.Should().BeTrue();
        store.Settings.EnableEmailNotifications.Should().BeTrue();
        store.Settings.EnableSMSNotifications.Should().BeTrue();
        store.Settings.EmailTemplate.Should().Be("Test Email Template");
        store.Settings.SMSTemplate.Should().Be("Test SMS Template");
        store.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UpdateSettings_WithNullSettings_ThrowsDomainException()
    {
        // Arrange
        var store = new Store(_name, _code, _address, _phone, _email, _timeZone);

        // Act
        var act = () => store.UpdateSettings(null!);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Store settings cannot be null");
    }

    #endregion

    #region Store Hours Tests

    [Fact]
    public void AddStoreHours_WithValidHours_AddsSuccessfully()
    {
        // Arrange
        var store = new Store(_name, _code, _address, _phone, _email, _timeZone);
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
        var store = new Store(_name, _code, _address, _phone, _email, _timeZone);
        var storeHours1 = new StoreHours(
            DayOfWeek.Monday,
            TimeSpan.FromHours(9),
            TimeSpan.FromHours(17));
        var storeHours2 = new StoreHours(
            DayOfWeek.Monday,
            TimeSpan.FromHours(10),
            TimeSpan.FromHours(18));

        store.AddStoreHours(storeHours1);

        // Act
        var act = () => store.AddStoreHours(storeHours2);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage($"Store hours for {DayOfWeek.Monday} already exist");
    }

    [Fact]
    public void UpdateStoreHours_WithValidHours_UpdatesSuccessfully()
    {
        // Arrange
        var store = new Store(_name, _code, _address, _phone, _email, _timeZone);
        var storeHours = new StoreHours(
            DayOfWeek.Monday,
            TimeSpan.FromHours(9),
            TimeSpan.FromHours(17));
        store.AddStoreHours(storeHours);

        var updatedHours = new StoreHours(
            DayOfWeek.Monday,
            TimeSpan.FromHours(10),
            TimeSpan.FromHours(18));

        // Act
        store.UpdateStoreHours(updatedHours);

        // Assert
        var updatedStoreHours = store.StoreHours.First();
        updatedStoreHours.OpenTime.Should().Be(TimeSpan.FromHours(10));
        updatedStoreHours.CloseTime.Should().Be(TimeSpan.FromHours(18));
        store.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UpdateStoreHours_WithNonExistentDay_ThrowsDomainException()
    {
        // Arrange
        var store = new Store(_name, _code, _address, _phone, _email, _timeZone);
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
        var store = new Store(_name, _code, _address, _phone, _email, _timeZone);
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
        var store = new Store(_name, _code, _address, _phone, _email, _timeZone);

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
        var store = new Store(_name, _code, _address, _phone, _email, _timeZone);

        // Act
        var act = () => store.RemoveStoreHours(DayOfWeek.Monday.ToString());

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage($"Store hours for {DayOfWeek.Monday} do not exist");
    }

    #endregion

    #region Store Device Tests

    [Fact]
    public void AddDevice_WithValidDevice_AddsSuccessfully()
    {
        // Arrange
        var store = new Store(_name, _code, _address, _phone, _email, _timeZone);
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
        var store = new Store(_name, _code, _address, _phone, _email, _timeZone);
        var device1 = new StoreDevice("DEV001", "POS", "Main POS Terminal");
        var device2 = new StoreDevice("DEV001", "POS", "Backup POS Terminal");

        store.AddDevice(device1);

        // Act
        var act = () => store.AddDevice(device2);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Device with ID DEV001 already exists");
    }

    [Fact]
    public void UpdateDevice_WithValidDevice_UpdatesSuccessfully()
    {
        // Arrange
        var store = new Store(_name, _code, _address, _phone, _email, _timeZone);
        var device = new StoreDevice("DEV001", "POS", "Main POS Terminal");
        store.AddDevice(device);

        var updatedDevice = new StoreDevice("DEV001", "POS", "Updated POS Terminal");

        // Act
        store.UpdateDevice(updatedDevice);

        // Assert
        var updatedStoreDevice = store.Devices.First();
        updatedStoreDevice.Name.Should().Be("Updated POS Terminal");
        store.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UpdateDevice_WithNonExistentDevice_ThrowsDomainException()
    {
        // Arrange
        var store = new Store(_name, _code, _address, _phone, _email, _timeZone);
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
        var store = new Store(_name, _code, _address, _phone, _email, _timeZone);
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
        var store = new Store(_name, _code, _address, _phone, _email, _timeZone);

        // Act
        var act = () => store.RemoveDevice("DEV001");

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Device with ID DEV001 does not exist");
    }

    #endregion

    #region Behavior-Driven Tests

    [Fact]
    public void WhenCreatingNewStore_ThenStoreIsCreatedWithDefaultSettings()
    {
        // Act
        var store = new Store(_name, _code, _address, _phone, _email, _timeZone);

        // Assert
        store.Settings.Should().NotBeNull();
        store.Settings.EnableTax.Should().BeFalse();
        store.Settings.TaxRate.Should().Be(0);
        store.Settings.EnableDiscounts.Should().BeFalse();
        store.Settings.MaxDiscountPercentage.Should().Be(0);
        store.Settings.RequireCustomerInfo.Should().BeFalse();
        store.Settings.EnableLoyaltyProgram.Should().BeFalse();
        store.Settings.ReceiptHeader.Should().BeEmpty();
        store.Settings.ReceiptFooter.Should().BeEmpty();
        store.Settings.EnableStockAlerts.Should().BeFalse();
        store.Settings.EnableEmailNotifications.Should().BeFalse();
        store.Settings.EnableSMSNotifications.Should().BeFalse();
        store.Settings.EmailTemplate.Should().BeNull();
        store.Settings.SMSTemplate.Should().BeNull();
    }

    [Fact]
    public void WhenSettingUpStoreOperatingHours_ThenStoreCanManageBusinessHours()
    {
        // Arrange
        var store = new Store(_name, _code, _address, _phone, _email, _timeZone);
        var mondayHours = new StoreHours(DayOfWeek.Monday, TimeSpan.FromHours(9), TimeSpan.FromHours(17));
        var tuesdayHours = new StoreHours(DayOfWeek.Tuesday, TimeSpan.FromHours(9), TimeSpan.FromHours(17));
        var wednesdayHours = new StoreHours(DayOfWeek.Wednesday, TimeSpan.FromHours(9), TimeSpan.FromHours(17));

        // Act
        store.AddStoreHours(mondayHours);
        store.AddStoreHours(tuesdayHours);
        store.AddStoreHours(wednesdayHours);

        // Assert
        store.StoreHours.Should().HaveCount(3);
        store.StoreHours.Should().Contain(mondayHours);
        store.StoreHours.Should().Contain(tuesdayHours);
        store.StoreHours.Should().Contain(wednesdayHours);
    }

    [Fact]
    public void WhenManagingStoreDevices_ThenStoreCanTrackPOSDevices()
    {
        // Arrange
        var store = new Store(_name, _code, _address, _phone, _email, _timeZone);
        var mainPOS = new StoreDevice("POS001", "POS", "Main POS Terminal");
        var backupPOS = new StoreDevice("POS002", "POS", "Backup POS Terminal");
        var kitchenPrinter = new StoreDevice("PRN001", "Printer", "Kitchen Printer");

        // Act
        store.AddDevice(mainPOS);
        store.AddDevice(backupPOS);
        store.AddDevice(kitchenPrinter);

        // Assert
        store.Devices.Should().HaveCount(3);
        store.Devices.Should().Contain(mainPOS);
        store.Devices.Should().Contain(backupPOS);
        store.Devices.Should().Contain(kitchenPrinter);
    }

    [Fact]
    public void WhenUpdatingStoreSettings_ThenStoreCanConfigureBusinessRules()
    {
        // Arrange
        var store = new Store(_name, _code, _address, _phone, _email, _timeZone);
        var settings = new StoreSettings(
            enableTax: true,
            taxRate: 0.1m,
            enableDiscounts: true,
            maxDiscountPercentage: 20,
            requireCustomerInfo: true,
            enableLoyaltyProgram: true,
            receiptHeader: "Welcome to Test Store",
            receiptFooter: "Thank you for shopping with us",
            enableStockAlerts: true,
            enableEmailNotifications: true,
            enableSMSNotifications: true,
            emailTemplate: "Order confirmation template",
            smsTemplate: "Order status update template");

        // Act
        store.UpdateSettings(settings);

        // Assert
        store.Settings.Should().Be(settings);
        store.Settings.EnableTax.Should().BeTrue();
        store.Settings.TaxRate.Should().Be(0.1m);
        store.Settings.EnableDiscounts.Should().BeTrue();
        store.Settings.MaxDiscountPercentage.Should().Be(20);
        store.Settings.RequireCustomerInfo.Should().BeTrue();
        store.Settings.EnableLoyaltyProgram.Should().BeTrue();
        store.Settings.ReceiptHeader.Should().Be("Welcome to Test Store");
        store.Settings.ReceiptFooter.Should().Be("Thank you for shopping with us");
        store.Settings.EnableStockAlerts.Should().BeTrue();
        store.Settings.EnableEmailNotifications.Should().BeTrue();
        store.Settings.EnableSMSNotifications.Should().BeTrue();
        store.Settings.EmailTemplate.Should().Be("Order confirmation template");
        store.Settings.SMSTemplate.Should().Be("Order status update template");
    }

    #endregion
} 
