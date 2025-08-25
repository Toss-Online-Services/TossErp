namespace Crm.Domain.Repositories;

public interface ICustomerInteractionRepository
{
    Task<CustomerInteraction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<CustomerInteraction>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<CustomerInteraction>> GetByStatusAsync(InteractionStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<CustomerInteraction>> GetByTypeAsync(InteractionType type, CancellationToken cancellationToken = default);
    Task<IEnumerable<CustomerInteraction>> GetFollowUpRequiredAsync(CancellationToken cancellationToken = default);
    Task AddAsync(CustomerInteraction interaction, CancellationToken cancellationToken = default);
    Task UpdateAsync(CustomerInteraction interaction, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
