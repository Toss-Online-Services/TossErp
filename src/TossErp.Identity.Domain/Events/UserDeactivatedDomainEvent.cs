using MediatR;
using TossErp.Identity.Domain.AggregatesModel.UserAggregate;

namespace TossErp.Identity.Domain.Events
{
    public class UserDeactivatedDomainEvent : INotification
    {
        public User User { get; }

        public UserDeactivatedDomainEvent(User user)
        {
            User = user;
        }
    }
} 
