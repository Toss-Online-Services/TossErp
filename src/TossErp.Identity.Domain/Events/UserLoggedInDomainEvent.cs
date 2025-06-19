using MediatR;
using TossErp.Identity.Domain.AggregatesModel.UserAggregate;

namespace TossErp.Identity.Domain.Events
{
    public class UserLoggedInDomainEvent : INotification
    {
        public User User { get; }

        public UserLoggedInDomainEvent(User user)
        {
            User = user;
        }
    }
} 
