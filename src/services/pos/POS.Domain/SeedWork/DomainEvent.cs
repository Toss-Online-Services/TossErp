namespace eShop.POS.Domain.Seedwork;

public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
} 
