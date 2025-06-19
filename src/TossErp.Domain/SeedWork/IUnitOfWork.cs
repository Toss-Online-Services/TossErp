using System.Threading;
using System.Threading.Tasks;

namespace TossErp.Domain.SeedWork
{
    /// <summary>
    /// Unit of Work abstraction for transaction management.
    /// </summary>
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
} 
