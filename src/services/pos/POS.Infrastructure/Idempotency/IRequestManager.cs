using System.Threading.Tasks;

namespace eShop.POS.Infrastructure.Idempotency;

public interface IRequestManager
{
    Task<bool> ExistAsync(string id);

    Task CreateRequestForCommandAsync<T>(string id);
}
