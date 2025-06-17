using POS.Domain.Common;
using POS.Domain.Models;
using POS.Domain.AggregatesModel.StaffAggregate.Events;

namespace POS.Domain.AggregatesModel.StaffAggregate
{
    public class Staff : AggregateRoot
    {
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string Phone { get; private set; }
        public string Role { get; private set; }
        public Guid StoreId { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastModifiedAt { get; private set; }
        public StaffSchedule Schedule { get; private set; }
        public StaffPermissions Permissions { get; private set; }

        private Staff()
        {
            Name = string.Empty;
            Email = string.Empty;
            Phone = string.Empty;
            Role = string.Empty;
            StoreId = Guid.Empty;
            IsActive = false;
            CreatedAt = DateTime.UtcNow;
            Schedule = new StaffSchedule();
            Permissions = new StaffPermissions();
        }

        public Staff(
            string name,
            string email,
            string phone,
            string role,
            Guid storeId,
            StaffSchedule schedule,
            StaffPermissions permissions)
        {
            Name = name;
            Email = email;
            Phone = phone;
            Role = role;
            StoreId = storeId;
            Schedule = schedule;
            Permissions = permissions;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
            AddDomainEvent(new StaffCreatedDomainEvent(Id, name, email, role));
        }

        public void UpdateDetails(
            string name,
            string email,
            string phone,
            string role)
        {
            Name = name;
            Email = email;
            Phone = phone;
            Role = role;
            LastModifiedAt = DateTime.UtcNow;
            AddDomainEvent(new StaffUpdatedDomainEvent(Id, name, email, role));
        }

        public void UpdateSchedule(StaffSchedule schedule)
        {
            Schedule = schedule;
            LastModifiedAt = DateTime.UtcNow;
            AddDomainEvent(new StaffScheduleUpdatedDomainEvent(Id));
        }

        public void UpdatePermissions(StaffPermissions permissions)
        {
            Permissions = permissions;
            LastModifiedAt = DateTime.UtcNow;
            AddDomainEvent(new StaffPermissionsUpdatedDomainEvent(Id));
        }

        public void Deactivate()
        {
            IsActive = false;
            LastModifiedAt = DateTime.UtcNow;
            AddDomainEvent(new StaffDeactivatedDomainEvent(Id));
        }

        public void Reactivate()
        {
            IsActive = true;
            LastModifiedAt = DateTime.UtcNow;
            AddDomainEvent(new StaffReactivatedDomainEvent(Id));
        }
    }

    public class StaffSchedule
    {
        public DayOfWeek Day { get; private set; }
        public TimeSpan StartTime { get; private set; }
        public TimeSpan EndTime { get; private set; }
        public bool IsWorking { get; private set; }

        public StaffSchedule()
        {
            Day = DayOfWeek.Monday;
            StartTime = TimeSpan.Zero;
            EndTime = TimeSpan.Zero;
            IsWorking = false;
        }

        public StaffSchedule(DayOfWeek day, TimeSpan startTime, TimeSpan endTime, bool isWorking)
        {
            Day = day;
            StartTime = startTime;
            EndTime = endTime;
            IsWorking = isWorking;
        }
    }

    public class StaffPermissions
    {
        public bool CanManageInventory { get; private set; }
        public bool CanManageStaff { get; private set; }
        public bool CanManageProducts { get; private set; }
        public bool CanProcessRefunds { get; private set; }
        public bool CanViewReports { get; private set; }
        public bool CanManageSettings { get; private set; }

        public StaffPermissions()
        {
            CanManageInventory = false;
            CanManageStaff = false;
            CanManageProducts = false;
            CanProcessRefunds = false;
            CanViewReports = false;
            CanManageSettings = false;
        }

        public StaffPermissions(
            bool canManageInventory,
            bool canManageStaff,
            bool canManageProducts,
            bool canProcessRefunds,
            bool canViewReports,
            bool canManageSettings)
        {
            CanManageInventory = canManageInventory;
            CanManageStaff = canManageStaff;
            CanManageProducts = canManageProducts;
            CanProcessRefunds = canProcessRefunds;
            CanViewReports = canViewReports;
            CanManageSettings = canManageSettings;
        }
    }
} 
