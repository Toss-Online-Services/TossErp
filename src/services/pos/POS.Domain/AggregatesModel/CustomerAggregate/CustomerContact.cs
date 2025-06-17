using POS.Domain.Common;
using POS.Domain.Exceptions;

namespace POS.Domain.AggregatesModel.CustomerAggregate
{
    public class CustomerContact : Entity
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public string PhoneNumber { get; private set; }
        public string? Title { get; private set; }
        public string? Department { get; private set; }
        public bool IsPrimary { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastModifiedAt { get; private set; }

        private CustomerContact()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            Email = string.Empty;
            PhoneNumber = string.Empty;
            IsPrimary = false;
            CreatedAt = DateTime.UtcNow;
        }

        public CustomerContact(string firstName, string lastName, string email, string phoneNumber, 
            string? title = null, string? department = null, bool isPrimary = false)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new DomainException("First name cannot be empty");
            if (string.IsNullOrWhiteSpace(lastName))
                throw new DomainException("Last name cannot be empty");
            if (string.IsNullOrWhiteSpace(email))
                throw new DomainException("Email cannot be empty");
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new DomainException("Phone number cannot be empty");

            Id = Guid.NewGuid();
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
            Title = title;
            Department = department;
            IsPrimary = isPrimary;
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdateContactInfo(string firstName, string lastName, string email, string phoneNumber, 
            string? title = null, string? department = null)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new DomainException("First name cannot be empty");
            if (string.IsNullOrWhiteSpace(lastName))
                throw new DomainException("Last name cannot be empty");
            if (string.IsNullOrWhiteSpace(email))
                throw new DomainException("Email cannot be empty");
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new DomainException("Phone number cannot be empty");

            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
            Title = title;
            Department = department;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void SetAsPrimary()
        {
            IsPrimary = true;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void SetAsSecondary()
        {
            IsPrimary = false;
            LastModifiedAt = DateTime.UtcNow;
        }

        public string FullName => $"{FirstName} {LastName}";
    }
} 
