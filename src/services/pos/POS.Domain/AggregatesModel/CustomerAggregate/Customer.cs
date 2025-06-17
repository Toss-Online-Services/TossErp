using POS.Domain.Common;
using POS.Domain.Models;
using POS.Domain.AggregatesModel.CustomerAggregate.Events;

namespace POS.Domain.AggregatesModel.CustomerAggregate
{
    public class Customer : Entity
    {
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string Phone { get; private set; }
        public string? Address { get; private set; }
        public Guid StoreId { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastPurchaseAt { get; private set; }
        public decimal TotalSpent { get; private set; }
        public int PurchaseCount { get; private set; }
        public bool IsActive { get; private set; }

        private Customer()
        {
            Name = string.Empty;
            Email = string.Empty;
            Phone = string.Empty;
            StoreId = Guid.Empty;
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
            TotalSpent = 0;
            PurchaseCount = 0;
        }

        public Customer(string name, string email, string phone, Guid storeId, string? address = null)
        {
            Name = name;
            Email = email;
            Phone = phone;
            StoreId = storeId;
            Address = address;
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
            TotalSpent = 0;
            PurchaseCount = 0;
            AddDomainEvent(new CustomerCreatedDomainEvent(Id, name, email));
        }

        public void UpdateContactInfo(string name, string email, string phone, string? address)
        {
            Name = name;
            Email = email;
            Phone = phone;
            Address = address;
            AddDomainEvent(new CustomerUpdatedDomainEvent(Id, name, email, phone, address));
        }

        public void RecordPurchase(decimal amount)
        {
            TotalSpent += amount;
            PurchaseCount++;
            LastPurchaseAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            AddDomainEvent(new CustomerDeactivatedDomainEvent(Id));
        }

        public void Reactivate()
        {
            IsActive = true;
            AddDomainEvent(new CustomerReactivatedDomainEvent(Id));
        }
    }
} 
