using FluentAssertions;
using POS.Domain.AggregatesModel.StaffAggregate;
using POS.Domain.Common.ValueObjects;
using POS.Domain.Enums;
using POS.Domain.Exceptions;
using Xunit;

namespace POS.Domain.Tests.AggregatesModel.StaffAggregate;

public class StaffTests
{
    private readonly string _name;
    private readonly string _email;
    private readonly string _phone;
    private readonly string _role;
    private readonly string _pin;
    private readonly Guid _storeId;
    private readonly StaffSchedule _schedule;
    private readonly StaffPermissions _permissions;

    public StaffTests()
    {
        _name = "John Doe";
        _email = "john.doe@example.com";
        _phone = "1234567890";
        _role = "Cashier";
        _pin = "1234";
        _storeId = Guid.NewGuid();
        _schedule = new StaffSchedule(DayOfWeek.Monday, new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0), true);
        _permissions = new StaffPermissions(
            canManageInventory: true,
            canManageStaff: false,
            canManageProducts: true,
            canProcessRefunds: true,
            canViewReports: true,
            canManageSettings: false);
    }

    [Fact]
    public void Constructor_WithValidParameters_CreatesStaffSuccessfully()
    {
        // Act
        var staff = new Staff(_name, _email, _phone, _role, _pin, _storeId, _schedule, _permissions);

        // Assert
        staff.Name.Should().Be(_name);
        staff.Email.Should().Be(_email);
        staff.Phone.Should().Be(_phone);
        staff.Role.Should().Be(_role);
        staff.PIN.Should().Be(_pin);
        staff.StoreId.Should().Be(_storeId);
        staff.Schedule.Should().Be(_schedule);
        staff.Permissions.Should().Be(_permissions);
        staff.IsActive.Should().BeTrue();
        staff.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UpdateDetails_WithValidParameters_UpdatesSuccessfully()
    {
        // Arrange
        var staff = new Staff(_name, _email, _phone, _role, _pin, _storeId, _schedule, _permissions);
        var newName = "Jane Doe";
        var newEmail = "jane.doe@example.com";
        var newPhone = "0987654321";
        var newRole = "Manager";
        var newPin = "5678";

        // Act
        staff.UpdateDetails(newName, newEmail, newPhone, newRole, newPin);

        // Assert
        staff.Name.Should().Be(newName);
        staff.Email.Should().Be(newEmail);
        staff.Phone.Should().Be(newPhone);
        staff.Role.Should().Be(newRole);
        staff.PIN.Should().Be(newPin);
        staff.LastModifiedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UpdateSchedule_WithValidSchedule_UpdatesSuccessfully()
    {
        // Arrange
        var staff = new Staff(_name, _email, _phone, _role, _pin, _storeId, _schedule, _permissions);
        var newSchedule = new StaffSchedule(DayOfWeek.Tuesday, new TimeSpan(10, 0, 0), new TimeSpan(18, 0, 0), true);

        // Act
        staff.UpdateSchedule(newSchedule);

        // Assert
        staff.Schedule.Should().Be(newSchedule);
        staff.LastModifiedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UpdatePermissions_WithValidPermissions_UpdatesSuccessfully()
    {
        // Arrange
        var staff = new Staff(_name, _email, _phone, _role, _pin, _storeId, _schedule, _permissions);
        var newPermissions = new StaffPermissions(
            canManageInventory: true,
            canManageStaff: true,
            canManageProducts: true,
            canProcessRefunds: true,
            canViewReports: true,
            canManageSettings: true);

        // Act
        staff.UpdatePermissions(newPermissions);

        // Assert
        staff.Permissions.Should().Be(newPermissions);
        staff.LastModifiedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Deactivate_WhenActive_DeactivatesSuccessfully()
    {
        // Arrange
        var staff = new Staff(_name, _email, _phone, _role, _pin, _storeId, _schedule, _permissions);

        // Act
        staff.Deactivate();

        // Assert
        staff.IsActive.Should().BeFalse();
        staff.LastModifiedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Reactivate_WhenInactive_ReactivatesSuccessfully()
    {
        // Arrange
        var staff = new Staff(_name, _email, _phone, _role, _pin, _storeId, _schedule, _permissions);
        staff.Deactivate();

        // Act
        staff.Reactivate();

        // Assert
        staff.IsActive.Should().BeTrue();
        staff.LastModifiedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
} 
