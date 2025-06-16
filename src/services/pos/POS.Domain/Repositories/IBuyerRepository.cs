using POS.Domain.AggregatesModel.BuyerAggregate;
using POS.Domain.Repositories;

namespace POS.Domain.Repositories;

public interface IBuyerRepository : IRepository<Buyer>
{
    Task<Buyer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Buyer?> GetByPhoneAsync(string phone, CancellationToken cancellationToken = default);
    Task<IEnumerable<Buyer>> GetByStoreAsync(Guid storeId, CancellationToken cancellationToken = default);
} 
