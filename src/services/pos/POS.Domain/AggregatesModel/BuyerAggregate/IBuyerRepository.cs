#nullable enable
using eShop.POS.Domain.AggregatesModel.BuyerAggregate;

namespace eShop.POS.Domain.Repositories;

//This is just the RepositoryContracts or Interface defined at the Domain Layer
//as requisite for the Buyer Aggregate

public interface IBuyerRepository : IRepository<Buyer>
{
    Task<Buyer?> FindAsync(string identityGuid);
    Task<Buyer?> FindByIdAsync(string id);
    Buyer Add(Buyer buyer);
    void Update(Buyer buyer);
}

