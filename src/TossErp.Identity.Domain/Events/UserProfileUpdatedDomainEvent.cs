using MediatR;
using TossErp.Identity.Domain.AggregatesModel.UserAggregate;

namespace TossErp.Identity.Domain.Events
{
    public class UserProfileUpdatedDomainEvent : INotification
    {
        public User User { get; }

        public UserProfileUpdatedDomainEvent(User user)
        {
            User = user;
        }
    }
} 
