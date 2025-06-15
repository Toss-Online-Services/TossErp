using POS.Domain.Common;

namespace POS.Domain.AggregatesModel.BuyerAggregate
{
    //This is just the RepositoryContracts or Interface defined at the Domain Layer
    //as requisite for the Buyer Aggregate

    public interface IBuyerRepository : IRepository<Buyer>
    {
        Task<Buyer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<Buyer?> GetByPhoneAsync(string phone, CancellationToken cancellationToken = default);
    }
}

