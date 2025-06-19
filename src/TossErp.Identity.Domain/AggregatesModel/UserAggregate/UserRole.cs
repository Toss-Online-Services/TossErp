using System;

namespace TossErp.Identity.Domain.AggregatesModel.UserAggregate
{
    public class UserRole : Entity
    {
        public string RoleName { get; private set; } = string.Empty;
        public DateTime AssignedAt { get; private set; }

        protected UserRole() { }

        public UserRole(string roleName)
        {
            RoleName = roleName ?? throw new ArgumentNullException(nameof(roleName));
            AssignedAt = DateTime.UtcNow;
        }
    }
} 
