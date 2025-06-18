using POS.Domain.Exceptions;
using POS.Domain.SeedWork;
using POS.Domain.Models;
using POS.Domain.Common;
using POS.Domain.AggregatesModel.StoreAggregate.Events;
using POS.Domain.Common.Events;
using POS.Domain.ValueObjects;

namespace POS.Domain.AggregatesModel.StoreAggregate;

public class Store : AggregateRoot
{
    public string Name { get; private set; }
    public string Code { get; private set; }
    public POS.Domain.Common.ValueObjects.Address Address { get; private set; }
    public string Phone { get; private set; }
    public string Email { get; private set; }
    public string? Website { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public string? TaxId { get; private set; }
    public string? LicenseNumber { get; private set; }
    public string? LogoUrl { get; private set; }
    public string? BannerUrl { get; private set; }
    public string? SocialMediaLinks { get; private set; }
    public StoreSettings Settings { get; private set; }
    public string TimeZone { get; private set; }

    private readonly List<StoreHours> _storeHours = new();
    public IReadOnlyCollection<StoreHours> StoreHours => _storeHours.AsReadOnly();

    private readonly List<StoreDevice> _devices = new();
    public IReadOnlyCollection<StoreDevice> Devices => _devices.AsReadOnly();

    private readonly List<StorePrinter> _printers = new();
    public IReadOnlyCollection<StorePrinter> Printers => _printers.AsReadOnly();

    private Store()
    {
        Name = string.Empty;
        Code = string.Empty;
        Address = new POS.Domain.Common.ValueObjects.Address();
        Phone = string.Empty;
        Email = string.Empty;
        Website = string.Empty;
        Description = string.Empty;
        TaxId = string.Empty;
        LicenseNumber = string.Empty;
        LogoUrl = string.Empty;
        BannerUrl = string.Empty;
        SocialMediaLinks = string.Empty;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        TimeZone = string.Empty;
        Settings = new StoreSettings();
    }

    public Store(
        string name,
        string code,
        POS.Domain.Common.ValueObjects.Address address,
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
            throw new DomainException("Store name cannot be empty");
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Store code cannot be empty");
        if (string.IsNullOrWhiteSpace(phone))
            throw new DomainException("Store phone cannot be empty");
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Store email cannot be empty");
        if (string.IsNullOrWhiteSpace(timeZone))
            throw new DomainException("TimeZone cannot be empty");        

        Name = name;
        Code = code;
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
    }

    public void UpdateDetails(
        string name,
        string code,
        POS.Domain.Common.ValueObjects.Address address,
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
            throw new DomainException("Store name cannot be empty");
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Store code cannot be empty");
        if (string.IsNullOrWhiteSpace(phone))
            throw new DomainException("Store phone cannot be empty");
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Store email cannot be empty");       

        Name = name;
        Code = code;
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
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new StoreUpdatedDomainEvent(Id, name, address.ToString(), DateTime.UtcNow));
    }

    public void UpdateSettings(StoreSettings settings)
    {
        if (settings == null)
            throw new DomainException("Store settings cannot be null");
        Settings = settings;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddStoreHours(StoreHours storeHours)
    {
        if (_storeHours.Any(h => h.Day == storeHours.Day))
            throw new DomainException($"Store hours for {storeHours.Day} already exist");

        _storeHours.Add(storeHours);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateStoreHours(StoreHours storeHours)
    {
        var existingHours = _storeHours.FirstOrDefault(h => h.Day == storeHours.Day);
        if (existingHours == null)
            throw new DomainException($"Store hours for {storeHours.Day} do not exist");

        existingHours.UpdateHours(storeHours.OpenTime, storeHours.CloseTime);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveStoreHours(string day)
    {
        if (!Enum.TryParse<DayOfWeek>(day, out var dayOfWeek))
            throw new DomainException($"Invalid day: {day}");

        var storeHours = _storeHours.FirstOrDefault(h => h.Day == dayOfWeek);
        if (storeHours == null)
            throw new DomainException($"Store hours for {day} do not exist");

        _storeHours.Remove(storeHours);
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddDevice(StoreDevice device)
    {
        if (_devices.Any(d => d.DeviceId == device.DeviceId))
            throw new DomainException($"Device with ID {device.DeviceId} already exists");

        _devices.Add(device);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDevice(StoreDevice device)
    {
        var existingDevice = _devices.FirstOrDefault(d => d.DeviceId == device.DeviceId);
        if (existingDevice == null)
            throw new DomainException($"Device with ID {device.DeviceId} does not exist");

        existingDevice.UpdateName(device.Name);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveDevice(string deviceId)
    {
        var device = _devices.FirstOrDefault(d => d.DeviceId == deviceId);
        if (device == null)
            throw new DomainException($"Device with ID {deviceId} does not exist");

        _devices.Remove(device);
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddPrinter(StorePrinter printer)
    {
        if (_printers.Any(p => p.PrinterId == printer.PrinterId))
            throw new DomainException($"Printer with ID {printer.PrinterId} already exists");

        _printers.Add(printer);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePrinter(StorePrinter printer)
    {
        var existingPrinter = _printers.FirstOrDefault(p => p.PrinterId == printer.PrinterId);
        if (existingPrinter == null)
            throw new DomainException($"Printer with ID {printer.PrinterId} does not exist");

        existingPrinter.UpdateName(printer.Name);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemovePrinter(string printerId)
    {
        var printer = _printers.FirstOrDefault(p => p.PrinterId == printerId);
        if (printer == null)
            throw new DomainException($"Printer with ID {printerId} does not exist");

        _printers.Remove(printer);
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        if (!IsActive)
            throw new DomainException("Store is already deactivated");

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new StoreDeactivatedDomainEvent(Id, DateTime.UtcNow));
    }

    public void Activate()
    {
        if (IsActive)
            throw new DomainException("Store is already active");

        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new StoreReactivatedDomainEvent(Id, DateTime.UtcNow));
    }
}
