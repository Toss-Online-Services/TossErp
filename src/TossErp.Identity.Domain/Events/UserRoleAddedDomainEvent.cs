using MediatR;
using TossErp.Identity.Domain.AggregatesModel.UserAggregate;

namespace TossErp.Identity.Domain.Events
{
    public class UserRoleAddedDomainEvent : INotification
    {
        public User User { get; }
        public string RoleName { get; }

        public UserRoleAddedDomainEvent(User user, string roleName)
        {
            User = user;
            RoleName = roleName;
        }
    }
} 
