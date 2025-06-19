using System;
using System.Collections.Generic;
using System.Linq;
using TossErp.Domain.SeedWork;
using TossErp.Identity.Domain.Events;

namespace TossErp.Identity.Domain.AggregatesModel.UserAggregate
{
    public class User : Entity, IAggregateRoot
    {
        public string UserName { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public string? PhoneNumber { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastLoginAt { get; private set; }
        public List<UserRole> UserRoles { get; private set; }

        protected User()
        {
            UserRoles = new List<UserRole>();
        }

        public User(string userName, string email, string firstName, string lastName, string? phoneNumber = null)
        {
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            PhoneNumber = phoneNumber;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
            UserRoles = new List<UserRole>();

            AddDomainEvent(new UserCreatedDomainEvent(this));
        }

        public void UpdateProfile(string firstName, string lastName, string? phoneNumber)
        {
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            PhoneNumber = phoneNumber;

            AddDomainEvent(new UserProfileUpdatedDomainEvent(this));
        }

        public void Deactivate()
        {
            if (!IsActive)
                throw new TossErpDomainException("User is already deactivated.");

            IsActive = false;
            AddDomainEvent(new UserDeactivatedDomainEvent(this));
        }

        public void Activate()
        {
            if (IsActive)
                throw new TossErpDomainException("User is already active.");

            IsActive = true;
            AddDomainEvent(new UserActivatedDomainEvent(this));
        }

        public void RecordLogin()
        {
            LastLoginAt = DateTime.UtcNow;
            AddDomainEvent(new UserLoggedInDomainEvent(this));
        }

        public void AddRole(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentException("Role name cannot be null or empty.", nameof(roleName));

            if (UserRoles.Any(ur => ur.RoleName == roleName))
                throw new TossErpDomainException($"User already has role '{roleName}'.");

            var userRole = new UserRole(roleName);
            UserRoles.Add(userRole);
            AddDomainEvent(new UserRoleAddedDomainEvent(this, roleName));
        }

        public void RemoveRole(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentException("Role name cannot be null or empty.", nameof(roleName));

            var userRole = UserRoles.FirstOrDefault(ur => ur.RoleName == roleName);
            if (userRole == null)
                throw new TossErpDomainException($"User does not have role '{roleName}'.");

            UserRoles.Remove(userRole);
            AddDomainEvent(new UserRoleRemovedDomainEvent(this, roleName));
        }

        public bool HasRole(string roleName)
        {
            return UserRoles.Any(ur => ur.RoleName == roleName);
        }

        public string FullName => $"{FirstName} {LastName}".Trim();
    }
} 
