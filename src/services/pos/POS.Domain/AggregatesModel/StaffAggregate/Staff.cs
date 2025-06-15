using POS.Domain.AggregatesModel.StoreAggregate;
using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.StaffAggregate
{
    public class Staff : AggregateRoot
    {
        public Guid StoreId { get; private set; }
        public Store Store { get; private set; } = null!;
        public string Name { get; private set; }
        public string? Email { get; private set; }
        public string? Phone { get; private set; }
        public string? Position { get; private set; }
        public string? Department { get; private set; }
        public string? EmployeeId { get; private set; }
        public string? TaxId { get; private set; }
        public string? Notes { get; private set; }
        public string Status { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsSynced { get; private set; }
        public DateTime? SyncedAt { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        protected Staff()
        {
            Name = string.Empty;
            Status = "Active";
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
        }

        public Staff(Guid storeId, string name, string? email = null, string? phone = null, string? position = null,
            string? department = null, string? employeeId = null, string? taxId = null, string? notes = null)
        {
            if (storeId == Guid.Empty)
                throw new DomainException("Store ID cannot be empty");
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Name cannot be empty");

            StoreId = storeId;
            Name = name;
            Email = email;
            Phone = phone;
            Position = position;
            Department = department;
            EmployeeId = employeeId;
            TaxId = taxId;
            Notes = notes;
            Status = "Active";
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        public void Update(string name, string? email = null, string? phone = null, string? position = null,
            string? department = null, string? employeeId = null, string? taxId = null, string? notes = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Name cannot be empty");

            Name = name;
            Email = email;
            Phone = phone;
            Position = position;
            Department = department;
            EmployeeId = employeeId;
            TaxId = taxId;
            Notes = notes;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            if (!IsActive)
                throw new DomainException("Staff is already inactive");

            IsActive = false;
            Status = "Inactive";
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            if (IsActive)
                throw new DomainException("Staff is already active");

            IsActive = true;
            Status = "Active";
            UpdatedAt = DateTime.UtcNow;
        }
    }
} 
