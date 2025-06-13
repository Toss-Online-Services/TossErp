namespace eShop.POS.Domain.Seedwork;

public interface IDomainEventDispatcher
{
    Task DispatchEventsAsync(IEnumerable<INotification> domainEvents);
} 
