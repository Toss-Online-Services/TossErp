using POS.Domain.Exceptions;
using POS.Domain.SeedWork;
using POS.Domain.Models;
using POS.Domain.Common;
using POS.Domain.AggregatesModel.StoreAggregate.Events;

namespace POS.Domain.AggregatesModel.StoreAggregate;

public class Store : AggregateRoot
{
    public new int Id { get; set; }
    public string Name { get; private set; }
    public string Address { get; private set; }
    public string Phone { get; private set; }
    public string Email { get; private set; }
    public string? Website { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public string? TaxId { get; private set; }
    public string? LicenseNumber { get; private set; }
    public string? LogoUrl { get; private set; }
    public string? BannerUrl { get; private set; }
    public string? SocialMediaLinks { get; private set; }
    public StoreSettings Settings { get; private set; }
    public List<StoreHours> OperatingHours { get; private set; }
    public string TimeZone { get; private set; }
    public List<StoreDevice> Devices { get; private set; }
    public List<StorePrinter> Printers { get; private set; }

    private Store()
    {
        Name = string.Empty;
        Address = string.Empty;
        Phone = string.Empty;
        Email = string.Empty;
        TimeZone = string.Empty;
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
        Settings = new StoreSettings();
        OperatingHours = new List<StoreHours>();
        Devices = new List<StoreDevice>();
        Printers = new List<StorePrinter>();
    }

    public Store(
        string name,
        string address,
        string phone,
        string email,
        string timeZone,
        string? website = null,
        string? description = null,
        string? taxId = null,
        string? licenseNumber = null,
        string? logoUrl = null,
        string? bannerUrl = null,
        string? socialMediaLinks = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Name cannot be empty");
        if (string.IsNullOrWhiteSpace(address))
            throw new DomainException("Address cannot be empty");
        if (string.IsNullOrWhiteSpace(phone))
            throw new DomainException("Phone cannot be empty");
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email cannot be empty");
        if (string.IsNullOrWhiteSpace(timeZone))
            throw new DomainException("TimeZone cannot be empty");

        Name = name;
        Address = address;
        Phone = phone;
        Email = email;
        Website = website;
        Description = description;
        TaxId = taxId;
        LicenseNumber = licenseNumber;
        LogoUrl = logoUrl;
        BannerUrl = bannerUrl;
        SocialMediaLinks = socialMediaLinks;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        TimeZone = timeZone;
        Settings = new StoreSettings();
        OperatingHours = new List<StoreHours>();
        Devices = new List<StoreDevice>();
        Printers = new List<StorePrinter>();

        AddDomainEvent(new StoreCreatedDomainEvent(Id, name, address));
    }

    public void UpdateDetails(
        string name,
        string address,
        string phone,
        string email,
        string? website = null,
        string? description = null,
        string? taxId = null,
        string? licenseNumber = null,
        string? logoUrl = null,
        string? bannerUrl = null,
        string? socialMediaLinks = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Name cannot be empty");
        if (string.IsNullOrWhiteSpace(address))
            throw new DomainException("Address cannot be empty");
        if (string.IsNullOrWhiteSpace(phone))
            throw new DomainException("Phone cannot be empty");
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email cannot be empty");

        Name = name;
        Address = address;
        Phone = phone;
        Email = email;
        Website = website;
        Description = description;
        TaxId = taxId;
        LicenseNumber = licenseNumber;
        LogoUrl = logoUrl;
        BannerUrl = bannerUrl;
        SocialMediaLinks = socialMediaLinks;
        LastModifiedAt = DateTime.UtcNow;

        AddDomainEvent(new StoreUpdatedDomainEvent(Id, name, address));
    }

    public void UpdateSettings(StoreSettings settings)
    {
        Settings = settings;
        LastModifiedAt = DateTime.UtcNow;

        AddDomainEvent(new StoreSettingsUpdatedDomainEvent(Id));
    }

    public void AddOperatingHours(DayOfWeek day, TimeSpan openTime, TimeSpan closeTime)
    {
        var hours = OperatingHours.FirstOrDefault(h => h.Day == day);
        if (hours != null)
        {
            hours.UpdateHours(openTime, closeTime);
        }
        else
        {
            hours = new StoreHours(day, openTime, closeTime);
            OperatingHours.Add(hours);
        }
        LastModifiedAt = DateTime.UtcNow;

        AddDomainEvent(new StoreOperatingHoursUpdatedDomainEvent(Id, day));
    }

    public void RemoveOperatingHours(DayOfWeek day)
    {
        var hours = OperatingHours.FirstOrDefault(h => h.Day == day);
        if (hours != null)
        {
            OperatingHours.Remove(hours);
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new StoreOperatingHoursRemovedDomainEvent(Id, day));
        }
    }

    public void AddDevice(string deviceId, string deviceType, string? name = null)
    {
        var device = new StoreDevice(deviceId, deviceType, name);
        Devices.Add(device);
        LastModifiedAt = DateTime.UtcNow;

        AddDomainEvent(new StoreDeviceAddedDomainEvent(Id, deviceId, deviceType));
    }

    public void RemoveDevice(string deviceId)
    {
        var device = Devices.FirstOrDefault(d => d.DeviceId == deviceId);
        if (device != null)
        {
            Devices.Remove(device);
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new StoreDeviceRemovedDomainEvent(Id, deviceId));
        }
    }

    public void AddPrinter(string printerId, string printerType, string? name = null)
    {
        var printer = new StorePrinter(printerId, printerType, name);
        Printers.Add(printer);
        LastModifiedAt = DateTime.UtcNow;

        AddDomainEvent(new StorePrinterAddedDomainEvent(Id, printerId, printerType));
    }

    public void RemovePrinter(string printerId)
    {
        var printer = Printers.FirstOrDefault(p => p.PrinterId == printerId);
        if (printer != null)
        {
            Printers.Remove(printer);
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new StorePrinterRemovedDomainEvent(Id, printerId));
        }
    }

    public bool IsOpen()
    {
        var now = DateTime.UtcNow;
        var localTime = TimeZoneInfo.ConvertTimeFromUtc(now, TimeZoneInfo.FindSystemTimeZoneById(TimeZone));
        var dayOfWeek = localTime.DayOfWeek;
        var timeOfDay = localTime.TimeOfDay;

        var hours = OperatingHours.FirstOrDefault(h => h.Day == dayOfWeek);
        return hours != null && timeOfDay >= hours.OpenTime && timeOfDay <= hours.CloseTime;
    }

    public void Deactivate()
    {
        if (!IsActive)
            throw new DomainException("Store is already inactive");

        IsActive = false;
        LastModifiedAt = DateTime.UtcNow;

        AddDomainEvent(new StoreDeactivatedDomainEvent(Id));
    }

    public void Reactivate()
    {
        if (IsActive)
            throw new DomainException("Store is already active");

        IsActive = true;
        LastModifiedAt = DateTime.UtcNow;

        AddDomainEvent(new StoreReactivatedDomainEvent(Id));
    }
}

public class StoreSettings
{
    public bool EnableTax { get; private set; }
    public decimal TaxRate { get; private set; }
    public bool EnableDiscounts { get; private set; }
    public int MaxDiscountPercentage { get; private set; }
    public bool RequireCustomerInfo { get; private set; }
    public bool EnableLoyaltyProgram { get; private set; }
    public string ReceiptHeader { get; private set; }
    public string ReceiptFooter { get; private set; }
    public bool EnableStockAlerts { get; private set; }
    public bool EnableEmailNotifications { get; private set; }
    public bool EnableSMSNotifications { get; private set; }
    public string? EmailTemplate { get; private set; }
    public string? SMSTemplate { get; private set; }

    public StoreSettings()
    {
        EnableTax = false;
        TaxRate = 0;
        EnableDiscounts = false;
        MaxDiscountPercentage = 0;
        RequireCustomerInfo = false;
        EnableLoyaltyProgram = false;
        ReceiptHeader = string.Empty;
        ReceiptFooter = string.Empty;
        EnableStockAlerts = false;
        EnableEmailNotifications = false;
        EnableSMSNotifications = false;
        EmailTemplate = null;
        SMSTemplate = null;
    }

    public StoreSettings(
        bool enableTax,
        decimal taxRate,
        bool enableDiscounts,
        int maxDiscountPercentage,
        bool requireCustomerInfo,
        bool enableLoyaltyProgram,
        string receiptHeader,
        string receiptFooter,
        bool enableStockAlerts = false,
        bool enableEmailNotifications = false,
        bool enableSMSNotifications = false,
        string? emailTemplate = null,
        string? smsTemplate = null)
    {
        EnableTax = enableTax;
        TaxRate = taxRate;
        EnableDiscounts = enableDiscounts;
        MaxDiscountPercentage = maxDiscountPercentage;
        RequireCustomerInfo = requireCustomerInfo;
        EnableLoyaltyProgram = enableLoyaltyProgram;
        ReceiptHeader = receiptHeader;
        ReceiptFooter = receiptFooter;
        EnableStockAlerts = enableStockAlerts;
        EnableEmailNotifications = enableEmailNotifications;
        EnableSMSNotifications = enableSMSNotifications;
        EmailTemplate = emailTemplate;
        SMSTemplate = smsTemplate;
    }
}

public class StoreHours
{
    public DayOfWeek Day { get; private set; }
    public TimeSpan OpenTime { get; private set; }
    public TimeSpan CloseTime { get; private set; }
    public bool IsOpen { get; private set; }

    private StoreHours() { } // For EF Core

    public StoreHours(DayOfWeek day, TimeSpan openTime, TimeSpan closeTime, bool isOpen = true)
    {
        Day = day;
        OpenTime = openTime;
        CloseTime = closeTime;
        IsOpen = isOpen;
    }

    public void UpdateHours(TimeSpan openTime, TimeSpan closeTime)
    {
        OpenTime = openTime;
        CloseTime = closeTime;
    }
}

public class StoreDevice
{
    public string DeviceId { get; private set; }
    public string Name { get; private set; }
    public string Type { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime LastSeen { get; private set; }

    private StoreDevice()
    {
        DeviceId = string.Empty;
        Name = string.Empty;
        Type = string.Empty;
        IsActive = true;
        LastSeen = DateTime.UtcNow;
    }

    public StoreDevice(string deviceId, string type, string? name = null)
    {
        DeviceId = deviceId;
        Type = type;
        Name = name ?? type;
        IsActive = true;
        LastSeen = DateTime.UtcNow;
    }

    public void UpdateLastSeen()
    {
        LastSeen = DateTime.UtcNow;
    }
}

public class StorePrinter
{
    public string PrinterId { get; private set; }
    public string Name { get; private set; }
    public string Type { get; private set; }
    public bool IsDefault { get; private set; }
    public bool IsActive { get; private set; }

    private StorePrinter()
    {
        PrinterId = string.Empty;
        Name = string.Empty;
        Type = string.Empty;
        IsDefault = false;
        IsActive = true;
    }

    public StorePrinter(string printerId, string type, string? name = null)
    {
        PrinterId = printerId;
        Type = type;
        Name = name ?? type;
        IsDefault = false;
        IsActive = true;
    }
} 
