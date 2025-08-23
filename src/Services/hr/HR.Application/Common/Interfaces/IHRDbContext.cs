using Microsoft.EntityFrameworkCore;

namespace TossErp.HR.Application.Common.Interfaces;

public interface IHRDbContext
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
