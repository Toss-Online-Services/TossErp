#nullable enable
using eShop.POS.Domain.SeedWork;

namespace eShop.POS.Domain.AggregatesModel.StaffAggregate
{
    public class Staff : Entity, IAggregateRoot
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string Role { get; set; } = string.Empty;
        public string StoreId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string Pin { get; private set; } = string.Empty;

        protected Staff() {
            StoreId = string.Empty;
            Name = string.Empty;
            Email = string.Empty;
            Role = string.Empty;
            CreatedAt = DateTime.UtcNow;
            Pin = string.Empty;
        }

        public Staff(string storeId, string name, string email, string role)
        {
            StoreId = storeId;
            Name = name;
            Email = email;
            Role = role;
            CreatedAt = DateTime.UtcNow;
            Pin = string.Empty;
        }

        public void UpdateDetails(string name, string? phone)
        {
            Name = name;
            Phone = phone;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateRole(string role)
        {
            Role = role;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetPin(string pin)
        {
            Pin = pin;
            UpdatedAt = DateTime.UtcNow;
        }

        public bool ValidatePin(string pin)
        {
            return Pin == pin;
        }
    }
} 
