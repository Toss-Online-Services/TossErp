using System;
using TossErp.Domain.SeedWork;

namespace TossErp.Domain.AggregatesModel.StokvelAggregate
{
    public class StokvelMember : Entity
    {
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public string IdNumber { get; private set; } = string.Empty;
        public string PhoneNumber { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string Address { get; private set; } = string.Empty;
        public int RotationOrder { get; private set; }
        public DateTime JoinDate { get; private set; }
        public DateTime? ExitDate { get; private set; }
        public string? ExitReason { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastModifiedAt { get; private set; }

        protected StokvelMember() { }

        public StokvelMember(
            string firstName,
            string lastName,
            string idNumber,
            string phoneNumber,
            string email,
            string address)
        {
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            IdNumber = idNumber ?? throw new ArgumentNullException(nameof(idNumber));
            PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            Address = address ?? throw new ArgumentNullException(nameof(address));
            RotationOrder = 0; // Will be set by the stokvel
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
            Address = address ?? throw new ArgumentNullException(nameof(address));
            LastModifiedAt = DateTime.UtcNow;
        }

        public void ExitStokvel(DateTime exitDate, string? reason = null)
        {
            if (!IsActive)
                throw new InvalidOperationException("Member is already inactive.");

            ExitDate = exitDate;
            ExitReason = reason;
            IsActive = false;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void SetRotationOrder(int rotationOrder)
        {
            RotationOrder = rotationOrder;
            LastModifiedAt = DateTime.UtcNow;
        }

        public TimeSpan GetMembershipDuration()
        {
            var endDate = ExitDate ?? DateTime.UtcNow;
            return endDate - JoinDate;
        }

        public bool IsLongTermMember(int yearsThreshold = 2)
        {
            return GetMembershipDuration().TotalDays >= yearsThreshold * 365;
        }

        public bool IsNewMember(int monthsThreshold = 6)
        {
            return GetMembershipDuration().TotalDays <= monthsThreshold * 30;
        }
    }
} 
