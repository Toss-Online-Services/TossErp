using Crm.Domain.Entities;
using Crm.Domain.Repositories;

namespace Crm.Infrastructure.Repositories;

public class InMemoryCustomerInteractionRepository : ICustomerInteractionRepository
{
    private readonly List<CustomerInteraction> _interactions = new();
    private readonly object _lock = new();

    public Task<CustomerInteraction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var interaction = _interactions.FirstOrDefault(i => i.Id == id);
            return Task.FromResult(interaction);
        }
    }

    public Task<IEnumerable<CustomerInteraction>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var interactions = _interactions.Where(i => i.CustomerId == customerId);
            return Task.FromResult(interactions);
        }
    }

    public Task<IEnumerable<CustomerInteraction>> GetByStatusAsync(InteractionStatus status, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var interactions = _interactions.Where(i => i.Status == status);
            return Task.FromResult(interactions);
        }
    }

    public Task<IEnumerable<CustomerInteraction>> GetByTypeAsync(InteractionType type, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var interactions = _interactions.Where(i => i.Type == type);
            return Task.FromResult(interactions);
        }
    }

    public Task<IEnumerable<CustomerInteraction>> GetFollowUpRequiredAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var interactions = _interactions.Where(i => i.FollowUpDate.HasValue && i.FollowUpDate.Value <= DateTime.UtcNow);
            return Task.FromResult(interactions);
        }
    }

    public Task AddAsync(CustomerInteraction interaction, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _interactions.Add(interaction);
            return Task.CompletedTask;
        }
    }

    public Task UpdateAsync(CustomerInteraction interaction, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var existingInteraction = _interactions.FirstOrDefault(i => i.Id == interaction.Id);
            if (existingInteraction != null)
            {
                var index = _interactions.IndexOf(existingInteraction);
                _interactions[index] = interaction;
            }
            return Task.CompletedTask;
        }
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var interaction = _interactions.FirstOrDefault(i => i.Id == id);
            if (interaction != null)
            {
                _interactions.Remove(interaction);
            }
            return Task.CompletedTask;
        }
    }
}
