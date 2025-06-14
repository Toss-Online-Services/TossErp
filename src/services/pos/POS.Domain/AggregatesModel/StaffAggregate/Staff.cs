using System;
using TossErp.POS.Domain.SeedWork;

namespace TossErp.POS.Domain.AggregatesModel.StaffAggregate
{
    public class Staff : Entity, IAggregateRoot
    {
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string Phone { get; private set; }
        public string? Address { get; private set; }
        public string Role { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        protected Staff()
        {
            Name = string.Empty;
            Email = string.Empty;
            Phone = string.Empty;
            Role = string.Empty;
            CreatedAt = DateTime.UtcNow;
        }

        public Staff(string name, string email, string phone, string role, string? address = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Name cannot be empty");
            if (string.IsNullOrWhiteSpace(email))
                throw new DomainException("Email cannot be empty");
            if (string.IsNullOrWhiteSpace(phone))
                throw new DomainException("Phone cannot be empty");
            if (string.IsNullOrWhiteSpace(role))
                throw new DomainException("Role cannot be empty");

            Name = name;
            Email = email;
            Phone = phone;
            Role = role;
            Address = address;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        public void Update(string name, string email, string phone, string role, string? address = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Name cannot be empty");
            if (string.IsNullOrWhiteSpace(email))
                throw new DomainException("Email cannot be empty");
            if (string.IsNullOrWhiteSpace(phone))
                throw new DomainException("Phone cannot be empty");
            if (string.IsNullOrWhiteSpace(role))
                throw new DomainException("Role cannot be empty");

            Name = name;
            Email = email;
            Phone = phone;
            Role = role;
            Address = address;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }
} 
