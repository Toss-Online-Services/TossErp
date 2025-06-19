using MediatR;
using TossErp.Identity.Domain.AggregatesModel.UserAggregate;

namespace TossErp.Identity.Domain.Events
{
    public class UserActivatedDomainEvent : INotification
    {
        public User User { get; }

        public UserActivatedDomainEvent(User user)
        {
            User = user;
        }
    }
} 
