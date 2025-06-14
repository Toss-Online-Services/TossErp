using System.Threading;
using System.Threading.Tasks;
using TossErp.POS.Domain.Common;

namespace TossErp.POS.Domain.AggregatesModel.BuyerAggregate;

//This is just the RepositoryContracts or Interface defined at the Domain Layer
//as requisite for the Buyer Aggregate

public interface IBuyerRepository : IRepository<Buyer>
{
    Task<Buyer?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Buyer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Buyer?> GetByPhoneAsync(string phone, CancellationToken cancellationToken = default);
}

