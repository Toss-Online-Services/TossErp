using System;
using TossErp.Domain.SeedWork;

namespace TossErp.Domain.AggregatesModel.CooperativeAggregate
{
    public class CooperativeMember : Entity
    {
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public string IdNumber { get; private set; } = string.Empty;
        public string PhoneNumber { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string Address { get; private set; } = string.Empty;
        public decimal ShareValue { get; private set; }
        public string Role { get; private set; } = string.Empty;
        public DateTime JoinDate { get; private set; }
        public DateTime? ExitDate { get; private set; }
        public string? ExitReason { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastModifiedAt { get; private set; }

        protected CooperativeMember() { }

        public CooperativeMember(
            string firstName,
            string lastName,
            string idNumber,
            string phoneNumber,
            string email,
            decimal shareValue,
            string role)
        {
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            IdNumber = idNumber ?? throw new ArgumentNullException(nameof(idNumber));
            PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            ShareValue = shareValue;
            Role = role ?? throw new ArgumentNullException(nameof(role));
            JoinDate = DateTime.UtcNow;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdateDetails(
            string firstName,
            string lastName,
            string phoneNumber,
            string email,
            string address)
        {
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            Address = address ?? string.Empty;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void UpdateShareValue(decimal newShareValue)
        {
            if (newShareValue < 0)
                throw new ArgumentException("Share value cannot be negative.");

            ShareValue = newShareValue;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void UpdateRole(string newRole)
        {
            Role = newRole ?? throw new ArgumentNullException(nameof(newRole));
            LastModifiedAt = DateTime.UtcNow;
        }

        public void ExitCooperative(DateTime exitDate, string? reason = null)
        {
            if (!IsActive)
                throw new InvalidOperationException("Member is already inactive.");

            ExitDate = exitDate;
            ExitReason = reason;
            IsActive = false;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void ExitCooperative()
        {
            ExitCooperative(DateTime.UtcNow);
        }

        public TimeSpan GetMembershipDuration()
        {
            var endDate = ExitDate ?? DateTime.UtcNow;
            return endDate - JoinDate;
        }

        public bool IsLongTermMember(int yearsThreshold = 5)
        {
            return GetMembershipDuration().TotalDays >= yearsThreshold * 365;
        }

        public bool HasShareValue => ShareValue > 0;

        public string FullName => $"{FirstName} {LastName}".Trim();
    }
} 
